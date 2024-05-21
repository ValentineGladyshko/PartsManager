using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.ViewModels
{
    public class InvoicePartInfo
    {
        public string Index { get; set; }
        public string PartName { get; set; }
        public string Count { get; set; }
        public string PriceOut { get; set; }
        public string SumOut { get; set;}
    }
}
