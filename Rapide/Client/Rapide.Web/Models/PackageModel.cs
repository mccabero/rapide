namespace Rapide.Web.Models
{
    public class PackageModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int NextServiceReminderDays { get; set; }

        public decimal IncentiveSA { get; set; }

        public decimal IncentiveTech { get; set; }

        public bool IsHideAmount { get; set; }

        public bool IsHideService { get; set; }

        public bool IsHidePartsAndMaterials { get; set; }

        public bool IsDisplayCode { get; set; }

        public string? Summary { get; set; }

        public decimal SubTotal { get; set; }

        public decimal VAT12 { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
