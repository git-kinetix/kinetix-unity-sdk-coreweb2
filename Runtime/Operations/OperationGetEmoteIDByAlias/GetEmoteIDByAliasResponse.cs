namespace Kinetix.Internal
{
    public class GetEmoteIDByAliasResponse : OperationResponse
    {
        public string EmoteID;

        public GetEmoteIDByAliasResponse(string _EmoteID)
        {
            EmoteID = _EmoteID;
        }
    }
}
