using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Entities
{
    public class Vehicle : BaseEntity
    {
        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string? PlateNo { get; set; }

        [Required]
        [ForeignKey("VehicleModelId")] // ThenInclude(x => x.VehicleMake)
        public VehicleModel VehicleModel { get; set; }
        public int VehicleModelId { get; set; }

        [StringLength(255)]
        public string? VIN { get; set; }

        [Required]
        public int YearModel { get; set; }

        [StringLength(255)]
        public string? EngineNo { get; set; }

        [StringLength(255)]
        public string? ChasisNo { get; set; }

        [Required]
        [ForeignKey("TransmissionParameterId")] // ThenInclude(x => x.Parameter)
        public Parameter TransmissionParameter { get; set; }
        public int TransmissionParameterId { get; set; }

        [Required]
        [ForeignKey("OdometerParameterId")] // ThenInclude(x => x.Parameter)
        public Parameter OdometerParameter { get; set; }
        public int OdometerParameterId { get; set; }

        [Required]
        [ForeignKey("CustomerRegistrationTypeParameterId")] // ThenInclude(x => x.Parameter)
        public Parameter CustomerRegistrationTypeParameter { get; set; }
        public int CustomerRegistrationTypeParameterId { get; set; }

        [Required]
        [ForeignKey("EngineSizeParameterId")] // ThenInclude(x => x.Parameter)
        public Parameter EngineSizeParameter { get; set; }
        public int EngineSizeParameterId { get; set; }

        [Required]
        [ForeignKey("EngineTypeParameterId")] // ThenInclude(x => x.Parameter)
        public Parameter EngineTypeParameter { get; set; }
        public int EngineTypeParameterId { get; set; }
    }
}
