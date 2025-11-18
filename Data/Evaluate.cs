using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HTNLShop.Data
{
    public class Evaluate
    {
        [Key]
        public int ReviewId { get; set;}

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public string? Comment { get; set; }

        public int Rate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
    }
}
