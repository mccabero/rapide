using Rapide.DTO;
using Rapide.Entities;

namespace Rapide.Web.Models
{
    public class MembershipModel
    {
        public int Id { get; set; }
        public string MembershipNo { get; set; }

        public DateTime? MembershipDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal PointsEarned { get; set; }

        public CustomerModel Customer { get; set; }

        public int CustomerId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? UpdatedDateTime { get; set; }
    }
}
