namespace Rapide.Web.Models
{
    public class EstimateTechnicianModel
    {
        public int Id { get; set; }

        public EstimateModel Estimate { get; set; }
        public int EstimateId { get; set; }

        public EstimateTechnicianModel EstimateTechnician { get; set; }
        public int EstimateTechnicianId { get; set; }
    }
}
