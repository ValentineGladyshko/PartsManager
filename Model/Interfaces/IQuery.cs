using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsManager.Model.Interfaces
{
    public interface IQuery
    {
        string GetTable();
        string GetQuery();
    }
}
