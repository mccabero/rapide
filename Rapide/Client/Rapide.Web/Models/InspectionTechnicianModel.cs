namespace Rapide.Web.Models
{
    public class InspectionTechnicianModel
    {
        public int Id { get; set; }

        public InspectionModel Inspection { get; set; }
        public int InspectionId { get; set; }

        public InspectionTechnicianModel InspectionTechnician { get; set; }
        public int InspectionTechnicianId { get; set; }
    }
}
