// // ----------------------------------------------------------------------------
// // <copyright file="TaskUtils.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Kinetix.Internal.Utils
{
	public static class TaskUtils
	{
		public static async Task Catch(this Task task, Action<Exception> catchMethod)
		{
			try
			{
				await task;
			}
			catch (Exception e)
			{
				catchMethod(e);
			}
		}
		public static async Task<T> Catch<T>(this Task<T> task, Action<Exception> catchMethod)
		{
			T result;
			try
			{
				result = await task;
			}
			catch (Exception e)
			{
				catchMethod(e);
				result = default;
			}

			return result;
		}

		public static async Task Then(this Task task, Func<Task> then)
		{
			await task;
			await then();
		}

		public static async Task<TReturn> Then<TReturn>(this Task task, Func<Task<TReturn>> then)
		{
			await task;
			return await then();
		}

		public static async Task Then<T>(this Task<T> task, Func<T, Task> then)
		{
			T result = await task;
			await then(result);
		}

		public static async Task<TReturn> Then<T, TReturn>(this Task<T> task, Func<T, Task<TReturn>> then)
		{
			T result = await task;
			return await then(result);
		}
		public static async Task Then(this Task task, Action then)
		{
			await task;
			then();
		}

		public static async Task<TReturn> Then<TReturn>(this Task task, Func<TReturn> then)
		{
			await task;
			return then();
		}

		public static async Task Then<T>(this Task<T> task, Action<T> then)
		{
			T result = await task;
			then(result);
		}

		public static async Task<TReturn> Then<T, TReturn>(this Task<T> task, Func<T, TReturn> then)
		{
			T result = await task;
			return then(result);
		}
	}
}
