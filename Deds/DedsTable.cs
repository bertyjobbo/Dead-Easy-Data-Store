using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsTable
    {
    }

    /// <summary>
    /// Deds table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DedsTable<T> : DedsTable, IEnumerable<T>
    {
        #region INNER

        // internal
        internal DedsTableRowCollection<T> InnerList { get; set; }
        internal IEnumerable<T> InnerEnumerable { get { return InnerList.List.Select(x => x.Value); } }

        #endregion

        #region CRUD

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="item"></param>
        public DedsResponse<T> Create(T item)
        {
            // start
            var resp = new DedsResponse<T>
            {
                ResponseType = DedsResponseType.Create
            };

            try
            {

                // pk val
                object pkVal;

                // find primary key
                if (InnerList.TypeOfPrimaryKey == typeof (int))
                {
                    var max = InnerList.List.Count == 0 ? 0: InnerList.List.Max(x => ((int) x.PrimaryKeyValue));
                    pkVal = max + 1;
                    InnerList.PrimaryKeyPropertyInfo.SetValue(item, pkVal);
                    
                }
                else
                {
                    pkVal = InnerList.PrimaryKeyPropertyInfo.GetValue(item);
                    if (pkVal == null)
                    {
                        throw new Exception("Set primary key value, string or Guid");
                    }
                }

                // add
                InnerList.List.Add(new DedsTableRow<T>
                {
                    PrimaryKeyValue = pkVal,
                    Value = item
                });
            }
            catch (Exception ex)
            {
                resp.ErrorMessage = ex.Message +
                                    (ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message);
            }

            resp.ResponseItem = item;
            return resp;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="item"></param>
        public DedsResponse<T> Update(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="item"></param>
        public DedsResponse<T> Delete(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="primaryKeyValue"></param>
        public DedsResponse Delete(object primaryKeyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        public T Find(object primaryKeyValue)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region other

        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get
            {
                return InnerList.List.Count;
            }
        }


        #endregion

        #region enumerator

        public IEnumerator<T> GetEnumerator()
        {
            return InnerEnumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
