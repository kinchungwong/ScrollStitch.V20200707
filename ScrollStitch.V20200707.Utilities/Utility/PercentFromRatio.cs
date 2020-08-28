using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Utility
{
    public struct PercentFromRatio
    {
        public double Numerator { get; }

        public double Denominator { get; }

        public double Ratio => Numerator / Denominator;

        public double Percent => Ratio * 100.0;

        public PercentFromRatio(double numerator, double denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public void Deconstruct(out double numerator, out double denominator)
        {
            numerator = Numerator;
            denominator = Denominator;
        }

        public void Deconstruct(out double numerator, out double denominator, out double ratio)
        {
            numerator = Numerator;
            denominator = Denominator;
            ratio = Ratio;
        }

        public void Deconstruct(out double numerator, out double denominator, out double ratio, out double percent)
        {
            numerator = Numerator;
            denominator = Denominator;
            ratio = Ratio;
            percent = Percent;
        }

        public override string ToString()
        {
            double roundNume = Math.Round(Numerator);
            double roundDenom = Math.Round(Denominator);
            string numeStr;
            if (Numerator == roundNume)
            {
                numeStr = ((int)roundNume).ToString();
            }
            else
            {
                numeStr = Numerator.ToString("G");
            }
            string denomStr;
            if (Denominator == roundDenom)
            {
                denomStr = ((int)roundDenom).ToString();
            }
            else
            {
                denomStr = Denominator.ToString("G");
            }
            double absPercent = Math.Abs(Percent);
            string percStr;
            if (absPercent > 10.0)
            {
                percStr = Ratio.ToString("P0");
            }
            else if (absPercent > 1.0)
            {
                percStr = Ratio.ToString("P1");
            }
            else if (absPercent > 0.1)
            {
                percStr = Ratio.ToString("P2");
            }
            else if (absPercent > 0.01)
            {
                percStr = Ratio.ToString("P3");
            }
            else if (absPercent > 0.001)
            {
                percStr = Ratio.ToString("P4");
            }
            else if (absPercent > 0.0001)
            {
                percStr = Ratio.ToString("P5");
            }
            else 
            {
                percStr = Ratio.ToString("P6");
            }
            return $"({numeStr}/{denomStr}, {percStr})";
        }
    }
}
