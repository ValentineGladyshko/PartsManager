using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class InvoicePart : IQuery
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
        [JsonIgnore]
        public decimal SumIn => PriceIn * Count;
        [NotMapped]
        [JsonIgnore]
        public decimal SumOut => PriceOut * Count;
        [NotMapped]
        [JsonIgnore]
        public decimal RecommendedPrice => decimal.Multiply(PriceIn, 1.28m);

        [Index("IX_InvoiceAndPart", 1, IsUnique = true), Required]
        public int InvoiceId { get; set; }
        [JsonIgnore]
        public virtual Invoice Invoice { get; set; }
        [Index("IX_InvoiceAndPart", 2, IsUnique = true), Required]
        public int PartId { get; set; }
        [JsonIgnore]
        public virtual Part Part { get; set; }

        public string GetTable()
        {
            return "INSERT INTO InvoiceParts (Id, Count, PriceIn, PriceOut, InvoiceId, PartId) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', '{Count}', '{PriceIn.ToString(CultureInfo.InvariantCulture)}', '{PriceOut.ToString(CultureInfo.InvariantCulture)}', '{InvoiceId}', '{PartId}')";
        }
    }
}
