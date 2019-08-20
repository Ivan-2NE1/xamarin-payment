
using Newtonsoft.Json;

namespace QuickDate.Helpers.SocialLogins
{
    public class FacebookResult
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
    }
}