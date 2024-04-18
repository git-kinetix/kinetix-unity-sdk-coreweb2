// // ----------------------------------------------------------------------------
// // <copyright file="KinetixWallet.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Threading;
using UnityEngine;
using Kinetix.Utils;

namespace Kinetix.Internal
{
	public class KinetixMetadata
	{
		/// <summary>
		/// Get metadata of a specific animation
		/// </summary>
		/// <param name="_Ids">Ids of the animation</param>
		/// <param name="_OnSuccess">Return the metadata</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void GetAnimationMetadataByAnimationIds(AnimationIds _Ids, Action<AnimationMetadata> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.GetAnimationMetadataByAnimationIds(_Ids, null, _OnSuccess, _OnFailure);
		}
		
		public void GetAnimationMetadataByAnimationIds(string _EmoteID, Action<AnimationMetadata> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.GetAnimationMetadataByAnimationIds(new AnimationIds(_EmoteID), null, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Get metadata of a specific animation
		/// </summary>
		/// <param name="_Ids">Ids of the animation</param>
		/// <param name="_OnSuccess">Return the metadata</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void GetAnimationMetadataByAnimationIds(AnimationIds _Ids, string _AvatarID, Action<AnimationMetadata> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.GetAnimationMetadataByAnimationIds(_Ids, _AvatarID, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Get metadata of a specific animation
		/// </summary>
		/// <param name="_EmoteID">Id of the animation</param>
		/// <param name="_OnSuccess">Return the metadata</param>
		public void GetAnimationMetadataByAnimationIds(string _EmoteID, string _AvatarID, Action<AnimationMetadata> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.GetAnimationMetadataByAnimationIds(new AnimationIds(_EmoteID), _AvatarID, _OnSuccess, _OnFailure);
		}

		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void IsAnimationOwnedByUser(AnimationIds _Ids, Action<bool> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.IsAnimationOwnedByUser(_Ids, _OnSuccess, _OnFailure);
		}
		
		public void IsAnimationOwnedByUser(string _EmoteID, Action<bool> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.IsAnimationOwnedByUser(new AnimationIds(_EmoteID), _OnSuccess, _OnFailure);
		}
		
		public void GetUserAnimationMetadatas(Action<AnimationMetadata[]> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.GetUserAnimationMetadatas(_OnSuccess, _OnFailure);
		}

		public void GetUserAnimationMetadatasByPage(int _Count, int _PageNumber, Action<AnimationMetadata[]> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.GetUserAnimationMetadatasByPage(_Count, _PageNumber, _OnSuccess, _OnFailure);
		}

		public void GetUserAnimationMetadatasTotalPagesCount(int _CountByPage, Action<int> _OnSuccess, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.GetUserAnimationsTotalPagesCount(_CountByPage, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Load the icon for an animation
		/// </summary>
		/// <param name="_Ids">IDs of the animations</param>
		/// <param name="_OnResponse">Delegate to recieve the response. Parameter is null on failure</param>
		/// <param name="token">Token to cancel the loading of the icon</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void LoadIconByAnimationId(AnimationIds _Ids, Action<Sprite> _OnResponse, CancellationTokenSource token = null)
		{
			KinetixMetadataBehaviour.LoadIconByAnimationId(_Ids, _OnResponse, token);
		}

		/// <summary>
		/// Load the icon for an animation
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_OnResponse">Delegate to recieve the response. Parameter is null on failure</param>
		/// <param name="token">Token to cancel the loading of the icon</param>
		public void LoadIconByAnimationId(string _EmoteID, Action<Sprite> _OnResponse, CancellationTokenSource token = null)
		{
			KinetixMetadataBehaviour.LoadIconByAnimationId(new AnimationIds(_EmoteID), _OnResponse, token);
		}

		/// <summary>
		/// Load the icon for an animation
		/// </summary>
		/// <param name="_Ids">IDs of the animations</param>
		/// <param name="_OnResponse">Delegate to recieve the response. Parameter is null on failure</param>
		/// <param name="token">Token to cancel the loading of the icon</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void LoadIconByAnimationId(AnimationIds _Ids, string _AvatarID, Action<Sprite> _OnResponse, CancellationTokenSource token = null)
		{
			KinetixMetadataBehaviour.LoadIconByAnimationId(_Ids, _AvatarID, _OnResponse, token);
		}

		/// <summary>
		/// Load the icon for an animation
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_OnResponse">Delegate to recieve the response. Parameter is null on failure</param>
		/// <param name="token">Token to cancel the loading of the icon</param>
		public void LoadIconByAnimationId(string _EmoteID, string _AvatarID, Action<Sprite> _OnResponse, CancellationTokenSource token = null)
		{
			KinetixMetadataBehaviour.LoadIconByAnimationId(new AnimationIds(_EmoteID), _AvatarID, _OnResponse, token);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_Ids"></param>
		/// <param name="_OnSuccess"></param>
		/// <param name="_OnFailure"></param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void UnloadIconByAnimationId(AnimationIds _Ids, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.UnloadIconByAnimationId(_Ids, _OnSuccess, _OnFailure);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_EmoteID"></param>
		/// <param name="_OnSuccess"></param>
		/// <param name="_OnFailure"></param>
		public void UnloadIconByAnimationId(string _EmoteID, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixMetadataBehaviour.UnloadIconByAnimationId(new AnimationIds(_EmoteID), _OnSuccess, _OnFailure);
		}
	}
}
