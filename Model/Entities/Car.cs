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
using PartsManager.Model.Interfaces;
using System.IO.Ports;

namespace PartsManager.Model.Entities
{
    public class Car : IQuery
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

        public string GetTable()
        {
            return "INSERT INTO Cars (Id, VINCode, Info, ModelId) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{VINCode}', N'{Info}', '{ModelId}')";
        }
    }
}
