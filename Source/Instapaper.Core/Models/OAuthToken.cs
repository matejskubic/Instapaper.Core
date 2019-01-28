using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Instapaper.Core.Models
{
    [DataContract]
    [DebuggerDisplay("Key = {Key}, Secret = {Secret}")]
    public class OAuthToken
    {
        [DataMember(Order = 1, IsRequired = true)]
        public string Key
        {
            get;
            private set;
        }

        [DataMember(Order = 2, IsRequired = true)]
        public string Secret
        {
            get;
            private set;
        }

        public OAuthToken(string key, string secret)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            Key = key;
            Secret = secret;
        }
    }
}
