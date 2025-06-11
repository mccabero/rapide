namespace Rapide.DTO
{
    public class InspectionTechnicianDTO : BaseDTO
    {
        public InspectionDTO Inspection { get; set; }
        public int InspectionId { get; set; }

        public UserDTO TechnicianUser { get; set; }
        public int TechnicianUserId { get; set; }
    }
}