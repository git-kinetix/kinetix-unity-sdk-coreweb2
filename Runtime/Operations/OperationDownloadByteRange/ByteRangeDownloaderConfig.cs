namespace Kinetix.Internal
{
    public class ByteRangeDownloaderConfig : OperationConfig
    {
        public readonly string Url;
        public readonly long ByteMin;
        public readonly long ByteMax;

        public ByteRangeDownloaderConfig(string _Url, long _ByteMin, long _ByteMax)
        {
            Url    = _Url;
            ByteMin = _ByteMin;
            ByteMax = _ByteMax;
        }
    }
}
