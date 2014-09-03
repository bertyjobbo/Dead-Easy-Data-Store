using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    /// <summary>
    /// Connection context interface
    /// </summary>
    public interface IDedsConnectionContext
    {
        DedsTableRowsPair<T> GetTableRowsPair<T>();
        void Commit();
    }
}
