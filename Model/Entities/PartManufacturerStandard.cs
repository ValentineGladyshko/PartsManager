using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class PartManufacturerStandard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ManufacturerStandardId { get; set; }
        public virtual ManufacturerStandard ManufacturerStandard { get; set; }
        [Required]
        public int PartId { get; set; }
        public virtual Part Part { get; set; }
    }
}
