using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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

        [Required]
        public int CarId { get; set; }
        [JsonIgnore]
        public virtual Car Car { get; set; }

        [JsonIgnore]
        public virtual ObservableCollection<InvoicePart> InvoiceParts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }

        [NotMapped]
        [JsonIgnore]
        public decimal SumIn
        {
            get
            {
                if (InvoiceParts == null) return 0;
                else return InvoiceParts.Sum(x => x.SumIn);
            }
        }
        [NotMapped]
        [JsonIgnore]
        public decimal SumOut
        {
            get
            {
                if (InvoiceParts == null) return 0;
                else return InvoiceParts.Sum(x => x.SumOut);
            }
        }
        [NotMapped]
        [JsonIgnore]
        public decimal SumTotal => SumOut + DeliveryPrice;
        [NotMapped]
        [JsonIgnore]
        public decimal PaymentTotal
        {
            get 
            { 
                if (Payments == null) return 0;
                else return Payments.Sum(x => x.PaymentAmount); 
            }
        }
        [NotMapped]
        [JsonIgnore]
        public decimal Residue => SumTotal - PaymentTotal;
        [NotMapped]
        [JsonIgnore]
        public decimal PartnerSum => (SumOut - SumIn) / 2;

    }
}
