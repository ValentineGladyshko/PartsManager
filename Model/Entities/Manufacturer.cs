using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class Manufacturer : Standard
    {
        public virtual ICollection<AdditionalInfo> AdditionalInfos { get; set; }
    }
}
