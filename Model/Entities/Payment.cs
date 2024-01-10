using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime Date { get; set; } 
        [MaxLength(255)]
        public string Info { get; set; } 

        [Required]
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
