using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D.Vectorized
{
    using Bitwise;

    /// <summary>
    /// Implementation of <see cref="IHash2DVector"/>.
    /// 
    /// <inheritdoc cref="IHash2DVector"/>
    /// </summary>
    /// 
    /// <inheritdoc cref="IHash2DVector"/>
    /// 
    public struct Hash2DVector
        : IHash2DVector
    {
        #region private hash function constants
        private const uint _c1 = 0xcc9e2d51u;
        private const uint _c2 = 0x1b873593u;
        private const uint _c3 = 0xe6546b64u;
        private const uint _c8 = 0x85ebca6bu;
        private const uint _c10 = 0xc2b2ae35u;
        #endregion

        public int Length => Hash2DVectorFactory.Length;

        #region private hash internal states
        private readonly uint[] _states;
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Hash2DVector Create()
        {
            return new Hash2DVector(Hash2DVectorFactory.Create());
        }

        public void Init(uint seed) => NoInline.Init(_states, seed);

        public void TransformData(uint[] inputTransformBuffer) => NoInline.TransformData(inputTransformBuffer);

        public void UpdateState(uint[] transformedBuffer) => NoInline.UpdateState(_states, transformedBuffer);

        public void GetResult(uint[] resultBuffer) => NoInline.GetResult(_states, resultBuffer);

        #region private ctor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Hash2DVector(uint[] states)
        {
            Hash2DVectorFactory.ValidateElseThrow(states);
            _states = states;
        }
        #endregion

        /// <summary>
        /// The actual function implementations in the form of static functions.
        /// </summary>
        public static class Inline
        {
            /// <inheritdoc cref="IHash2DVector.Init(uint)"/>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Init(uint[] states, uint seed)
            {
                Hash2DVectorFactory.Fill(states, seed);
            }

            /// <inheritdoc cref="IHash2DVector.TransformData(uint[])"/>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void TransformData(uint[] inputTransformBuffer)
            {
                Hash2DVectorFactory.ValidateElseThrow(inputTransformBuffer);
                for (int k = 0; k < Hash2DVectorFactory.Length; ++k)
                {
                    uint data = inputTransformBuffer[k];
                    uint data2, data3, data4;
                    unchecked
                    {
                        data2 = data * _c1;
                        data3 = BitwiseUtility.Rotate(data2, 15);
                        data4 = data3 * _c2;
                    }
                    inputTransformBuffer[k] = data4;
                }
            }

            /// <inheritdoc cref="IHash2DVector.UpdateState(uint[])"/>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void UpdateState(uint[] states, uint[] transformedBuffer)
            {
                Hash2DVectorFactory.ValidateElseThrow(states);
                Hash2DVectorFactory.ValidateElseThrow(transformedBuffer);
                for (int k = 0; k < Hash2DVectorFactory.Length; ++k)
                {
                    uint stateValue = states[k];
                    uint transformedValue = transformedBuffer[k];
                    uint state5, state6, state7;
                    unchecked
                    {
                        state5 = stateValue ^ transformedValue;
                        state6 = BitwiseUtility.Rotate(state5, 13);
                        state7 = state6 * 5u + _c3;
                    }
                    states[k] = state7;
                }
            }

            /// <inheritdoc cref="IHash2DVector.GetResult(uint[])"/>
            /// 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void GetResult(uint[] states, uint[] resultBuffer)
            {
                Hash2DVectorFactory.ValidateElseThrow(states);
                Hash2DVectorFactory.ValidateElseThrow(resultBuffer);
                for (int k = 0; k < Hash2DVectorFactory.Length; ++k)
                {
                    uint stateValue = states[k];
                    uint endstate7, endstate8, endstate9, endstate10, endstate11;
                    unchecked
                    {
                        endstate7 = stateValue ^ (stateValue >> 16);
                        endstate8 = endstate7 * _c8;
                        endstate9 = endstate8 ^ (endstate8 >> 13);
                        endstate10 = endstate9 * _c10;
                        endstate11 = endstate10 ^ (endstate10 >> 16);
                    }
                    resultBuffer[k] = endstate11;
                }
            }
        }

        /// <summary>
        /// Provides the option of calling a function that is not inlined.
        /// 
        /// <para>
        /// Intended usage. <br/>
        /// This is for performance investigations only. <br/>
        /// This can be used for comparing the costs and benefits of inlining particular functions. <br/>
        /// This can also be used in interactive debugging, where breakpoints can be set inside the 
        /// non-inline function entry point, and also the disassembly is easier for human understanding
        /// due to clear delineation.
        /// </para>
        /// </summary>
        public static class NoInline
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void Init(uint[] states, uint seed)
            {
                Inline.Init(states, seed);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void TransformData(uint[] inputTransformBuffer)
            {
                Inline.TransformData(inputTransformBuffer);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void UpdateState(uint[] states, uint[] transformedBuffer)
            {
                Inline.UpdateState(states, transformedBuffer);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void GetResult(uint[] states, uint[] resultBuffer)
            {
                Inline.GetResult(states, resultBuffer);
            }
        }
    }
}
