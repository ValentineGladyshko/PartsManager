using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace PartsManager.Model.Entities
{
    public class PartApiStandard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ApiStandardId { get; set; }
        [JsonIgnore]
        public virtual ApiStandard ApiStandard { get; set; }
        [Required]
        public int PartId { get; set; }
        [JsonIgnore]
        public virtual Part Part { get; set; }
    }
}
