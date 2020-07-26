using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D.Vectorized
{
    /// <summary>
    /// Processes a horizontal row for Hash2D.
    /// </summary>
    public class Hash2DSingleRow
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Process(ArraySegment<int> source, ArraySegment<int> dest)
        {
            int[] sourceArray = source.Array;
            int[] destArray = dest.Array;
            if (sourceArray is null)
            {
                throw new ArgumentNullException(nameof(source) + ".Array");
            }
            if (destArray is null)
            {
                throw new ArgumentNullException(nameof(dest) + ".Array");
            }
            int totalLength = source.Count;
            if (dest.Count != totalLength)
            {
                throw new ArgumentException(nameof(dest) + ".Count");
            }
            int sourceOffset = source.Offset;
            int destOffset = dest.Offset;
            Hash2DVector state = Hash2DVector.Create();
            uint[] data = Hash2DVectorFactory.Create();
            int fixedLength = state.Length;
            int currentOffset = 0;
            while (currentOffset + fixedLength <= totalLength)
            {
                // ====== TODO ====== MOCK CODE ======
                // The following code does not compute a valid hashing result.
                // It is used during development only.
                // ======
                state.Init(0u);
                var sourceSegment = new ArraySegment<int>(sourceArray, sourceOffset + currentOffset, fixedLength);
                var destSegment = new ArraySegment<int>(destArray, destOffset + currentOffset, fixedLength);
                Hash2DVectorFactory.CopyTo(sourceSegment, data);
                state.TransformData(data);
                state.UpdateState(data);
#if true
                state.UpdateState(data);
                state.UpdateState(data);
                state.UpdateState(data);
                state.UpdateState(data);
#endif
                state.GetResult(data);
                Hash2DVectorFactory.CopyTo(data, destSegment);
                currentOffset += fixedLength;
            }
            // ====== TODO ======
            // Handle end-of-array tail.
            // ======
        }
    }
}
