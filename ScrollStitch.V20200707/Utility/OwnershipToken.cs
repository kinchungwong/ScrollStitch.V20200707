using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Utility
{
    /// <summary>
    /// <para>
    /// A token class that allows an owner class to distinguish additional data objects 
    /// that are created by the same owner.
    /// </para>
    /// 
    /// <para>
    /// 1. The owner creates an instance of <see cref="OwnershipToken{OwnerType}"/>
    /// and keep it private.<br/>
    /// 2. When the owner creates additional data objects, it passes the token instance
    /// to the constructor on the data object.<br/>
    /// 3. The data object also keeps the token instance private.<br/>
    /// 4. However, the data object exposes a function, <c>CompareOwnershipToken</c>,
    /// which allows the caller to check whether it has the same token instance as
    /// the one privately held by the data object.<br/>
    /// 5. When the owner needs to validate a data object, it can call the method
    /// on the data object to compare the data object's token with its own.
    /// </para>
    /// 
    /// <para>
    /// In addition to confirmation of same-ownership, instances of this token class 
    /// can also be used as a locking mechanism between multiple objects. However, 
    /// it is not robust against deadlocks and other locking pitfalls.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="OwnerType">
    /// </typeparam>
    /// 
    public sealed class OwnershipToken<OwnerType>
        : IEquatable<OwnershipToken<OwnerType>>
        where OwnerType: class
    {
        private static Lazy<string> _lzTokenTypeName = new Lazy<string>(
            () => "(OwnershipToken<" + typeof(OwnerType).Name + ">)");
       
        public OwnershipToken(OwnerType owner)
        {
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case OwnershipToken<OwnerType> other:
                    return Equals(this, other);
                default:
                    return false;
            }
        }

        public bool Equals(OwnershipToken<OwnerType> other)
        {
            return Equals(this, other);
        }

        public static bool Equals(OwnershipToken<OwnerType> a, OwnershipToken<OwnerType> b)
        {
            return ReferenceEquals(a, b);
        }

        public static bool operator ==(OwnershipToken<OwnerType> a, OwnershipToken<OwnerType> b)
        {
            return ReferenceEquals(a, b);
        }

        public static bool operator !=(OwnershipToken<OwnerType> a, OwnershipToken<OwnerType> b)
        {
            return !ReferenceEquals(a, b);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return _lzTokenTypeName.Value;
        }
    }
}
