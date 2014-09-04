using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace DedStore
{
    public class DedStoreTable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DedStoreTable(DedStoreContext context, Type type)
        {
            Context = context;
            Type = type;
        }

        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Inner collection
        /// </summary>
        internal DedStoreTableRowCollection InnerCollection
        {
            get { return Context.CollectionsInContext[Type]; }
        }

        /// <summary>
        /// Context
        /// </summary>
        internal DedStoreContext Context { get; private set; }
    }

    /// <summary>
    /// Ded store table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DedStoreTable<T> : DedStoreTable, IEnumerable<T>
    {
        /// <summary>
        /// Ded store table
        /// </summary>
        /// <param name="context"></param>
        public DedStoreTable(DedStoreContext context)
            : base(context, typeof(T))
        {
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return InnerCollection.TableRows.Select(x => (T)x.RawItem).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Find
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public T Find(object primaryKey)
        {
            // check type
            var type = primaryKey.GetType();
            if (type != InnerCollection.TypeOfPrimaryKey) throw new Exception("Type " + InnerCollection.Type.FullName + " has a primary key of type " + InnerCollection.TypeOfPrimaryKey.FullName + ", not " + type.FullName);

            // go
            var output = InnerCollection.TableRows.FirstOrDefault(x => x.PrimaryKey.Equals(primaryKey));

            // null
            if (output != null) return (T)output.RawItem;

            // ok
            return default(T);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public DedStoreResponse<T> Add(T item)
        {
            var output = new DedStoreResponse<T> { ResponseType = DedStoreResponseType.Add, ResponseItem = item };
            try
            {
                InnerCollection.Add(item);
            }
            catch (Exception ex)
            {
                output.ErrorMessage = ex.Message +
                                      (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }
            return output;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="item"></param>
        public DedStoreResponse<T> Update(T item)
        {
            var output = new DedStoreResponse<T> { ResponseType = DedStoreResponseType.Update, ResponseItem = item };
            try
            {
                InnerCollection.Update(item);
            }
            catch (Exception ex)
            {
                output.ErrorMessage = ex.Message +
                                      (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }
            return output;
        }

        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public DedStoreResponse Remove(T item)
        {
            var output = new DedStoreResponse { ResponseType = DedStoreResponseType.Remove };
            try
            {
                InnerCollection.Remove(item);
            }
            catch (Exception ex)
            {
                output.ErrorMessage = ex.Message +
                                      (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }
            return output;
        }

        /// <summary>
        /// Items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public DedStoreResponse AddMany(IEnumerable<T> items)
        {
            var output = new DedStoreResponse { ResponseType = DedStoreResponseType.Add };
            try
            {
                foreach (var item in items)
                {
                    InnerCollection.Add(item);
                }
            }
            catch (Exception ex)
            {
                output.ErrorMessage = ex.Message +
                                      (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }
            return output;
        }

        /// <summary>
        /// Add many
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public DedStoreResponse UpdateMany(IEnumerable<T> items)
        {
            var output = new DedStoreResponse { ResponseType = DedStoreResponseType.Update };
            try
            {
                foreach (var item in items)
                {
                    InnerCollection.Update(item);
                }
            }
            catch (Exception ex)
            {
                output.ErrorMessage = ex.Message +
                                      (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }
            return output;
        }

        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public DedStoreResponse RemoveMany(IEnumerable<T> items)
        {
            var output = new DedStoreResponse { ResponseType = DedStoreResponseType.Remove };
            try
            {
                foreach (var item in items.ToList())
                {
                    InnerCollection.Remove(item);
                }
            }
            catch (Exception ex)
            {
                output.ErrorMessage = ex.Message +
                                      (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }
            return output;
        }

        /// <summary>
        /// Clear
        /// </summary>
        /// <returns></returns>
        public DedStoreResponse Clear()
        {
            var output = new DedStoreResponse { ResponseType = DedStoreResponseType.Remove };
            try
            {
                RemoveMany(this);
            }
            catch (Exception ex)
            {
                output.ErrorMessage = ex.Message +
                                      (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }
            return output;
        }
    }
}
