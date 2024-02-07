using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class OilPart
    {
        public Part Part { get; set; }
        public SaeQualityStandard SaeQualityStandard { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public OilPart(AdditionalInfo info, Part part) 
        { 
            SaeQualityStandard = info.SaeQualityStandard;
            Manufacturer = info.Manufacturer;
            Part = part;
        }


    }
}
