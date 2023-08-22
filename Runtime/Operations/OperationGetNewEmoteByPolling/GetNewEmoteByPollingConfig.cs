using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.Internal
{
    public class GetNewEmoteByPollingConfig : OperationConfig
    {
        public readonly string Url;
        public readonly string ApiKey;
        public readonly float  IntervalTimeInSeconds;
        
        public GetNewEmoteByPollingConfig(string _Url, string _ApiKey, float _IntervalTimeInSeconds)
        {
            Url                   = _Url;
            ApiKey                = _ApiKey;
            IntervalTimeInSeconds = _IntervalTimeInSeconds;
        }
    }
}
