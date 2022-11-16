using Blogifier.Shared.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogifier.Shared
{
    public class TreeBranch
    {
        public TreeBranch() { }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        [Required]
        [StringLength(160)]
        public string Title { get; set; }

        public int? ParentBranchId { get; set; }

        [NotMapped]
        public List<TreeLeaf> Leaves { get; set; }

        [NotMapped]
        public List<TreeBranch> Branches { get; set; }
    }
}
