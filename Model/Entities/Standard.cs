using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartsManager.Model.Interfaces;

namespace PartsManager.Model.Entities
{
    public class Standard : IItem
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), Required, MaxLength(255)]
        public string Name { get; set; }
        public string Info { get; set; }
    }
}
