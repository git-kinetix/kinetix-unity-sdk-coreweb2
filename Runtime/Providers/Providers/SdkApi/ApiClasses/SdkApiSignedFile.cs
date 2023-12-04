namespace Kinetix.Internal 
{
    [System.Serializable]
    internal class SignedFile
    {
        public int id;
        public string name;
        public string extension;
        public int assetId;
        public string url;
        public System.DateTime createdAt;
    }
    
    [System.Serializable]
    internal class AvatarSignedFile
    {
        public string id;
        public string name;
        public string extension;
        public string uuid;
        public string assetId;
        public string createdAt;
        public string url;
        public string avatar;
    }
}
