using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class SaeQualityStandard : Standard
    {
        [JsonIgnore]
        public virtual ICollection<AdditionalInfo> AdditionalInfos { get; set; }
    }
}
