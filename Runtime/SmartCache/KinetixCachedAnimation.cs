using System;

namespace Kinetix.Internal
{
    public class KinetixCachedAnimation
    {
        public AnimationIds Ids;
        public DateTime LastTimePlayed;

        public KinetixCachedAnimation(AnimationIds _Ids, DateTime _LastTimePlayed)
        {
            Ids = _Ids;
            LastTimePlayed = _LastTimePlayed;
        }

        public float GetScore()
        {
            // Naive implementation, the more recent, the better
            float score = 1 - ((float) (DateTime.Now - LastTimePlayed).TotalSeconds);

            return score;
        }
    } 
}
