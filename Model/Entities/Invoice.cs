using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Info { get; set; } // можна змінити
        public DateTime Date { get; set; } // можна змінити
        public decimal DeliveryPrice { get; set; }
        public bool IsPayed { get; set; } // 
        public bool IsPartnerPayed { get; set; } // розрахунок між партнерами можна змінити
        public decimal Prepayment { get; set; } // скільки заплатили завдатку можна змінити

        [Required]
        public int CarId { get; set; }
        public virtual Car Car { get; set; }

        public virtual ICollection<InvoicePart> InvoiceParts { get; set; }

        [NotMapped]
        public decimal SumIn => InvoiceParts.Sum(x => x.SumIn);
        [NotMapped]
        public decimal SumOut => InvoiceParts.Sum(x => x.SumOut);
        [NotMapped]
        public decimal SumTotal => SumOut + DeliveryPrice;
    }
}
