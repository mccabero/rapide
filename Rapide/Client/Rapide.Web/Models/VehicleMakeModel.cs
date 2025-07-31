namespace Rapide.Web.Models
{
    public class VehicleMakeModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public ParameterModel RegionParameter { get; set; }
        public int RegionParameterId { get; set; }

        public int CreatedById { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int UpdatedById { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
