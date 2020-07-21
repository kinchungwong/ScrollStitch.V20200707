using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Caching
{
    /// <summary>
    /// An interface for retrieval access from a collection of items with integer-valued key.
    /// 
    /// <para>
    /// Implementations of this interface are allowed to implement additional behaviors.
    /// <br/>
    /// These additional behaviors are optional. 
    /// <br/>
    /// These additional behaviors are not guaranteed across all implementations.
    /// </para>
    /// 
    /// <para>
    /// Examples of additional behaviors:
    /// <br/>
    /// Automatic item creation on first retrieval
    /// <br/>
    /// Caching and item lifetime management
    /// <br/>
    /// Automatic or scheduled item removal
    /// <br/>
    /// Handling for removed items, such as disposal or reuse
    /// </para>
    /// 
    /// <para>
    /// The multi-threading behaviors are implementation-specific.
    /// </para>
    /// 
    /// <para>
    /// All integer values are allowed as key. In particular, negative and non-sequential keys are 
    /// allowed. For this reason, this class does not implement any of the standard collections 
    /// interface.
    /// </para>
    /// 
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of items that are made accessible through this collection interface.
    /// </typeparam>
    /// 
    public interface IItemSource<T>
    {
        T this[int key] { get; }
    }
}
