using Newtonsoft.Json;

namespace Rapide.Web.Models
{
    public class InspectionDetailsModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isGreen")]
        public bool IsGreen { get; set; }

        [JsonProperty("isAmber")]
        public bool IsAmber { get; set; }

        [JsonProperty("isRed")]
        public bool IsRed { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }
    }
}