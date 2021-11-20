using VectorExpressionEngine;
using System;
using System.Linq;
using SimpleCalculator.Models;
// ReSharper disable UnusedMember.Global

namespace SimpleCalculator.Business;

public class MathLibrary : ExtendedMath
{
    [Expression("plot")]
    public static LineSeries Plot(double[] x, double[] y)
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
    public static LineSeries Plot(double[] x)
    {
        return new LineSeries
        {
            X = x,
            Y = Enumerable.Range(0, x.Length).Select(it => (double)it).ToArray(),
        };
    }

    [Expression("linspace", isConstant: true)]
    public static double[] Linspace(double start, double end, double count)
    {
        if (count <= 0)
        {
            return Array.Empty<double>();
        }

        return Enumerable.Range(0, (int)(count - 1)).Select(it => start + (end - start) / (count - 1) * it).Append(end).ToArray();
    }

    [Expression("element", isConstant: true)]
    public static double Element(double[] vector, double no)
    {
        int noInt = (int)Math.Round(no);

        if (noInt < 0 || noInt >= vector.Length)
        {
            throw new SyntaxException("element access out of range");
        }

        return vector[noInt];
    }

    [Expression("LowPass", isConstant: true)]
    public static double[] LowPass(double[] signal, double cutoff, double samplingRate)
    {
        var b = LowPassFIRCoefficients(cutoff, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
        return FiltFilt(b, signal);
    }

    [Expression("HighPass", isConstant: true)]
    public static double[] HighPass(double[] signal, double cutoff, double samplingRate)
    {
        var b = HighPassFIRCoefficients(cutoff, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
        return FiltFilt(b, signal);
    }

    [Expression("BandPass", isConstant: true)]
    public static double[] BandPass(double[] signal, double cutoffLow, double cutoffHigh, double samplingRate)
    {
        var b = BandPassFIRCoefficients(cutoffLow, cutoffHigh, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
        return FiltFilt(b, signal);
    }

    [Expression("BandStop", isConstant: true)]
    public static double[] BandStop(double[] signal, double cutoffLow, double cutoffHigh, double samplingRate)
    {
        var b = BandStopFIRCoefficients(cutoffLow, cutoffHigh, samplingRate, Math.Min(signal.Length * 3 - 1, 500));
        return FiltFilt(b, signal);
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
    public static double[] Filter(double[] b, double[] a, double[] x, double[] zi)
    {
        NormalizeCoefficients(ref b, ref a);

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
    public static double[] Filter(double[] b, double[] a, double[] x)
    {
        return Filter(b, a, x, new [] { 0.0 });
    }

    [Expression(isConstant: true)]
    public static double[] Filter(double[] b, double[] x)
    {
        return Filter(b, new [] { 1.0 }, x, new [] { 0.0 });
    }

    [Expression(isConstant: true)]
    public static double[] FiltFilt(double[] b, double[] a, double[] x)
    {
        NormalizeCoefficients(ref b, ref a);

        var dataLength = x.Length;
        var filterOrder = a.Length;
        var requiredDataLength = 3 * filterOrder;

        if (dataLength <= requiredDataLength)
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
            si = Enumerable.Repeat(0.0, filterOrder).ToArray();
        }

        var leadIn = x.Skip(1).Reverse().Select(x1 => 2 * x.First() - x1);
        var leadOut = x.Reverse().Skip(1).Select(x1 => 2 * x.Last() - x1);
        var paddedData = leadIn.Concat(x).Concat(leadOut).ToArray();

        var forwardFilterData = Filter(b, a, paddedData, si.Select(si1 => si1 * paddedData.First()).ToArray());
        var reverseFilterData = Filter(b, a, forwardFilterData.Reverse().ToArray(), si.Select(si1 => si1 * forwardFilterData.Last()).ToArray()).Reverse();

        var filteredData = reverseFilterData.Skip(requiredDataLength).Take(dataLength).ToArray();
        return filteredData;
    }

    [Expression(isConstant: true)]
    public static double[] FiltFilt(double[] b, double[] x)
    {
        return FiltFilt(b, new [] { 1.0 }, x);
    }

    private static void NormalizeCoefficients(ref double[] b, ref double[] a)
    {
        if (!a.Any())
        {
            throw new SyntaxException("the feedback filter coefficients are empty");
        }

        if (Math.Abs(a[0]) < 1e-8)
        {
            throw new SyntaxException("the first feedback filter coefficient has to be non-zero");
        }

        // normalize filter coefficients
        for (int it = b.Length - 1; it >= 0; --it)
        {
            b[it] /= a[0];
        }

        for (int it = a.Length - 1; it >= 0; --it)
        {
            a[it] /= a[0];
        }

        var filterOrder = b.Length > a.Length ? b.Length : a.Length;

        Array.Resize(ref a, filterOrder);
        Array.Resize(ref b, filterOrder);
    }
}