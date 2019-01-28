using Instapaper.Core.Converters;
using Newtonsoft.Json;

namespace Instapaper.Core.Models
{
    public class User
    {
        [JsonProperty("username")]
        public string UserName { get; set; }
        
        [JsonProperty("user_id")]
        public int ID { get; set; }
        public string Type { get; set; }

        // TODO: bool converter
        [JsonProperty("subscription_is_active")]
        [JsonConverter(typeof(BoolConverter))]
        public bool SubscriptionIsActive { get; set; }

    }
}
