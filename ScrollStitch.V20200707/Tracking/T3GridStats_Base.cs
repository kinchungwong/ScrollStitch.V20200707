using ScrollStitch.V20200707.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Collections.Specialized;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;

    /// <summary>
    /// The non-generic base class for grid-based statistics collectors for three-image trajectory algorithms.
    /// 
    /// <para>
    /// It is usually more appropriate to use the generic base class, because it provides the prototype 
    /// <see cref="T3GridStats_Base{ResultType}.GetResult"/> method.
    /// </para>
    /// </summary>
    public abstract class T3GridStats_Base
    {
        public T3GridStats Host { get; }

        public int ImageKey => Host.ImageKey;

        public Grid Grid => Host.Grid;

        public IReadOnlyList<Point> Points => Host.Points;

        public IReadOnlyList<int> HashValues => Host.HashValues;

        public IReadOnlyList<int> Labels => Host.Labels;

        protected T3GridStats_Base(T3GridStats host)
        {
            Host = host;
        }

        /// <summary>
        /// Accumulates the given sample into the statistics which the concrete implementer 
        /// is tasked with collecting.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="hashValue"></param>
        /// <param name="label"></param>
        /// <param name="cellIndex"></param>
        public abstract void Add(Point point, int hashValue, int label, CellIndex cellIndex);

        public abstract object GetResultAsObject();
    }

    /// <summary>
    /// The generic base class for grid-based statistics collectors for three-image trajectory algorithms.
    /// </summary>
    /// 
    /// <typeparam name="ResultType">
    /// The return type containing the statistics that is computed by a concrete implementation.
    /// </typeparam>
    /// 
    public abstract class T3GridStats_Base<ResultType>
        : T3GridStats_Base
    {
        protected T3GridStats_Base(T3GridStats host)
            : base(host)
        {
        }

        public abstract ResultType GetResult();
    }
}
