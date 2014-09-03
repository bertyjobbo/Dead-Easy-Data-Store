using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DedStore
{
    /// <summary>
    /// Collection
    /// </summary>
    public class DedStoreTableRowCollection
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public DedStoreTableRowCollection(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Table rows
        /// </summary>
        public IList<DedsStoreTableRow> TableRows { get; set; }

        /// <summary>
        /// Primary key prop info
        /// </summary>
        public PropertyInfo PrimaryKeyPropertyInfo { get; set; }

        /// <summary>
        /// Type of primary key
        /// </summary>
        public Type TypeOfPrimaryKey
        {
            get
            {
                return PrimaryKeyPropertyInfo == null ? default(Type) : PrimaryKeyPropertyInfo.PropertyType;
            }
        }

        /// <summary>
        /// Add table row
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item)
        {
            var newItem = new DedsStoreTableRow(item.GetType()) { Added = true, RawItem = item };
            newItem.SetPrimaryKeyFromRawItem(PrimaryKeyPropertyInfo);
            TableRows.Add(newItem);
        }
    }

    ///// <summary>
    ///// Collection
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class DedStoreTableRowCollection<T>
    //{
    //    public DedStoreTableRowCollection(DedStoreTableRowCollection parent)
    //    {
    //        ParentItem = parent;
    //    }

    //    public DedStoreTableRowCollection ParentItem { get; private set; }

    //    /// <summary>
    //    /// Table rows
    //    /// </summary>
    //    public IEnumerable<DedsStoreTableRow<T>> TableRows
    //    {
    //        get { return ParentItem.TableRows.Select(x => new DedsStoreTableRow<T>(x)); }
    //    }
    //}
}
