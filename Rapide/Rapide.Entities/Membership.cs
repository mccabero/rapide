using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapide.Entities
{
    public class Membership : BaseEntity
    {
        public string MembershipNo { get; set; }

        [Required]
        public DateTime MembershipDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public decimal PointsEarned { get; set; }

        [Required]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
