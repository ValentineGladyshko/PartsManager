using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class PartApiStandard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ApiStandardId { get; set; }
        public virtual ApiStandard ApiStandard { get; set; }
        [Required]
        public int PartId { get; set; }
        public virtual Part Part { get; set; }
    }
}
