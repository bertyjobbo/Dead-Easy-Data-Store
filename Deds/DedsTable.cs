using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    public class DedsTable: IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public DedsTableRowCollection InnerList { get; set; }
    }

    /// <summary>
    /// Deds table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DedsTable<T> : DedsTable, IEnumerable<T>
    {
        private DedsTableRowCollection<T> _innerList;

        #region INNER

        // internal
        internal new DedsTableRowCollection<T> InnerList
        {
            get { return _innerList; }
            set
            {
                base.InnerList = value;
                _innerList = value;
            }
        }

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
                // add
                InnerList.List.Add(new DedsTableRow<T>
                {
                    Value = item,
                    Added = true
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
            var found =  InnerList.List.FirstOrDefault(x => x.PrimaryKeyValue.Equals(primaryKeyValue));
            if (found == null) return default (T);
            return found.Value;
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
