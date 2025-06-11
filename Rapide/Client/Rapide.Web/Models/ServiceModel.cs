using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class ServiceModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Code { get; set; }

        public ServiceGroupModel ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        public ServiceCategoryModel ServiceCategory { get; set; }
        public int ServiceCategoryId { get; set; }

        public decimal StandardRate { get; set; }

        public decimal StandardHours { get; set; }

        public bool IsReplacement { get; set; }

        public bool IsAllowRateOverride { get; set; }

        public bool IsMechanicRequired { get; set; }

        public bool DisplayStandardHours { get; set; }

        public bool DisplayStandardRate { get; set; }

        public bool DisplayNotes { get; set; }
    }
}
