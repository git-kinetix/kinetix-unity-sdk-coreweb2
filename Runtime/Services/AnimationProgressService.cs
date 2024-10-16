// // ----------------------------------------------------------------------------
// // <copyright file="AnimationProgressService.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal;
using System;
using System.Threading.Tasks;
using UnityEngine;
using KinetixClipTask   = Kinetix.Internal.RetargetTask<Kinetix.KinetixClip, Kinetix.Internal.Retargeting.KinetixClipExporter>;

namespace Kinetix.Internal
{
	public class AnimationProgressService : IKinetixService
	{
		private ServiceLocator serviceLocator;

		public AnimationProgressService(ServiceLocator serviceLocator)
		{
			this.serviceLocator = serviceLocator;
		}

		public KinetixProgressData GetProgress(AnimationIds _Uuid)
			=> GetProgress(_Uuid.UUID);
		public KinetixProgressData GetProgress(string _Uuid)
			=> new KinetixProgressData(RetargetTaskManager.retargetTasks[_Uuid]);	
	}
}

namespace Kinetix
{
	public class KinetixProgressData
	{
		private ARetargetTask retargetTask;
		public KinetixClip kinetixClip => (retargetTask as KinetixClipTask)?.Exporter.result;
		public float progress => retargetTask.Progress;
		public ARetargetTask.Status status => retargetTask.GetStatus();

		private EExportType ExportType
		{
			get
			{
				if (retargetTask is KinetixClipTask)
					return EExportType.KinetixClip;
				
				return (EExportType)(-1);
			}
		}

		public KinetixProgressData(ARetargetTask _RetargetTask)
		{
			retargetTask = _RetargetTask;
		}

		public async void AwaitAll(Action<KinetixProgressData> _OnComplete, Action<Exception> _OnFailed)
		{
			try
			{
				await retargetTask.CreateAwaitAll();
				_OnComplete(this);
			}
			catch (Exception e)
			{
				_OnFailed(e);
			}
		}

		public async void AwaitFrame(int frame, Action<KinetixProgressData> _OnComplete, Action<Exception> _OnFailed)
		{
			try
			{
				await retargetTask.CreateAwaitFrame(frame);
				_OnComplete(this);
			}
			catch (Exception e)
			{
				_OnFailed(e);
			}
		}

		/// <param name="progress">Progress between 0 and 1</param>
		/// <param name="_OnComplete"></param>
		/// <param name="_OnFailed"></param>
		public async void AwaitProgress(float progress, Action<KinetixProgressData> _OnComplete, Action<Exception> _OnFailed)
		{
			Mathf.Clamp01(progress);
			int keyCount = ExportType switch
			{
				EExportType.KinetixClip => kinetixClip.KeyCount,
				_ => 0
			};

			try
			{
				await retargetTask.CreateAwaitFrame(Mathf.FloorToInt(keyCount*progress));
				_OnComplete(this);
			}
			catch (Exception e)
			{
				_OnFailed(e);
			}
		}

		public async Task<KinetixProgressData> AwaitAll()
		{
			await retargetTask.CreateAwaitAll();
			return this;
		}

		public async Task<KinetixProgressData> AwaitFrame(int frame)
		{
			await retargetTask.CreateAwaitFrame(frame);
			return this;
		}

		/// <param name="progress">Progress between 0 and 1</param>
		/// <param name="_OnComplete"></param>
		/// <param name="_OnFailed"></param>
		public async Task<KinetixProgressData> AwaitProgress(float progress)
		{
			Mathf.Clamp01(progress);
			int keyCount = ExportType switch
			{
				EExportType.KinetixClip => kinetixClip.KeyCount,
				_ => 0
			};

			await retargetTask.CreateAwaitFrame(Mathf.FloorToInt(keyCount*progress));
			return this;
		}
	}
}
