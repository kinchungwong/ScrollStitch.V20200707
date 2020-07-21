using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections.Specialized
{
    using ScrollStitch.V20200707.Collections.Internals;

    public static class HistogramUtility
    {
        public static Histogram<KeyType, int> CreateIntHistogram<KeyType>()
        {
            int addFunc(int a, int b) => a + b;
            HistArith<int> arith = new HistArith<int>()
            {
                Zero = 0,
                One = 1,
                AddFunc = addFunc
            };
            return new Histogram<KeyType, int>(arith);
        }

        public static Histogram<KeyType, double> CreateFloatHistogram<KeyType>()
        {
            double addFunc(double a, double b) => a + b;
            HistArith<double> arith = new HistArith<double>()
            {
                Zero = 0.0,
                One = 1.0,
                AddFunc = addFunc
            };
            return new Histogram<KeyType, double>(arith);
        }
    }
}
