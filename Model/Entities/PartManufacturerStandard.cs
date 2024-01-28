using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class PartManufacturerStandard : IQuery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ManufacturerStandardId { get; set; }
        [JsonIgnore]
        public virtual ManufacturerStandard ManufacturerStandard { get; set; }
        [Required]
        public int PartId { get; set; }
        [JsonIgnore]
        public virtual Part Part { get; set; }

        public string GetTable()
        {
            return "INSERT INTO PartManufacturerStandards (Id, ManufacturerStandardId, PartId) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{ManufacturerStandardId}', '{PartId}')";
        }
    }
}
