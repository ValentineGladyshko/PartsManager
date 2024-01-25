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
    public class Model : IItem
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
    }
}
