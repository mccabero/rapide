namespace Rapide.DTO
{
    public class VehicleMakeDTO : BaseDTO
    {
        public string? Name { get; set; }

        public int RegionParameterId { get; set; }

        public ParameterDTO RegionParameter { get; set; }

        public string? Description { get; set; }
    }
}