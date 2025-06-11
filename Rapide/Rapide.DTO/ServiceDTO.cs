namespace Rapide.DTO
{
    public class ServiceDTO : BaseDTO
    {
        public string? Name { get; set; }

        public string? Code { get; set; }

        public ServiceGroupDTO ServiceGroup { get; set; }
        public int ServiceGroupId { get; set; }

        public ServiceCategoryDTO ServiceCategory { get; set; }
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