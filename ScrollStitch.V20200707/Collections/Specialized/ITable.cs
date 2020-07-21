using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections.Specialized
{
    /// <summary>
    /// Represents a table with a key associated with a row of data of the same type.
    /// 
    /// <para>
    /// Classes that implement this interface may optionally implement additional behavior.
    /// </para>
    /// 
    /// <para>
    /// See also:
    /// <br/>
    /// <seealso cref="IntKeyIntRow"/>
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="KeyType">
    /// The key type.
    /// </typeparam>
    /// 
    /// <typeparam name="ItemType">
    /// The type of items stored in the row.
    /// </typeparam>
    ///
    public interface ITable<KeyType, ItemType>
    {
        /// <summary>
        /// The number of columns on this table.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Checks whether the table contain the specified row key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(KeyType key);

        /// <summary>
        /// Returns a snapshot of all row keys contained in the table.
        /// 
        /// <para>
        /// In typical implementations, the row keys are returned in unspecific ordering,
        /// due to internal use of <see cref="Dictionary{TKey, TValue}"/> or similar data 
        /// structures.
        /// </para>
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// An array containing a snapshot copy of existing row keys.
        /// </returns>
        /// 
        KeyType[] GetKeys();

        /// <summary>
        /// Reads or writes a single value on this table.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        ItemType this[KeyType key, int column] { get; set; }

        /// <summary>
        /// Reads or writes an entire row of values on this table.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ItemType[] this[KeyType key] { get; set; }

        void Clear();

        void Remove(KeyType key);
    }
}
