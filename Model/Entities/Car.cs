using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace PartsManager.Model.Entities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }       
        [Index(IsUnique = true), Required, MaxLength(17)]
        public string VINCode { get; set; }
        [MaxLength(255)]
        public string Info { get; set; }

        [Required]
        public int ModelId { get; set; }
        [JsonIgnore]
        public virtual Model Model { get; set; }
        [JsonIgnore]
        public virtual ICollection<Invoice> Invoices { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string FullInfo => $"{Model.Mark.Name} {Model.Name} {VINCode} {Info}";
    }
}
