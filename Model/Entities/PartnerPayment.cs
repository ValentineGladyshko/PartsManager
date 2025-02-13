using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PartsManager.BaseHandlers;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsManager.Model.Entities
{
    public class PartnerPayment : IQuery
    {
        [Key]
        public int Id { get; set; }
        public decimal PaymentAmountOut { get; set; }
        public decimal BackPayment { get; set; }
        public decimal InvoicePayment { get; set; }
        [NotMapped]
        [JsonIgnore]
        public decimal PaymentAmountIn
        {
            get
            {
               return PaymentAmountOut - BackPayment - InvoicePayment;
            }
        }
        public bool IsActive { get; set; }
        public DateTime DateOut { get; set; }
        public DateTime DateIn { get; set; }
        [MaxLength(255)]
        public string Info { get; set; } 

        public string GetTable()
        {
            return "INSERT INTO PartnerPayments (Id, PaymentAmountOut, BackPayment, InvoicePayment, IsActive, DateOut, DateIn, Info) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', '{PaymentAmountOut.ToString(CultureInfo.InvariantCulture)}', '{BackPayment.ToString(CultureInfo.InvariantCulture)}', '{InvoicePayment.ToString(CultureInfo.InvariantCulture)}', '{IsActive}', '{DateOut.ToString("yyyy-MM-ddTHH:mm:ss")}', '{DateIn.ToString("yyyy-MM-ddTHH:mm:ss")}', N'{Info.Screen()}')";
        }
    }
}
