using System;
// ReSharper disable UnusedMember.Global

namespace VectorExpressionEngine
{
    public class ExtendedMath
    {
        [Expression("pi", isConstant: true)]
        public static double Pi => Math.PI;

        [Expression("e", isConstant: true)]
        public static double E => Math.E;

        [Expression("exp", isConstant: true)]
        public static object Exp(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Exp, a);
        }

        [Expression("log", isConstant: true)]
        public static object Log(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Log, a);
        }

        [Expression("log", isConstant: true)]
        public static object Log(object a, double newBase)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>((v1) => Math.Log(v1, newBase), a);
        }

        [Expression("log10", isConstant: true)]
        public static object Log10(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Log10, a);
        }

        [Expression("sqrt", isConstant: true)]
        public static object Sqrt(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Sqrt, a);
        }

        [Expression("abs", isConstant: true)]
        public static object Abs(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Abs, a);
        }

        [Expression("sin", isConstant: true)]
        public static object Sin(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Sin, a);
        }

        [Expression("sind", isConstant: true)]
        public static object SinD(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>((v1) => Math.Sin(v1 * Math.PI / 180.0), a);
        }

        [Expression("cos", isConstant: true)]
        public static object Cos(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Cos, a);
        }

        [Expression("cosd", isConstant: true)]
        public static object CosD(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>((v1) => Math.Cos(v1 * Math.PI / 180.0), a);
        }

        [Expression("tan", isConstant: true)]
        public static object Tan(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Tan, a);
        }

        [Expression("tand", isConstant: true)]
        public static object TanD(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>((v1) => Math.Tan(v1 * Math.PI / 180.0), a);
        }

        [Expression("asin", isConstant: true)]
        public static object Asin(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Asin, a);
        }

        [Expression("asind", isConstant: true)]
        public static object AsinD(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>((v1) => Math.Asin(v1) * 180.0 / Math.PI, a);
        }

        [Expression("acos", isConstant: true)]
        public static object Acos(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Acos, a);
        }

        [Expression("acosd", isConstant: true)]
        public static object AcosD(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>((v1) => Math.Acos(v1) * 180.0 / Math.PI, a);
        }

        [Expression("atan", isConstant: true)]
        public static object Atan(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Atan, a);
        }

        [Expression("atand", isConstant: true)]
        public static object AtanD(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>((v1) => Math.Atan(v1) * 180.0 / Math.PI, a);
        }

        [Expression("atan2", isConstant: true)]
        public static object Atan2(object a, object b)
        {
            return BasicOperations.BinaryVectorAwareOperation<double, double, double>(Math.Atan2, a, b);
        }

        [Expression("atan2d", isConstant: true)]
        public static object Atan2D(object a, object b)
        {
            return BasicOperations.BinaryVectorAwareOperation<double, double, double>((v1, v2) => Math.Atan2(v1, v2) * 180.0 / Math.PI, a, b);
        }

        [Expression("sinh", isConstant: true)]
        public static object Sinh(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Sinh, a);
        }

        [Expression("cosh", isConstant: true)]
        public static object Cosh(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Cosh, a);
        }

        [Expression("tanh", isConstant: true)]
        public static object Tanh(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Tanh, a);
        }

        [Expression("mod", isConstant: true)]
        public static object Mod(object a, object b)
        {
            return BasicOperations.BinaryVectorAwareOperation<double, double, double>((v1, v2) => v1 % v2, a, b);
        }

        [Expression("round", isConstant: true)]
        public static object Round(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Round, a);
        }

        [Expression("floor", isConstant: true)]
        public static object Floor(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Floor, a);
        }

        [Expression("ceil", isConstant: true)]
        public static object Ceiling(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(Math.Ceiling, a);
        }

        [Expression("sign", isConstant: true)]
        public static object Sign(object a)
        {
            return BasicOperations.UnaryVectorAwareOperation<double, double>(v1 => Math.Sign(v1), a);
        }

        [Expression("max", isConstant: true)]
        public static object Max(object a, object b)
        {
            return BasicOperations.BinaryVectorAwareOperation<double, double, double>(Math.Max, a, b);
        }

        [Expression("min", isConstant: true)]
        public static object Min(object a, object b)
        {
            return BasicOperations.BinaryVectorAwareOperation<double, double, double>(Math.Min, a, b);
        }

        [Expression("absmax", isConstant: true)]
        public static object AbsMax(object a, object b)
        {
            return BasicOperations.BinaryVectorAwareOperation<double, double, double>((v1, v2) => Math.Abs(v1) > Math.Abs(v2) ? v1 : v2, a, b);
        }
    }
}
