using System;
using System.Collections.Generic;

namespace Kinetix.Internal
{
    internal class KinetixCacheManifest
    {
        public event Action<AnimationIds> OnAnimationRemoved;


        public List<KinetixCachedAnimation> Animations => animations;

        public int TargetAnimationNb;

        private List<KinetixCachedAnimation> animations;

        

        public KinetixCacheManifest(int _AnimationNb)
        {
            animations = new List<KinetixCachedAnimation>();
            
            TargetAnimationNb = _AnimationNb;
        }

        public bool IsFull()
        {
            return animations.Count >= TargetAnimationNb;
        }

        public bool IsOverMaxCapacity()
        {
            return animations.Count > TargetAnimationNb;
        }

        public bool Contains(AnimationIds _Ids)
        {
            foreach (KinetixCachedAnimation anim in Animations)
            {
                if (anim.Ids.UUID == _Ids.UUID)
                    return true;
            }

            return false;
        }        

        public void AddOnTop(AnimationIds _Ids)
        {
            animations.Insert(0, new KinetixCachedAnimation(_Ids, DateTime.Now));
        }

        public void Add(AnimationIds _Ids)
        {
            animations.Add(new KinetixCachedAnimation(_Ids, DateTime.Now));

            Reorder();

            if (IsOverMaxCapacity())
            {
                RemoveLast();
            }
        }

        public void Remove(AnimationIds _Ids)
        {
            foreach (KinetixCachedAnimation anim in animations)
            {
                if (anim.Ids.UUID == _Ids.UUID)
                {
                    animations.Remove(anim);
                    OnAnimationRemoved?.Invoke(_Ids);
                }
            }
        }

        public void RemoveLast()
        {
            if (animations.Count == 0)
                return;

            OnAnimationRemoved?.Invoke(animations[animations.Count - 1].Ids);
            animations.RemoveAt(animations.Count - 1);
        }

        public void Reorder()
        {
            KinetixCachedAnimation tmpAnim;
            bool isOrdered = false;

            while (!isOrdered)
            {
                isOrdered = true;

                for (int i = 0; i < animations.Count; i++)
                {
                    if (i < animations.Count - 1 && animations[i + 1].GetScore() > animations[i].GetScore())
                    {
                        tmpAnim = animations[i];
                        animations[i] = animations[i + 1];
                        animations[i + 1] = tmpAnim;
                        isOrdered = false;
                    }
                }
            }
        }

        public void CleanUntilTargetNumber()
        {
            if (!IsOverMaxCapacity())
                return;

            Reorder();

            for (int i = animations.Count; i > TargetAnimationNb; i--)
            {
                RemoveLast();
            }
        }
    }
}
