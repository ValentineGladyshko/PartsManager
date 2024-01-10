using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class ApiStandard : Standard
    {
        public virtual ICollection<PartApiStandard> PartApiStandards { get; set; }
    }
}
