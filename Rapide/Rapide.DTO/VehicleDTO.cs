namespace Rapide.DTO
{
    public class VehicleDTO : BaseDTO
    {
        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public string? PlateNo { get; set; }

        public VehicleModelDTO VehicleModel { get; set; }
        public int VehicleModelId { get; set; }

        public string? VIN { get; set; }

        public int YearModel { get; set; }

        public string? EngineNo { get; set; }

        public string? ChasisNo { get; set; }

        public ParameterDTO TransmissionParameter { get; set; }
        public int TransmissionParameterId { get; set; }

        public ParameterDTO OdometerParameter { get; set; }
        public int OdometerParameterId { get; set; }

        public ParameterDTO CustomerRegistrationTypeParameter { get; set; }
        public int CustomerRegistrationTypeParameterId { get; set; }

        public ParameterDTO EngineSizeParameter { get; set; }
        public int EngineSizeParameterId { get; set; }

        public ParameterDTO EngineTypeParameter { get; set; }
        public int EngineTypeParameterId { get; set; }
    }
}