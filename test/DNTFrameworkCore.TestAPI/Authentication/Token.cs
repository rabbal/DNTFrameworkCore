using Newtonsoft.Json;

namespace DNTFrameworkCore.TestAPI.Authentication
{
    public class Token
    {
        [JsonProperty("token")]
        public string Value { get; set; }
    }

}