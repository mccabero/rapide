namespace Rapide.Web.Models
{
    public class EstimateDetailsModel
    {
        public int Id { get; set; }

        public EstimateModel Estimate { get; set; }
        public int EstimateId { get; set; }

        public PackageModel Package { get; set; }
        public int PackageId { get; set; }
    }
}