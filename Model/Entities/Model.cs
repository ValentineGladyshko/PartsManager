using PartsManager.BaseHandlers;
using PartsManager.Model.Interfaces;
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
    public class Model : IItem, IQuery
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), Required, MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int MarkId { get; set; }
        [JsonIgnore]
        public virtual Mark Mark { get; set; }

        [JsonIgnore]
        public virtual ICollection<Car> Cars { get; set; }

        public string GetTable()
        {
            return "INSERT INTO Models (Id, Name, MarkId) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{Name.Screen()}', N'{MarkId}')";
        }
    }
}
