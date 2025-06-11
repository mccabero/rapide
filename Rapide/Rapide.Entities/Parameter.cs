using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Rapide.Entities
{
    public class Parameter : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string? Code { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public int NumericData { get; set; }

        [StringLength(100)]
        public string? OtherData { get; set; }

        [Required]
        [ForeignKey("ParameterGroupId")]
        public ParameterGroup ParameterGroup { get; set; }

        public int ParameterGroupId { get; set; }
    }
}
