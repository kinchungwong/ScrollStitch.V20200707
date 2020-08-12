using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Data;

    /// <summary>
    /// A collection of user-defined records that are indexed by a rectangle member.
    /// 
    /// <para>
    /// Implementation interoperability requirement. Unless otherwise permitted,
    /// <br/>
    /// implementations of this interface shall require that all item rectangles and 
    /// search rectangles have positve width and height. 
    /// <br/>
    /// Moreover, two rectangles intersect only if their area of overlap has positive 
    /// width and height.
    /// <br/>
    /// Interface clients requiring non-compliant behaviors can usually get around these
    /// requirements by using modified rectangles.
    /// </para>
    /// </summary>
    /// <typeparam name="RecordType"></typeparam>
    public interface IRectCollectionEx<RecordType>
        : IEnumerable<RecordType>
    {
        /// <summary>
        /// Returns the <see cref="Type"/> of the <see cref="RecordType"/> generic parameter 
        /// of this instance.
        /// </summary>
        /// <returns></returns>
        Type GetRecordType();

        /// <summary>
        /// Configures a function that extracts the <see cref="Rect"/> from the <see cref="RecordType"/>.
        /// </summary>
        Func<RecordType, Rect> RecordToRectFunc { get; set; }

        /// <summary>
        /// Configures a function that tests two instances of <see cref="RecordType"/> for equality.
        /// 
        /// Normally, implementations will use <see cref="EqualityComparer{RecordType}.Default"/> unless 
        /// configured otherwise.
        /// </summary>
        IEqualityComparer<RecordType> RecordEqualityFunc { get; set; }

        /// <summary>
        /// Checks whether a record compares identical to an item in this collection.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        bool Contains(RecordType record);

        /// <summary>
        /// Adds one item.
        /// </summary>
        /// <param name="record"></param>
        void Add(RecordType record);

        /// <summary>
        /// Adds multiple items.
        /// </summary>
        /// <param name="records"></param>
        void AddRange(IEnumerable<RecordType> records);

        /// <summary>
        /// Removes an item that compares identically to the specified record.
        /// </summary>
        /// <param name="record"></param>
        void Remove(RecordType record);

        /// <summary>
        /// For each specified item, try to remove an item from the collection that compares identically.
        /// </summary>
        /// <param name="records"></param>
        void RemoveRange(IEnumerable<RecordType> records);

        /// <summary>
        /// Passes each item in the collection into the specified function.
        /// </summary>
        /// <param name="func"></param>
        void ForEach(Action<RecordType> action);

        /// <summary>
        /// Passes each item in the collection into the specified function, if the item's rectangle
        /// has a positive-area overlap with the search rectangle.
        /// </summary>
        /// <param name="searchRect">The search rectangle.</param>
        /// <param name="func"></param>
        void ForEach(Rect searchRect, Action<RecordType> action);

        /// <summary>
        /// Returns an array containing a copy of all items.
        /// </summary>
        /// <returns></returns>
        RecordType[] ToArray();

        /// <summary>
        /// Returns an array containing a copy of all items that have a positive-area overlap
        /// with the search rectangle.
        /// </summary>
        /// <returns></returns>
        RecordType[] ToArray(Rect searchRect);

        /// <summary>
        /// Enumerates all items.
        /// 
        /// <para>
        /// The enumerator is invalidated if the collection is modified.
        /// </para>
        /// </summary>
        /// <param name="searchRect"></param>
        /// <returns></returns>
        IEnumerable<RecordType> Enumerate();

        /// <summary>
        /// Enumerates all items that have a positive-area overlap with the search rectangle.
        /// 
        /// <para>
        /// The enumerator is invalidated if the collection is modified.
        /// </para>
        /// </summary>
        /// <param name="searchRect"></param>
        /// <returns></returns>
        IEnumerable<RecordType> Enumerate(Rect searchRect);

        /// <summary>
        /// Resets the collection by removing all items.
        /// 
        /// <para>
        /// These properties are not changed by this method:<br/>
        /// <see cref="RecordToRectFunc"/><br/>
        /// <see cref="RecordEqualityFunc"/><br/>
        /// </para>
        /// </summary>
        void Clear();
    }
}
