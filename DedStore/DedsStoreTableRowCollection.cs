using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            ItemsToAdd = new List<DedsStoreTableRow>();
            ItemsToRemove = new List<DedsStoreTableRow>();
            ItemsToUpdate = new List<DedsStoreTableRow>();
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
        /// Items to update
        /// </summary>
        public IList<DedsStoreTableRow> ItemsToAdd { get; set; }

        /// <summary>
        /// Items to update
        /// </summary>
        public IList<DedsStoreTableRow> ItemsToUpdate { get; set; }

        /// <summary>
        /// Items to delete
        /// </summary>
        public IList<DedsStoreTableRow> ItemsToRemove { get; set; }

        /// <summary>
        /// Has commits
        /// </summary>
        public bool HasCommits
        {
            get { return ItemsToAdd.Count + ItemsToUpdate.Count + ItemsToRemove.Count > 0; }
        }

        /// <summary>
        /// Add table row
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item)
        {
            var newItem = new DedsStoreTableRow(item.GetType()) { Added = true, RawItem = item };
            newItem.SetPrimaryKeyFromRawItem(PrimaryKeyPropertyInfo);
            if (PrimaryKeyPropertyInfo.PropertyType != typeof(int) && ItemsToAdd.Select(x => x.PrimaryKey).Contains(newItem.PrimaryKey))
                throw new Exception("Object with primary key of '" + newItem.PrimaryKey + "'" + "already in add collection");
            ItemsToAdd.Add(newItem);
            TableRows.Add(newItem);
        }

        /// <summary>
        /// Add table row
        /// </summary>
        /// <param name="item"></param>
        public void Update(object item)
        {
            var found = getRowFromItem(item);
            if (found != null)
            {
                // check not already marked
                var alreadyMarked = ItemsToUpdate.FirstOrDefault(x => x == item) != null;
                if (alreadyMarked) throw new Exception("Object already marked for update");

                // ok
                found.Updated = true;
                //found.RawItem = item;
                //found.SetRawItemPrimaryKey(found.PrimaryKey, PrimaryKeyPropertyInfo);
                ItemsToUpdate.Add(found);
            }
            else
            {
                throw new Exception("Cannot find object in collection when calling DedStoreTable<T>.Update");
            }
        }

        /// <summary>
        /// Add table row
        /// </summary>
        /// <param name="item"></param>
        public void Remove(object item)
        {
            var found = getRowFromItem(item);
            if (found != null)
            {
                // check not already marked
                var alreadyMarked = ItemsToRemove.FirstOrDefault(x => x == item) != null;
                if (alreadyMarked) throw new Exception("Object already marked for removal");

                // ok
                ItemsToRemove.Add(found);
                TableRows.Remove(found);
            }
            else
            {
                throw new Exception("Cannot find object in collection when calling DedStoreTable<T>.Remove");
            }
        }

        /// <summary>
        /// Helper
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DedsStoreTableRow getRowFromItem(object item)
        {
            var primaryKeyValue = PrimaryKeyPropertyInfo.GetValue(item);
            var output =  TableRows.FirstOrDefault(x => x.PrimaryKey.Equals(primaryKeyValue));
            return output;
        }
    }
}
