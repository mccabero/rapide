namespace Rapide.Web.Models
{
    public class VehicleModelModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public VehicleMakeModel VehicleMake { get; set; }
        public int VehicleMakeId { get; set; }

        public ParameterModel BodyParameter { get; set; }
        public int BodyParameterId { get; set; }

        public ParameterModel ClassificationParameter { get; set; }
        public int ClassificationParameterId { get; set; }

        public int CreatedById { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int UpdatedById { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
