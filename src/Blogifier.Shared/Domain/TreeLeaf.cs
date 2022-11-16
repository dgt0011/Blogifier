using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogifier.Shared.Domain
{
    public class TreeLeaf
    {
        public TreeLeaf() { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        [Required]
        [StringLength(160)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int BranchId { get; set; }
    }
}
