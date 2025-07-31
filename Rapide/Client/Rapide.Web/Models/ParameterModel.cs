using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class ParameterModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Code { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public int NumericData { get; set; }

        public string? OtherData { get; set; }

        public ParameterGroupModel? ParameterGroup { get; set; }

        public int CreatedById { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int UpdatedById { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
