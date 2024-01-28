using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class ManufacturerStandard : Standard, IQuery
    {
        [JsonIgnore]
        public virtual ICollection<PartManufacturerStandard> PartManufacturerStandards { get; set; }

        public string GetTable()
        {
            return "INSERT INTO ManufacturerStandards (Id, Name, Info) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{Name}', N'{Info}')";
        }
    }
}
