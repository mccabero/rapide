using Newtonsoft.Json;

namespace Rapide.Web.Models
{
    public class InspectionGroupModel
    {
        public int Sequence { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("detailsModelList")]
        public List<InspectionDetailsModel> DetailsModelList { get; set; }
    }
}

