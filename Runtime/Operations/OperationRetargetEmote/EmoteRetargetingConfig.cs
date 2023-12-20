using Kinetix.Internal.Retargeting.AnimationData;

namespace Kinetix.Internal
{
    public class EmoteRetargetingConfig : OperationConfig
    {
        public readonly KinetixAvatar Avatar;
        public readonly KinetixEmote Emote;
        public readonly SequencerPriority Priority;
        public readonly bool IsAnimationRT3K;
        public readonly string Path;
        public readonly RuntimeRetargetFrameIndexer Indexer;
        public readonly SequencerCancel CancellationSequencer;
        public OperationConfig ResponseType;

        public EmoteRetargetingConfig(KinetixEmote _Emote, KinetixAvatar _Avatar, SequencerPriority _Priority, string _Path, SequencerCancel _CancellationSequencer, bool isAnimationRT3K)
        {
            Avatar = _Avatar;
            Emote = _Emote;
            Priority = _Priority;
            Path = _Path;
            CancellationSequencer = _CancellationSequencer;
            IsAnimationRT3K = isAnimationRT3K;
        }
        public EmoteRetargetingConfig(KinetixEmote _Emote, KinetixAvatar _Avatar, SequencerPriority _Priority, RuntimeRetargetFrameIndexer _Indexer, SequencerCancel _CancellationSequencer, bool isAnimationRT3K)
        {
            Avatar = _Avatar;
            Emote = _Emote;
            Priority = _Priority;
            Indexer = _Indexer;
            CancellationSequencer = _CancellationSequencer;
            IsAnimationRT3K = isAnimationRT3K;
        }
    }
}
