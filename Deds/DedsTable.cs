using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deds
{
    /// <summary>
    /// Deds table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DedsTable<T> : IEnumerable<T>
    {
        // fields
        private readonly List<T> _innerList = new List<T>();
        private readonly Type _typeOfT = typeof(T);
        private readonly Type _typeOfPrimaryKeyAttr = typeof(DedsKeyAttribute);
        private readonly PropertyInfo _primaryKeyInfo;

        #region INIT

        public DedsTable()
        {
            // TODO CACHE THIS
            var props = _typeOfT.GetProperties();

            // attrs
            var attrs = props.Where(prop => prop.CustomAttributes.Any(a => a.AttributeType == _typeOfPrimaryKeyAttr)).ToList();

            // checks
            if (attrs.Count < 1) throw new Exception("Type " + _typeOfT.Name + " has no DedsKey attribute");
            if (attrs.Count > 1) throw new Exception("Type " + _typeOfT.Name + " has more than one DedsKey attribute");

            // all good
            _primaryKeyInfo = attrs.First();
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="item"></param>
        public DedsResponse<T> Create(T item)
        {
            var lockObj = new object();
            lock (lockObj)
            {
                // NEED TO GET NEXT PRIMARY KEY HERE!!

                // DO YOU IMPLEMENT "FIND" FIRST

                // DOES THE INNER LIST HAVE A WRAPPER-TYPE WHICH HAS THE OBJECT ON ONE SIDE, AND SOME METADATA WITH IT?? (IE THE PRIMARY KEY)
                // -- this would be set when the table is read / an item is created
            }
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
        public DedsResponse<T> Delete(object primaryKeyValue)
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

        /// <summary>
        /// Save changes
        /// </summary>
        public void SaveChanges()
        {
        }

        #endregion


        #region other

        public int Count { get { return _innerList.Count; } }

        #endregion


        #region enumerator

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator()
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
