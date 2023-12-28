using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class Mark : IComparable<Mark>, IItem
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), Required, MaxLength(255)]
        public string Name { get; set; }

        public virtual ICollection<Model> Models { get; set; }

        public int CompareTo(Mark other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
