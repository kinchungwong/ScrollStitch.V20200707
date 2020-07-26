using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D.Vectorized
{
    /// <summary>
    /// Interface definition for a scalar hashing functor.
    /// </summary>
    public interface IHash2DScalar
    {
        /// <summary>
        /// Initializes the internal states with the given seed.
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
        /// <param name="inputData">
        /// One unit of input data.
        /// </param>
        /// 
        /// <returns>
        /// The transformation of the given unit of input data.
        /// </returns>
        /// 
        uint TransformData(uint inputData);

        /// <summary>
        /// Performs the second step of processing a new input value. 
        /// 
        /// <para>
        /// Given the output from <see cref="TransformData(uint)"/>, this method updates the internal states 
        /// of the hashing functor.
        /// </para>
        /// </summary>
        /// 
        /// <param name="transformedData">
        /// The output from <see cref="TransformData(uint)"/>.
        /// </param>
        ///
        void UpdateState(uint transformedData);

        /// <summary>
        /// Computes the hashing functor's output using its internal states.
        /// 
        /// <para>
        /// Implementation Requirement. <br/>
        /// 1. This function must not alter the internal states of the hashing functor. <br/>
        /// </para>
        /// </summary>
        /// 
        /// <returns>
        /// The output of the hashing functor, computed using its internal states, the latter of which 
        /// is indirectly influenced by the seed and all units of input data it has ever processed.
        /// </returns>
        /// 
        uint GetResult();
    }
}
