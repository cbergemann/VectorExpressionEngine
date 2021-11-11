using VectorExpressionEngine;
using System;
using System.Linq;
using SimpleCalculator.Models;

namespace SimpleCalculator.Business
{
    public class MathLibrary : ExtendedMath
    {
        public ReflectionContext Context { get; set; }

        [Expression("plot")]
        public static LineSeries plot(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new SyntaxException("x and y vectors have to be of same length");
            }

            return new LineSeries
            {
                X = x,
                Y = y,
            };
        }

        [Expression("plot")]
        public static LineSeries plot(double[] x)
        {
            return new LineSeries
            {
                X = x,
                Y = Enumerable.Range(0, x.Length).Select(it => (double)it).ToArray(),
            };
        }

        [Expression("linspace", isConstant: true)]
        public static double[] linspace(double start, double end, double count)
        {
            if (count <= 0)
            {
                return Array.Empty<double>();
            }

            return Enumerable.Range(0, (int)(count - 1)).Select(it => start + (end - start) / (count - 1) * it).Append(end).ToArray();
        }

        [Expression("element", isConstant: true)]
        public double element(double[] vector, double no)
        {
            int noInt = (int)Math.Round(no);

            if (noInt < 0 || noInt >= vector.Length)
            {
                throw new SyntaxException("element access out of range");
            }

            return vector[noInt];
        }

        [Expression("clear")]
        public void clear()
        {
            Context?.Reset();
        }

        [Expression("LowPass", isConstant: true)]
        public static double[] LowPass(double[] signal, double cutoff, double samplingRate)
        {
            var b = LowPassFIRCoefficients(cutoff, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
            return filtfilt(b, signal);
        }

        [Expression("HighPass", isConstant: true)]
        public static double[] HighPass(double[] signal, double cutoff, double samplingRate)
        {
            var b = HighPassFIRCoefficients(cutoff, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
            return filtfilt(b, signal);
        }

        [Expression("BandPass", isConstant: true)]
        public static double[] BandPass(double[] signal, double cutoffLow, double cutoffHigh, double samplingRate)
        {
            var b = BandPassFIRCoefficients(cutoffLow, cutoffHigh, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
            return filtfilt(b, signal);
        }

        [Expression("BandStop", isConstant: true)]
        public static double[] BandStop(double[] signal, double cutoffLow, double cutoffHigh, double samplingRate)
        {
            var b = BandStopFIRCoefficients(cutoffLow, cutoffHigh, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
            return filtfilt(b, signal);
        }

        [Expression(isConstant: true)]
        public static double[] LowPassFIRCoefficients(double cutoff, double samplingRate, double order)
        {
            return MathNet.Filtering.FIR.FirCoefficients.LowPass(samplingRate, cutoff, 1.0, (int)(2 * order));
        }

        [Expression(isConstant: true)]
        public static double[] LowPassFIRCoefficients(double cutoff, double samplingRate, double order, double dcGain)
        {
            return MathNet.Filtering.FIR.FirCoefficients.LowPass(samplingRate, cutoff, dcGain, (int)(2 * order));
        }

        [Expression(isConstant: true)]
        public static double[] HighPassFIRCoefficients(double cutoff, double samplingRate, double order)
        {
            return MathNet.Filtering.FIR.FirCoefficients.HighPass(samplingRate, cutoff, (int)(2 * order));
        }

        [Expression(isConstant: true)]
        public static double[] BandPassFIRCoefficients(double cutoffLow, double cutoffHigh, double samplingRate, double order)
        {
            return MathNet.Filtering.FIR.FirCoefficients.BandPass(samplingRate, cutoffLow, cutoffHigh, (int)(2 * order));
        }

        [Expression(isConstant: true)]
        public static double[] BandStopFIRCoefficients(double cutoffLow, double cutoffHigh, double samplingRate, double order)
        {
            return MathNet.Filtering.FIR.FirCoefficients.BandStop(samplingRate, cutoffLow, cutoffHigh, (int)(2 * order));
        }

        [Expression(isConstant: true)]
        public static double[] filter(double[] b, double[] a, double[] x, double[] zi)
        {
            normalizeCoefficients(ref b, ref a);

            var filterOrder = b.Length;
            var inputSize = x.Length;

            Array.Resize(ref zi, filterOrder);

            var y = new double[inputSize];

            for (int it = 0; it < inputSize; ++it)
            {
                int order = filterOrder - 1;
                while (order != 0)
                {
                    if (it >= order)
                    {
                        zi[order - 1] = b[order] * x[it - order] - a[order] * y[it - order] + zi[order];
                    }
                    --order;
                }
                y[it] = b[0] * x[it] + zi[0];
            }

            return y;
        }

        [Expression(isConstant: true)]
        public static double[] filter(double[] b, double[] a, double[] x)
        {
            return filter(b, a, x, new double[] { 0.0 });
        }

        [Expression(isConstant: true)]
        public static double[] filter(double[] b, double[] x)
        {
            return filter(b, new double[] { 1.0 }, x, new double[] { 0.0 });
        }

        [Expression(isConstant: true)]
        public static double[] filtfilt(double[] b, double[] a, double[] x)
        {
            normalizeCoefficients(ref b, ref a);

            var lx = x.Length;
            var n = a.Length;
            var lrefl = 3 * n;

            if (lx <= lrefl)
            {
                throw new SyntaxException("Input data too short! Data must have length more than 3 times filter order.");
            }

            var kdc = b.Sum() / a.Sum();
            double[] si;
            if (!double.IsInfinity(kdc) && !double.IsNaN(kdc))
            {
                double sum = 0.0;
                si = a.Zip(b, (a1, b1) => b1 - kdc * a1).Reverse().Select(va => sum + va).Reverse().Skip(1).ToArray();
            }
            else
            {
                si = Enumerable.Repeat(0.0, n).ToArray();
            }

            var v1 = x.Skip(1).Reverse().Select(x1 => 2 * x.First() - x1);
            var v2 = x;
            var v3 = x.Reverse().Skip(1).Select(x1 => 2 * x.Last() - x1);
            var v = v1.Concat(v2).Concat(v3).ToArray();

            // forward filter
            var vo1 = filter(b, a, v, si.Select(si1 => si1 * v.First()).ToArray());

            // reverse filter
            var vo2 = filter(b, a, vo1.Reverse().ToArray(), si.Select(si1 => si1 * vo1.Last()).ToArray()).Reverse();

            var y = vo2.Skip(lrefl).Take(lx).ToArray();
            return y;
        }

        [Expression(isConstant: true)]
        public static double[] filtfilt(double[] b, double[] x)
        {
            return filtfilt(b, new double[] { 1.0 }, x);
        }

        private static void normalizeCoefficients(ref double[] b, ref double[] a)
        {
            if (!a.Any())
            {
                throw new SyntaxException("the feedback filter coefficients are empty");
            }

            if (a[0] == 0.0)
            {
                throw new SyntaxException("the first feedback filter coefficient has to be non-zero");
            }

            // normalize filter coefficients
            if (a[0] != 1.0)
            {
                for (int it = b.Length; it >= 0; --it)
                {
                    b[it] /= a[0];
                }

                for (int it = a.Length; it >= 0; --it)
                {
                    a[it] /= a[0];
                }
            }

            var filterOrder = b.Length > a.Length ? b.Length : a.Length;

            Array.Resize(ref a, filterOrder);
            Array.Resize(ref b, filterOrder);
        }
    }
}
