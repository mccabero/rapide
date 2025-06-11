namespace Rapide.DTO
{
    public class EstimateTechnicianDTO : BaseDTO
    {
        public EstimateDTO Estimate { get; set; }
        public int EstimateId { get; set; }

        public UserDTO TechnicianUser { get; set; }
        public int TechnicianUserId { get; set; }
    }
}