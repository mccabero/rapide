
namespace Rapide.DTO
{
    public class MembershipDTO : BaseDTO
    {
        public string MembershipNo { get; set; }

        public DateTime? MembershipDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal PointsEarned { get; set; }

        public CustomerDTO Customer { get; set; }
        public int CustomerId { get; set; }

        public bool IsActive { get; set; }
    }
}
