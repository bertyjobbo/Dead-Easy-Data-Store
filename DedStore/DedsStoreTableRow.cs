using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    /// <summary>
    /// Table row
    /// </summary>
    public class DedsStoreTableRow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public DedsStoreTableRow(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Raw item
        /// </summary>
        public object RawItem { get; set; }

        /// <summary>
        /// Added
        /// </summary>
        public bool Added { get; set; }

        /// <summary>
        /// Updated
        /// </summary>
        public bool Updated { get; set; }

        /// <summary>
        /// Primary key
        /// </summary>
        public object PrimaryKey { get; set; }

        /// <summary>
        /// Get primary key
        /// </summary>
        /// <returns></returns>
        internal void SetPrimaryKeyFromRawItem(PropertyInfo property)
        {
            PrimaryKey = property.GetValue(RawItem);
        }

        /// <summary>
        /// Set pk
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="property"></param>
        internal void SetRawItemPrimaryKey(object pk, PropertyInfo property)
        {
            property.SetValue(RawItem,pk);
        }
    }
}
