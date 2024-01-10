using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class InvoicePart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public decimal PriceIn { get; set; }
        [Required]
        public decimal PriceOut { get; set; }
        [NotMapped]
        public decimal SumIn => PriceIn * Count;
        [NotMapped]
        public decimal SumOut => PriceOut * Count;
        [NotMapped]
        public decimal RecommendedPrice => decimal.Multiply(PriceIn, 1.2m);

        [Index("IX_InvoiceAndPart", 1, IsUnique = true), Required]
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        [Index("IX_InvoiceAndPart", 2, IsUnique = true), Required]
        public int PartId { get; set; }
        public virtual Part Part { get; set; }
    }
}
