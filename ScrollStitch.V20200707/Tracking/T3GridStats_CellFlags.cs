using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;
    using BitFlagArith;
    using System.CodeDom;

    /// <summary>
    /// <inheritdoc cref="T3GridStats_CellFlags{FlagType, FlagArith}"/>
    /// </summary>
    /// 
    public static class T3GridStats_CellFlags
    {
        /// <summary>
        /// Creates an instance of <see cref="T3GridStats_CellFlags{FlagType, FlagArith}"/> using 
        /// <see cref="FlagType"/> as bit flag.
        /// 
        /// <para>
        /// <see cref="FlagType"/> is an integer type used as bit flag. Allowed integer types are: <br/>
        /// <see cref="int"/>, for up to 31 trajectory labels, <br/>
        /// <see cref="uint"/>, for up to 32 trajectory labels, <br/>
        /// <see cref="ulong"/>, for up to 64 trajectory labels.
        /// </para>
        /// </summary>
        /// 
        /// <typeparam name="FlagType">
        /// An integer type used as bit flag.
        /// </typeparam>
        /// 
        public static T3GridStats_Base<GridArray<FlagType>> Create<FlagType>(T3GridStats host)
            where FlagType : struct, IConvertible
        {
            T3GridStats_Base<GridArray<FlagType>> AsReturnType<ArgType>(ArgType v)
            {
                return v as T3GridStats_Base<GridArray<FlagType>>;
            }
            switch (default(FlagType))
            {
                case int _:
                    return AsReturnType(Create_Int32(host));
                case uint _:
                    return AsReturnType(Create_UInt32(host));
                case ulong _:
                    return AsReturnType(Create_UInt64(host));
                default:
                    throw new NotImplementedException(
                        message: $"An implementation of {nameof(T3GridStats_CellFlags)} for type {typeof(FlagType).Name} has not been provided.");
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="T3GridStats_CellFlags{FlagType, FlagArith}"/> using 
        /// <see cref="int"/> as bit flag, capable of recording the first 31 trajectory labels.
        /// </summary>
        public static T3GridStats_Base<GridArray<int>> Create_Int32(T3GridStats host)
        {
            return new T3GridStats_CellFlags<int, BitFlagArith_Int32>(host);
        }

        /// <summary>
        /// Creates an instance of <see cref="T3GridStats_CellFlags{FlagType, FlagArith}"/> using 
        /// <see cref="uint"/> as bit flag, capable of recording the first 32 trajectory labels.
        /// </summary>
        public static T3GridStats_Base<GridArray<uint>> Create_UInt32(T3GridStats host)
        {
            return new T3GridStats_CellFlags<uint, BitFlagArith_UInt32>(host);
        }

        /// <summary>
        /// Creates an instance of <see cref="T3GridStats_CellFlags{FlagType, FlagArith}"/> using 
        /// <see cref="ulong"/> as bit flag, capable of recording the first 64 trajectory labels.
        /// </summary>
        public static T3GridStats_Base<GridArray<ulong>> Create_UInt64(T3GridStats host)
        {
            return new T3GridStats_CellFlags<ulong, BitFlagArith_UInt64>(host);
        }
    }

    /// <summary>
    /// <para>
    /// This is an intermediate abstract class where <see cref="FlagType"/> is specified but
    /// <see cref="FlagArith"/> is not specified.
    /// <br/>
    /// For the concrete implementations, refer to <see cref="T3GridStats_CellFlags{FlagType, FlagArith}"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="FlagType">
    /// An integer type used as bit flag. Allowed integer types are: <br/>
    /// <see cref="int"/>, for up to 31 trajectory labels, <br/>
    /// <see cref="uint"/>, for up to 32 trajectory labels, <br/>
    /// <see cref="ulong"/>, for up to 64 trajectory labels.
    /// </typeparam>
    /// 
    public abstract class T3GridStats_CellFlags<FlagType>
        : T3GridStats_Base<GridArray<FlagType>>
        where FlagType : struct
    {
        protected T3GridStats_CellFlags(T3GridStats host)
            : base(host)
        {
        }
    }

    /// <summary>
    /// Computes a <see cref="GridArray{FlagType}"/> containing bit flags which indicate the presence of samples 
    /// which confirm a particular set of trajectories inside each cell rectangle.
    /// 
    /// <para>
    /// Inside the <see cref="GridArray{FlagType}"/>, each cell contains an integer value used as a bit flag.
    /// <br/>
    /// Each bit corresponds to one trajectory label.
    /// <br/>
    /// The <c>n-th</c> bit indicates whether the trajectory label <c>n</c> is present among the samples 
    /// found inside the cell rectangle.
    /// <br/>
    /// If there are more distinct trajectory labels than the number of usable bits in the integer type,
    /// only the first <c>n</c> trajectory labels will be captured, depending  on the integer type.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="FlagType">
    /// An integer type used as bit flag. Allowed integer types are: <br/>
    /// <see cref="int"/>, for up to 31 trajectory labels, <br/>
    /// <see cref="uint"/>, for up to 32 trajectory labels, <br/>
    /// <see cref="ulong"/>, for up to 64 trajectory labels.
    /// </typeparam>
    /// 
    /// <typeparam name="FlagArith">
    /// A stateless struct that provides an implementation of some bitwise operations for <see cref="FlagType"/> 
    /// required by this class.
    /// </typeparam>
    /// 
    public sealed class T3GridStats_CellFlags<FlagType, FlagArith>
        : T3GridStats_CellFlags<FlagType>
        where FlagType : struct
        where FlagArith : struct, IBitFlagArith<FlagType>
    {
        private FlagArith _arith;
        private GridArray<FlagType> _array;

        public T3GridStats_CellFlags(T3GridStats host)
            : base(host)
        {
            _arith = default;
            _array = new GridArray<FlagType>(host.Grid);
        }

        public override void Add(Point point, int hashValue, int label, CellIndex cellIndex)
        {
            if (label < 0 || label >= _arith.NumUsableBits)
            {
                return;
            }
            int bitPos = label;
            FlagType oldValue = _array[cellIndex];
            FlagType updateValue = _arith.GetBitPositionMask(bitPos);
            FlagType newValue = _arith.Or(oldValue, updateValue);
            _array[cellIndex] = newValue;
        }

        public override GridArray<FlagType> GetResult()
        {
            return _array;
        }

        public override object GetResultAsObject() => GetResult();
    }
}
