using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using PartsManager.Model.Interfaces;

namespace PartsManager.Model.Entities
{
    public class AdditionalInfo : IQuery
    {
        [Key]
        public int Id { get; set; }
        public string Info { get; set; }

        [Index(IsUnique = true), Required]
        public int PartId { get; set; }
        public virtual Part Part { get; set; }

        [Required]
        public int SaeQualityStandardId { get; set; }
        [JsonIgnore]
        public virtual SaeQualityStandard SaeQualityStandard { get; set; }

        [Required]
        public int ManufacturerId { get; set; }
        [JsonIgnore]
        public virtual Manufacturer Manufacturer { get; set; }

        public string GetTable()
        {
            return "INSERT INTO AdditionalInfoes (Id, Info, PartId, SaeQualityStandardId, ManufacturerId) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{Info}', '{PartId}', '{SaeQualityStandardId}', '{ManufacturerId}')";
        }
    }
}
