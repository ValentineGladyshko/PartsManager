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
        public string Info { get; set; }
        public DateTime Date { get; set; }
        public decimal DeliveryPrice { get; set; }
        public bool IsPayed { get; set; }
        public bool IsPayed2 { get; set; } // розрахунок між партнерами
        public decimal Prepayment { get; set; } // скільки заплатили завдатку

        [Required]
        public int CarId { get; set; }
        public virtual Car Car { get; set; }

        public virtual ICollection<InvoicePart> InvoiceParts { get; set; }
    }
}
