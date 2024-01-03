using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class Part : IItem
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Index(IsUnique = true), Required, MaxLength(25)]
        public string Article { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public int PartTypeId { get; set; }
        public virtual PartType PartType { get; set; }

        public virtual ICollection<InvoicePart> InvoiceParts { get; set; }

        public override string ToString() => $"{PartType} {Name} {Article}";
        [NotMapped]
        public string FullInfo => $"{PartType} {Name} {Article}";
    }
}
