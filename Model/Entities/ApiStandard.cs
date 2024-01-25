using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class ApiStandard : Standard
    {
        [JsonIgnore]
        public virtual ICollection<PartApiStandard> PartApiStandards { get; set; }
    }
}
