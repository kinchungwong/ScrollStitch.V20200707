using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting.Internals
{
    using ScrollStitch.V20200707.Functional;
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// This is an internal class that provides the algorithm implementation for the 
    /// <see cref="AnnulusSector"/> drawing command. 
    /// 
    /// <para>
    /// The mathematical operations have not yet been verified for correctness.
    /// </para>
    /// </summary>
    public class AnnulusSectorQuadrantLoop
    {
        public double RadiusBegin { get; }

        public double RadiusEnd { get; }

        public double DegreeBegin { get; }

        public double DegreeEnd { get; }

        private readonly bool _hasRadiusBegin;
        private readonly bool _hasDegreeCheck;
        private readonly int _radiusMax;
        private readonly int _radiusSqBegin;
        private readonly int _radiusSqEnd;
        private readonly int _mxBegin;
        private readonly int _myBegin;
        private readonly int _mxEnd;
        private readonly int _myEnd;

        public AnnulusSectorQuadrantLoop(double radius)
            : this(0.0, radius, 0.0, 90.0)
        {
        }

        public AnnulusSectorQuadrantLoop((double Begin, double End) radiusRange)
            : this(radiusRange.Begin, radiusRange.End, 0.0, 90.0)
        {
        }

        public AnnulusSectorQuadrantLoop((double Begin, double End) radiusRange, (double Begin, double End) degreeRange)
            : this(radiusRange.Begin, radiusRange.End, degreeRange.Begin, degreeRange.End)
        { 
        }

        public AnnulusSectorQuadrantLoop(double radiusBegin, double radiusEnd, double degreeBegin = 0.0, double degreeEnd = 90.0)
        {
            RadiusBegin = radiusBegin;
            RadiusEnd = radiusEnd;
            DegreeBegin = degreeBegin;
            DegreeEnd = degreeEnd;
            _radiusMax = (int)Math.Ceiling(radiusEnd + 1.0);
            _radiusSqBegin = (int)Math.Round(radiusBegin * radiusBegin);
            _radiusSqEnd = (int)Math.Round(radiusEnd * radiusEnd);
            _hasRadiusBegin = (_radiusSqBegin > 0);
            _hasDegreeCheck = (DegreeBegin > 0.0) || (DegreeEnd < 90.0);
            if (_hasDegreeCheck)
            {
                _DegreeToRatio(degreeBegin, out _mxBegin, out _myBegin);
                _DegreeToRatio(degreeEnd, out _mxEnd, out _myEnd);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke<FuncType>(FuncType func)
            where FuncType : struct, IFunc, IFunc<FuncType, int, int, int>
        {
            if (_hasDegreeCheck)
            {
                if (_hasRadiusBegin)
                {
                    InvokeWithRadiusAndDegree(func);
                }
                else
                {
                    InvokeWithDegree(func);
                }
            }
            else
            {
                if (_hasRadiusBegin)
                {
                    InvokeWithRadius(func);
                }
                else
                {
                    InvokeSector(func);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InvokeSector<FuncType>(FuncType func)
            where FuncType : struct, IFunc, IFunc<FuncType, int, int, int>
        {
            int y2 = 0;
            for (int y = 0; y < _radiusMax; ++y)
            {
                int cy2 = y2;
                y2 += 2 * y + 1;
                int x2 = 0;
                for (int x = 0; x < _radiusMax; ++x)
                {
                    int cx2 = x2;
                    int csumSq = cx2 + cy2;
                    x2 += 2 * x + 1;
                    if (csumSq > _radiusSqEnd)
                    {
                        break;
                    }
                    int _ = func.Invoke(x, y);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InvokeWithRadius<FuncType>(FuncType func)
            where FuncType : struct, IFunc, IFunc<FuncType, int, int, int>
        {
            int y2 = 0;
            for (int y = 0; y < _radiusMax; ++y)
            {
                int cy2 = y2;
                y2 += 2 * y + 1;
                int x2 = 0;
                for (int x = 0; x < _radiusMax; ++x)
                {
                    int cx2 = x2;
                    int csumSq = cx2 + cy2;
                    x2 += 2 * x + 1;
                    if (csumSq < _radiusSqBegin)
                    {
                        continue;
                    }
                    if (csumSq > _radiusSqEnd)
                    {
                        break;
                    }
                    int _ = func.Invoke(x, y);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InvokeWithDegree<FuncType>(FuncType func)
            where FuncType : struct, IFunc, IFunc<FuncType, int, int, int>
        {
            int y2 = 0;
            for (int y = 0; y < _radiusMax; ++y)
            {
                int cy2 = y2;
                y2 += 2 * y + 1;
                int x2 = 0;
                int testBegin = -(y * _myBegin);
                int testEnd = y * _myEnd;
                for (int x = 0; x < _radiusMax; ++x)
                {
                    int cx2 = x2;
                    int cbegin = testBegin;
                    int cend = testEnd;
                    int csumSq = cx2 + cy2;
                    x2 += 2 * x + 1;
                    testBegin += _mxBegin;
                    testEnd -= _mxEnd;
                    if (csumSq > _radiusSqEnd)
                    {
                        break;
                    }
                    if (cbegin > _mxBegin)
                    {
                        break;
                    }
                    if (cend > 0)
                    {
                        continue;
                    }
                    int _ = func.Invoke(x, y);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InvokeWithRadiusAndDegree<FuncType>(FuncType func)
            where FuncType : struct, IFunc, IFunc<FuncType, int, int, int>
        {
            int y2 = 0;
            for (int y = 0; y < _radiusMax; ++y)
            {
                int cy2 = y2;
                y2 += 2 * y + 1;
                int x2 = 0;
                int testBegin = -(y * _myBegin);
                int testEnd = y * _myEnd;
                for (int x = 0; x < _radiusMax; ++x)
                {
                    int cx2 = x2;
                    int cbegin = testBegin;
                    int cend = testEnd;
                    int csumSq = cx2 + cy2;
                    x2 += 2 * x + 1;
                    testBegin += _mxBegin;
                    testEnd -= _mxEnd;
                    if (csumSq < _radiusSqBegin)
                    {
                        continue;
                    }
                    if (csumSq > _radiusSqEnd)
                    {
                        break;
                    }
                    if (cbegin > _mxBegin)
                    {
                        break;
                    }
                    if (cend > 0)
                    {
                        continue;
                    }
                    int _ = func.Invoke(x, y);
                }
            }
        }

        private static void _DegreeToRatio(double degree, out int mx, out int my)
        {
            const double degreeToRadian = (Math.PI / 180.0);
            if (degree < 45.0)
            {
                double tang = Math.Tan(degree * degreeToRadian);
                mx = (int)Math.Round(256 * tang);
                my = 256;
            }
            else if (degree > 45.0)
            {
                double tang = Math.Tan((90.0 - degree) * degreeToRadian);
                mx = 256;
                my = (int)Math.Round(256 * tang);
            }
            else
            {
                mx = 256;
                my = 256;
            }
        }
    }
}
