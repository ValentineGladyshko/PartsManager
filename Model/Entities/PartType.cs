using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class PartType : IItem, IQuery
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Part> Parts { get; set;}

        public override string ToString() => $"{Name}";

        public string GetTable()
        {
            return "INSERT INTO PartTypes (Id, Name) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{Name}')";
        }
    }
}
