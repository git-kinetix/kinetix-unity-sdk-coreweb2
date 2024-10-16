using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Kinetix.Internal
{
    public class OperationSequencer
    {
        private readonly Queue<IOperationBatch> operationsQueue;

        private bool isRunning;
        
        public OperationSequencer()
        {
            operationsQueue = new Queue<IOperationBatch>();
            isRunning       = false;
        }

        public void Enqueue<TConfig, TResponse>(IOperationBatch<TConfig, TResponse> _OperationBatch)
            where TConfig : OperationConfig
            where TResponse : OperationResponse
        {
            if (IsOperationBatchInQueue(_OperationBatch))
                return;

            operationsQueue.Enqueue(_OperationBatch);
            
            if (isRunning)
                return;
            
            RunNext();
        }

        private bool IsOperationBatchInQueue<TConfig, TResponse>(IOperationBatch<TConfig, TResponse> _OperationBatch)
            where TConfig : OperationConfig
            where TResponse : OperationResponse
        {
            return operationsQueue.Any(operationBatch =>
            {
                IOperationBatch<TConfig, TResponse> op = operationBatch as IOperationBatch<TConfig, TResponse>;
                return op != null && op.GetOperation().Compare(_OperationBatch.GetOperation().Config);
            });
        }
        
        private async void RunNext()
        {
            IOperationBatch operationBatch = operationsQueue.Peek();
            if (operationBatch.GetOperation().IsCompleted())
            {
                operationsQueue.Dequeue();
                if (operationsQueue.Count > 0)
                    RunNext();
                return;
            }
            
            Task task = operationBatch.GetOperation().Execute();
            
            isRunning = true;
            try
            {
                await task;
            }
            catch (TaskCanceledException e)
            {
#if DEV_KINETIX
				KinetixLogger.LogInfo("Cancel", "Canceled an operation. " + Environment.NewLine + e.StackTrace, true);
#endif
                operationBatch.GetOperation().CancellationTokenSource.Cancel();
		    }
		    isRunning = false;
            
            operationsQueue.Dequeue();

            if (operationsQueue.Count > 0)
                RunNext();
        }
        
    }
}
