using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kinetix.Internal
{
    public abstract class EmoteRetargetingClipResult : IDisposable
    {
        public abstract void Dispose();
        public abstract bool HasClip();
    }

    public class EmoteRetargetingClipResult<TResponseType>: EmoteRetargetingClipResult
    {
        public TResponseType                                                 Clip;
        public EProgressStatus                                               Status;
        public TaskCompletionSource<EmoteRetargetingResponse<TResponseType>> Task;

        public EmoteRetargetingClipResult(TResponseType _Clip, EProgressStatus _Status, TaskCompletionSource<EmoteRetargetingResponse<TResponseType>> _Task = null)
        {
            Clip   = _Clip;
            Status = _Status;
            Task   = _Task;
        }

        public EmoteRetargetingClipResult(EProgressStatus _Status, TaskCompletionSource<EmoteRetargetingResponse<TResponseType>> _Task = null)
        {
            Status = _Status;
            Task   = _Task;
        }

        public override bool HasClip()
        {
            return Clip != null;
        }
        
        public override void Dispose()
        {
            if (Clip is AnimationClip clip)
            {
                Object.Destroy(clip);
            }
        }
    }
}
