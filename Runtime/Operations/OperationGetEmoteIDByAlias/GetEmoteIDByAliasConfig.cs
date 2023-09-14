namespace Kinetix.Internal
{
    public class GetEmoteIDByAliasConfig : OperationConfig
    {
        public readonly string ApiKey;
        public readonly string Url;

        public GetEmoteIDByAliasConfig(string _ApiKey, string _Url)
        {
            Url    = _Url;
            ApiKey = _ApiKey;
        }
    }
}
