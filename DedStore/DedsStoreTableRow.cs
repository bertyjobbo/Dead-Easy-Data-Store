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
        /// Primary key
        /// </summary>
        public object PrimaryKey { get; set; }

        /// <summary>
        /// Get primary key
        /// </summary>
        /// <returns></returns>
        internal void SetPrimaryKeyFromRawItem(PropertyInfo prop)
        {
            PrimaryKey = prop.GetValue(RawItem);
        }

        /// <summary>
        /// Set pk
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="prop"></param>
        internal void SetRawItemPrimaryKey(object pk, PropertyInfo prop)
        {
            prop.SetValue(RawItem,pk);
        }
    }

    ///// <summary>
    ///// Table row
    ///// </summary>
    //public class DedsStoreTableRow<T>
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="parent"></param>
    //    public DedsStoreTableRow(DedsStoreTableRow parent)
    //    {
    //        ParentItem = parent;
    //    }

    //    /// <summary>
    //    /// Raw item
    //    /// </summary>
    //    public T RawItem { get { return (T) ParentItem.RawItem; } }

    //    /// <summary>
    //    /// Parent item
    //    /// </summary>
    //    public DedsStoreTableRow ParentItem { get; private set; }
    //}
}
