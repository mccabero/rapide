using System.ComponentModel.DataAnnotations.Schema;

namespace Rapide.Web.Models
{
    public class VehiclesModel
    {
        public int Id { get; set; }

        [NotMapped]
        public string FullName { get; set; }

        public CustomerModel Customer { get; set; }
        public int CustomerId { get; set; }

        public string? PlateNo { get; set; }

        public VehicleModelModel VehicleModel { get; set; }
        public int VehicleModelId { get; set; }

        public string? VIN { get; set; }

        public int YearModel { get; set; }

        public string? EngineNo { get; set; }

        public string? ChasisNo { get; set; }

        public ParameterModel TransmissionParameter { get; set; }
        public int TransmissionParameterId { get; set; }

        public ParameterModel OdometerParameter { get; set; }
        public int OdometerParameterId { get; set; }

        public ParameterModel CustomerRegistrationTypeParameter { get; set; }
        public int CustomerRegistrationTypeParameterId { get; set; }

        public ParameterModel EngineSizeParameter { get; set; }
        public int EngineSizeParameterId { get; set; }

        public ParameterModel EngineTypeParameter { get; set; }
        public int EngineTypeParameterId { get; set; }

        public int CreatedById { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int UpdatedById { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
