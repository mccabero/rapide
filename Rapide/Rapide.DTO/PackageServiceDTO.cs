namespace Rapide.DTO
{
    public class PackageServiceDTO : BaseDTO
    {
        public PackageDTO Package { get; set; }
        public int PackageId { get; set; }

        public ServiceDTO Service { get; set; }
        public int ServiceId { get; set; }

        public decimal Rate { get; set; }

        public decimal Hours { get; set; }

        public decimal Amount { get; set; }
    }
}