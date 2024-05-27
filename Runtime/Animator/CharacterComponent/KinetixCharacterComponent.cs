// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCharacterComponent.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using UnityEngine;
using Kinetix.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Kinetix
{
	/// <summary>
	/// Kinetix Character Component
	/// </summary>
	public abstract class KinetixCharacterComponent : IDisposable
	{

		/// <summary>
		/// Called when an animation starts playing
		/// </summary>
		public event Action<AnimationIds> OnAnimationStart;
		/// <summary>
		/// Called when an animation stop playing
		/// </summary>
		public event Action<AnimationIds> OnAnimationEnd;
		/// <summary>
		/// Called on each frame of the animation
		/// </summary>
		public event Action OnPlayedFrame;

		/// <summary>
		/// If true, the animation will automaticaly play.<br/>
		/// If false you can handle the animation using the events<br/>
		/// <list type="bullet">
		///		<item>
		///		<term><see cref="OnAnimationStart"/></term>
		///     <description> Animation Start </description>
		///		</item>
		///		<item>
		///		<term><see cref="OnAnimationEnd"/></term>
		///     <description> Animation End </description>
		///		</item>
		///		<item>
		///		<term><see cref="OnPlayedFrame"/></term>
		///     <description> Animation Update </description>
		///		</item>
		/// </list>
		/// </summary>
		public virtual bool AutoPlay { get => _autoPlay; set => _autoPlay = value; }
		private bool _autoPlay = false;

		internal protected KinetixNetworkSampler networkSampler;

		protected KinetixCharacterComponentBehaviour behaviour;

		// CACHE
		protected KinetixAvatar kinetixAvatar;
        
		protected readonly List<IPoseInterpreter> poseInterpretor = new List<IPoseInterpreter>();
		protected HumanBodyBones[] characterBones;
		protected ServiceLocator serviceLocator;

		/// <summary>
		/// Init the Character
		/// </summary>
		/// <param name="_ServiceLocator">The service locator</param>
		/// <param name="_KinetixAvatar">The avatar to use for the animation</param>
		public void Init(ServiceLocator _ServiceLocator, KinetixAvatar _KinetixAvatar)
			=> Init(_ServiceLocator, _KinetixAvatar, null);

		/// <summary>
		/// Init the Character
		/// </summary>
		/// <param name="_ServiceLocator">The service locator</param>
		/// <param name="_KinetixAvatar">The avatar to use for the animation</param>
		/// <param name="_RootMotionConfig">Configuration of the root motion</param>
		public virtual void Init(ServiceLocator _ServiceLocator, KinetixAvatar _KinetixAvatar, RootMotionConfig _RootMotionConfig)
		{
			networkSampler = new KinetixNetworkSampler();
			behaviour = _KinetixAvatar.Root.gameObject.AddComponent<KinetixCharacterComponentBehaviour>();
			behaviour._kcc = this;
			behaviour.OnUpdate += Update;

			this.kinetixAvatar = _KinetixAvatar;

			AvatarData avatar = _KinetixAvatar.Avatar;

			//Get the human bones
			//NOTE: Maybe we can create an extension method from this
            if (avatar.avatar != null)
            {
                characterBones = avatar.avatar.humanDescription.human.Select(h => UnityHumanUtils.HUMANS.IndexOf(h.humanName) >= 0 ? UnityHumanUtils.HUMANS_UNITY[UnityHumanUtils.HUMANS.IndexOf(h.humanName)] : UnityHumanUtils.HUMANS_UNITY[UnityHumanUtils.HUMANS_UNITY.Count - 1]).ToArray();
            }
			else if (avatar.hierarchy != null)
			{
				characterBones = avatar.hierarchy.Where(h => h.m_humanBone.HasValue).Select(h => h.m_humanBone.Value).ToArray();
			}
			else
			{
				throw new ArgumentNullException(nameof(_KinetixAvatar)+"."+nameof(_KinetixAvatar.Avatar));
			}
			//----//

			networkSampler = new KinetixNetworkSampler();
		}

		/// <summary>
		/// Update (unity message given by the <see cref="behaviour"/>)
		/// </summary>
		protected virtual void Update() { }

		#region Abstract

		#endregion

		#region Interpreter
		/// <summary>
		/// Set a pose interpreter as the main interpreter<br/>
		/// (It's the only one that will recieve <see cref="IPoseInterpreter.GetPose"/>)
		/// </summary>
		/// <param name="interpreter"></param>
		public void SetMainPoseInterpreter(IPoseInterpreter interpreter)
		{
			poseInterpretor.Remove(interpreter);
			poseInterpretor.Insert(0, interpreter);
		}

		/// <summary>
		/// Add a pose interpreter to the list of interpreters
		/// </summary>
		/// <param name="interpreter"></param>
		public void RegisterPoseInterpreter(IPoseInterpreter interpreter)
		{
			poseInterpretor.Remove(interpreter);
			poseInterpretor.Add(interpreter);
		}

		/// <summary>
		/// Remove a pose interpreter from the list of interpreters
		/// </summary>
		/// <param name="interpreter"></param>
		public void UnregisterPoseInterpreter(IPoseInterpreter interpreter)
		{
			poseInterpretor.Remove(interpreter);
		}
		#endregion

		#region Call events
		/// <summary>
		/// Call <see cref="OnAnimationStart"/>
		/// </summary>
		protected void Call_OnAnimationStart(AnimationIds _Ids) => OnAnimationStart?.Invoke(_Ids);
		/// <summary>
		/// Call <see cref="OnAnimationEnd"/>
		/// </summary>
		protected void Call_OnAnimationEnd(AnimationIds _Ids) => OnAnimationEnd?.Invoke(_Ids);
		/// <summary>
		/// Call <see cref="OnPlayedFrame"/>
		/// </summary>
		protected void Call_OnPlayedFrame() => OnPlayedFrame?.Invoke();
		#endregion

		/// <summary>
		/// Dispose the character.
		/// </summary>
		public virtual void Dispose()
		{
			OnAnimationStart = null;
			OnAnimationEnd = null;
			OnPlayedFrame = null;

			if (behaviour != null)
			{
				behaviour._kcc = null;
				UnityEngine.Object.DestroyImmediate(behaviour, true);
			}
		}
	}
}
