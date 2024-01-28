using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class Part : IItem, IQuery
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required, MaxLength(255)]
        public string FullName { get; set; }
        [Index(IsUnique = true), Required, MaxLength(63)]
        public string Article { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public int PartTypeId { get; set; }
        [JsonIgnore]
        public virtual PartType PartType { get; set; }

        [JsonIgnore]
        public virtual ICollection<InvoicePart> InvoiceParts { get; set; }

        public override string ToString() => $"{Name} {Article}";
        [NotMapped]
        [JsonIgnore]
        public string FullInfo => $"{FullName} {Article}";

        public string GetTable()
        {
            return "INSERT INTO Parts (Id, Name, FullName, Article, Description, PartTypeId) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{Name}', N'{FullName}', N'{Article}', N'{Description}', '{PartTypeId}')";
        }
    }
}
