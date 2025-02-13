using PartsManager.BaseHandlers;
using PartsManager.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PartsManager.Model.Entities
{
    public class ApiStandard : Standard, IQuery
    {
        [JsonIgnore]
        public virtual ICollection<PartApiStandard> PartApiStandards { get; set; }

        public string GetTable()
        {
            return "INSERT INTO ApiStandards (Id, Name, Info) VALUES ";
        }

        public string GetQuery()
        {
            return $"('{Id}', N'{Name.Screen()}', N'{Info.Screen()}')";
        }
    }
}
