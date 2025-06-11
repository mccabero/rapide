namespace Rapide.DTO
{
    public class VehicleModelDTO : BaseDTO
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public VehicleMakeDTO VehicleMake { get; set; }

        public int VehicleMakeId { get; set; }

        public int BodyParameterId { get; set; }
        public ParameterDTO BodyParameter { get; set; }

        public int ClassificationParameterId { get; set; }
        public ParameterDTO ClassificationParameter { get; set; }
    }
}