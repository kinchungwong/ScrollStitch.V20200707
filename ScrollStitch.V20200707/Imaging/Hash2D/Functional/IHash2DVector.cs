using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D.Functional
{
    /// <summary>
    /// Interface definition for a scalar hashing functor.
    /// </summary>
    public interface IHash2DVector
    {
        /// <summary>
        /// The static constant vector length. 
        /// 
        /// <para>
        /// Implementation Requirement. <br/>
        /// For any given concrete implementation (type) of IHash2DVector, the <c>Length</c> property must return
        /// the same value.
        /// </para>
        /// </summary>
        /// 
        int Length { get; }

        /// <summary>
        /// Initializes the internal states with the given seed.
        /// 
        /// 
        /// <para>
        /// Implementation Requirement. <br/>
        /// This function must restore the object to a state that is equivalent to that of a newly constructed instance. <br/>
        /// In other words, the instance must be reusable after a call to <c>Init(seed)</c>.
        /// </para>
        /// 
        /// <para>
        /// Implementation Reminder. <br/>
        /// Details about internal states initialization depend on the concrete implementation. It is not required 
        /// to contain a literal copy of the seed value.
        /// </para>
        /// </summary>
        /// 
        /// <param name="seed"></param>
        /// 
        void Init(uint seed);

        /// <summary>
        /// Performs the first step of processing a new input value. 
        /// 
        /// <para>
        /// Given a unit of input data, this function returns its transformation.
        /// </para>
        /// 
        /// <para>
        /// Implementation Requirement. <br/>
        /// This method is required to be practically static, in the sense that, for any chosen implementation: <br/>
        /// 1. This function must always return the same result whenever given the same input; <br/>
        /// 2. The returned result must not depend on the internal states of the hashing functor; <br/>
        /// 3. This function must not alter the internal states of the hashing functor. <br/>
        /// </para>
        /// 
        /// </summary>
        /// 
        /// <param name="inputTransformBuffer">
        /// A buffer containing a vector of input or transformed data.<br/>
        /// Caller should fill this buffer with input data.<br/>
        /// Upon finishing, the buffer will be filled with the transformed data by this function.
        /// </param>
        /// 
        /// <returns>
        /// The transformation of the given unit of input data.
        /// </returns>
        /// 
        void TransformData(uint[] inputTransformBuffer);

        /// <summary>
        /// Performs the second step of processing a new input value. 
        /// 
        /// <para>
        /// Given the output from <see cref="TransformData"/>, this method updates the internal states 
        /// of the hashing functor.
        /// </para>
        /// </summary>
        /// 
        /// <param name="transformedBuffer">
        /// The output from <see cref="TransformData(uint[])"/>.
        /// </param>
        ///
        void UpdateState(uint[] transformedBuffer);

        /// <summary>
        /// Computes the hashing functor's output and writes the output to the buffer.
        /// 
        /// <para>
        /// The hashing functor's output is computed using its internal states, which is indirectly 
        /// influenced by the seed and all units of input data it has ever processed.
        /// </para>
        /// 
        /// <para>
        /// Implementation Requirement. <br/>
        /// 1. This function must not alter the internal states of the hashing functor. <br/>
        /// </para>
        /// </summary>
        /// 
        /// <param name="resultBuffer">
        /// A caller-provided buffer where the output will be written to.
        /// </param>
        /// 
        void GetResult(uint[] resultBuffer);
    }
}
