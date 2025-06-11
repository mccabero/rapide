namespace Rapide.Web.Models
{
    public class JobOrderTechnicianModel
    {
        public int Id { get; set; }

        public JobOrderModel JobOrder { get; set; }
        public int JobOrderId { get; set; }

        public JobOrderTechnicianModel JobOrderTechnician { get; set; }
        public int JobOrderTechnicianId { get; set; }
    }
}
