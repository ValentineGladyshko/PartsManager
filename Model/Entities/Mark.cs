using PartsManager.BaseHandlers;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class Mark : IComparable<Mark>, IItem, IQuery
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), Required, MaxLength(255)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Model> Models { get; set; }

        public int CompareTo(Mark other)
        {
            return Name.CompareTo(other.Name);
        }

        public string GetTable()
        {
            return "INSERT INTO Marks (Id, Name) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{Name.Screen()}')";
        }
    }
}
