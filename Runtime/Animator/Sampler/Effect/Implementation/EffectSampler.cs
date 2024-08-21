// // ----------------------------------------------------------------------------
// // <copyright file="EffectSampler.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Kinetix.Internal
{
	/// <summary>
	/// Class used by the <see cref="SimulationSampler"/> to sample effects before sending the _Frame to the network and the
	/// </summary>
	public class EffectSampler
	{
#pragma warning disable IDE0044 // readonly
		internal List<IFrameEffect>         frameEffect   = new List<IFrameEffect>();
		private  List<IFrameEffectAdd>      frameAdds     = new List<IFrameEffectAdd>();
		private  List<IFrameEffectModify>   frameModify   = new List<IFrameEffectModify>();
		private  List<ISamplerAuthority>     authority     = new List<ISamplerAuthority>();
		
		private SamplerAuthorityBridge authorityBridge;


		/// <param name="_AuthorityBridge">
		/// Authority on the _Tracks.<br/>
		/// You can call some methods like "StartNextClip" or "GetAvatarPos".
		/// </param>
		public EffectSampler(SamplerAuthorityBridge _AuthorityBridge)
		{
			this.authorityBridge = _AuthorityBridge;
		}

		/// <summary>
		/// Called when <see cref="IFrameEffectAdd"/> added a _Frame to play
		/// </summary>
		public event Action<KinetixFrame> OnFrameAdded;
#pragma warning restore IDE0044 // readonly

		/// <summary>
		/// Add an _Effect to the list of _Effect
		/// </summary>
		/// <param name="_Effect"></param>
		public void RegisterEffect(IFrameEffect _Effect)
		{
			bool added = false;

			if (_Effect is IFrameEffectAdd add)
			{
				added = true;
				if (!frameAdds.Contains(add))
				{
					AddToArray(frameAdds, add);
					add.OnAddFrame -= IFrameEffectAdd_OnAddFrame;
					add.OnAddFrame += IFrameEffectAdd_OnAddFrame;
				}
			}
			
			if (_Effect is IFrameEffectModify modify)
			{
				added = true;

				AddToArray(frameModify, modify);
			}

			if (_Effect is ISamplerAuthority auth)
			{
				//At the moment there's no priority for authority
				if (!authority.Contains(auth))
				{
					authority.Add(auth);
					auth.Authority = authorityBridge;
				}
			}

			if (!added)
			{
				KinetixLogger.LogError(
					nameof(EffectSampler),
					nameof(IFrameEffect) + " should be specialised in at least one of the child interface. See " + nameof(IFrameEffectAdd) + " and " + nameof(IFrameEffectModify),
					true
				);
			}
			else
			{
				AddToArray(frameEffect, _Effect);
			}
		}

		private void AddToArray<TEffect>(List<TEffect> _List, TEffect _Item) where TEffect : IFrameEffect
		{
			/* insertIndex optimises access to '_List.Count' when loop start */
			bool insertFound = false;
			int insertIndex = _List.Count;
			for (int i = insertIndex - 1; i >= 0; i--)
			{
				TEffect effect = _List[i];
				if (effect.GetHashCode() == _Item.GetHashCode() && effect.Equals(_Item))
					return;

				if (!insertFound && _List[i].Priority < _Item.Priority)
					insertIndex = i;
				else
					insertFound = true;
			}

			_List.Insert(insertIndex, _Item);
		}

		/// <summary>
		/// Remove an _Effect from the list of _Effect
		/// </summary>
		/// <param name="_Effect"></param>
		public void UnRegisterEffect(IFrameEffect _Effect)
		{
			bool removed = false;

			frameEffect.Remove(_Effect);
	
			if (_Effect is IFrameEffectAdd add)
			{
				removed = true;
				frameAdds.Remove(add);
				add.OnAddFrame -= IFrameEffectAdd_OnAddFrame;
			}

			if (_Effect is IFrameEffectModify modify)
			{
				removed = true;
				frameModify.Remove(modify);
			}

			if (_Effect is ISamplerAuthority start)
			{
				removed = true;
				authority.Remove(start);
			}

			if (!removed)
			{
				KinetixLogger.LogError(
					nameof(EffectSampler),
					nameof(IFrameEffect) + " shall be specialised in at least one of the child interface. See " + nameof(IFrameEffectAdd) + " and " + nameof(IFrameEffectModify),
					true
				);
			}
		}

		private void IFrameEffectAdd_OnAddFrame(KinetixFrame _Frame)
		{
			ModifyFrame(ref _Frame);
			OnFrameAdded?.Invoke(_Frame);
		}
		
		/// <summary>
		/// Modify a single _Frame (called by <see cref="IFrameEffectAdd_OnAddFrame"/>
		/// </summary>
		/// <param name="_Frame"></param>
		public void ModifyFrame(ref KinetixFrame _Frame)
			=> ModifyFrame(new KinetixFrame[] { _Frame }, null);

		/// <summary>
		/// Compute the effects for each <see cref="SimulationSampler.Sampler"/>'s result. The first non null _Frame is considered the main _Frame
		/// </summary>
		/// <param name="_Frame">Result of the _Tracks</param>
		/// <returns></returns>
		public KinetixFrame ModifyFrame(in KinetixFrame[] _Frame, in KinetixClipTrack[] _Tracks)
		{
			KinetixFrame toReturn = new KinetixFrame(_Frame[0]);

			int count = frameModify.Count;
			for (int i = 0; i < count; i++)
			{
				frameModify[i].OnPlayedFrame(ref toReturn, _Frame, in _Tracks);
			}

			return toReturn;
		}

		/// <summary>
		/// Dispatch the "Update" to the effects
		/// </summary>
		/// <param name="clip"></param>
		public void Update()
		{
			int lenght = frameEffect.Count;
			for (int i = 0; i < lenght; i++)
			{
				frameEffect[i].Update();
			}
		}

		/// <summary>
		/// Dispatch the "OnQueueStart" to the effects
		/// </summary>
		public void OnQueueStart()
		{
			int lenght = frameEffect.Count;
			for (int i = 0; i < lenght; i++)
			{
				frameEffect[i].OnQueueStart();
			}
		}

		/// <summary>
		/// Dispatch the "OnQueueEnd" to the effects
		/// </summary>
		public void OnQueueStop()
		{
			int lenght = frameEffect.Count;
			for (int i = 0; i < lenght; i++)
			{
				frameEffect[i].OnQueueEnd();
			}
		}

		/// <summary>
		/// Dispatch the "OnSoftStop" to the effects
		/// </summary>
		public void OnSoftStop(float _SoftDuration)
		{
			int lenght = frameEffect.Count;
			for (int i = 0; i < lenght; i++)
			{
				frameEffect[i].OnSoftStop(_SoftDuration);
			}
		}


	}
}
