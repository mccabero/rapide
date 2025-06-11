namespace Rapide.DTO
{
    public class JobOrderTechnicianDTO : BaseDTO
    {
        public JobOrderDTO JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public UserDTO TechnicianUser { get; set; }
        public int TechnicianUserId { get; set; }
    }
}