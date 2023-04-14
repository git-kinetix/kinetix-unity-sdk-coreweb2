using System;
using System.Collections.Generic;
using System.Linq;
using Kinetix.Internal.Cache;
using Kinetix.Internal.Utils;
using UnityEngine;

namespace Kinetix.Internal
{
    public static class AccountManager
    {
        public static Action OnUpdatedAccount;

        public static void Initialize()
        {
            AddFreeAnimations();
        }

        public static async void GetAllUserEmotes(Action<AnimationMetadata[]> _OnSuccess, Action _OnFailure = null)
        {
            List<KinetixEmote> emotesAccountAggregation = new List<KinetixEmote>();

            try
            {
                KinetixEmote[] freeEmotes = await FreeAnimationsManager.GetFreeEmotes();
                emotesAccountAggregation.AggregateAndDistinct(freeEmotes);
                _OnSuccess?.Invoke(emotesAccountAggregation.Select(emote => emote.Metadata).ToArray());
            }
            catch (Exception e)
            {
                KinetixDebug.LogWarning(e.Message);
                _OnSuccess?.Invoke(emotesAccountAggregation.Select(emote => emote.Metadata).ToArray());
            }
        }

        public static void AddFreeAnimations()
        {
            FreeAnimationsManager.AddFreeAnimations(OnUpdatedAccount);
        }
        
        public static void IsAnimationOwnedByUser(AnimationIds _AnimationIds, Action<bool> _OnSuccess, Action _OnFailure = null)
        {
            GetAllUserEmotes(metadatas => { _OnSuccess.Invoke(metadatas.ToList().Exists(metadata => metadata.Ids.Equals(_AnimationIds))); }, _OnFailure);
        }
        
        public static void GetUserAnimationsMetadatasByPage(int _Count, int _Page, Action<AnimationMetadata[]> _Callback, Action _OnFailure)
        {
            GetAllUserEmotes(animationMetadatas =>
            {
                if ((_Page + 1) * _Count <= animationMetadatas.Length)
                {
                    _Callback?.Invoke(animationMetadatas.ToList().GetRange((_Page * _Count), _Count).ToArray());
                }
                else
                {
                    int lastPageCount = animationMetadatas.Length % _Count;
                    _Callback?.Invoke(animationMetadatas.ToList()
                        .GetRange(animationMetadatas.Length - lastPageCount, lastPageCount).ToArray());
                }
            }, () => { _OnFailure?.Invoke(); });
        }

        public static void GetUserAnimationsTotalPagesCount(int _CountByPage, Action<int> _Callback, Action _OnFailure)
        {
            GetAllUserEmotes(animationMetadatas =>
            {
                if (animationMetadatas.Length == 0)
                {
                    _Callback?.Invoke(1);
                    return;
                }

                int totalPage = Mathf.CeilToInt((float)animationMetadatas.Length / (float)_CountByPage);
                _Callback?.Invoke(totalPage);
            }, () => { _OnFailure?.Invoke(); });
        }
    }
}
