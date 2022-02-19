using System;
using System.Globalization;
using System.Linq;

namespace VectorExpressionEngine
{
    internal class BasicOperations
    {
        public const double Tolerance = 0.000000001;

        public static object ElementAccess(object vector, object no)
        {
            long idx;
            Array array;

            try
            {
                idx = Convert.ToInt64(no);
                array = (Array)vector;
            }
            catch (InvalidCastException ex)
            {
                throw new SyntaxException($"array element access not defined for types {vector.GetType()}[{no.GetType()}]", ex);
            }

            try
            {
                if (idx < 0)
                {
                    idx = array.LongLength + idx;
                }

                return array.GetValue(idx);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new SyntaxException($"array index {idx} is out of range 0...{array.Length-1}", ex);
            }
        }

        #region Ternary Operators

        public static object TernaryIf(object a, object b, object c)
        {
            return TernaryVectorAwareOperation<bool, double, double, double>((v1, v2, v3) => v1 ? v2 : v3, a, b, c);
        }

        #endregion Ternary Operators

        #region Binary Operators

        public static object Add(object a, object b)
        {
            if (a is string str1)
            {
                if (b is string str2)
                {
                    return str1 + str2;
                }
                
                if (b is double dbl2)
                {
                    return str1 + dbl2.ToString(CultureInfo.InvariantCulture);
                }
            }
            else if (a is double dbl1)
            {
                if (b is string str2)
                {
                    return dbl1.ToString(CultureInfo.InvariantCulture) + str2;
                }
            }
            
            return BinaryVectorAwareOperation<double, double, double>((v1, v2) => v1 + v2, a, b);
        }

        public static object Subtract(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, double>((v1, v2) => v1 - v2, a, b);
        }

        public static object Multiply(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, double>((v1, v2) => v1 * v2, a, b);
        }

        public static object Divide(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, double>((v1, v2) => v1 / v2, a, b);
        }

        public static object Pow(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, double>(Math.Pow, a, b);
        }

        public static object LessThan(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, bool>((v1, v2) => v1 < v2, a, b);
        }

        public static object LessThanOrEqual(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, bool>((v1, v2) => v1 <= v2, a, b);
        }

        public static object GreaterThan(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, bool>((v1, v2) => v1 > v2, a, b);
        }

        public static object GreaterThanOrEqual(object a, object b)
        {
            return BinaryVectorAwareOperation<double, double, bool>((v1, v2) => v1 >= v2, a, b);
        }

        public static object Equal(object a, object b)
        {
            if ((a is bool || a is bool[]) && (b is bool || b is bool[]))
            {
                return BinaryVectorAwareOperation<bool, bool, bool>((v1, v2) => v1 == v2, a, b);
            }

            return BinaryVectorAwareOperation<double, double, bool>((v1, v2) => Math.Abs(v1 - v2) <= Tolerance, a, b);
        }

        public static object NotEqual(object a, object b)
        {
            if ((a is bool || a is bool[]) && (b is bool || b is bool[]))
            {
                return BinaryVectorAwareOperation<bool, bool, bool>((v1, v2) => v1 != v2, a, b);
            }

            return BinaryVectorAwareOperation<double, double, bool>((v1, v2) => Math.Abs(v1 - v2) > Tolerance, a, b);
        }

        public static object And(object a, object b)
        {
            return BinaryVectorAwareOperation<bool, bool, bool>((v1, v2) => v1 && v2, a, b);
        }

        public static object Or(object a, object b)
        {
            return BinaryVectorAwareOperation<bool, bool, bool>((v1, v2) => v1 || v2, a, b);
        }

        #endregion Binary Operators

        #region Unary Operators

        public static object Negate(object a)
        {
            return UnaryVectorAwareOperation<double, double>(v1 => -v1, a);
        }

        public static object Not(object a)
        {
            return UnaryVectorAwareOperation<bool, bool>(v1 => !v1, a);
        }

        #endregion Unary Opeartors

        public static object TernaryVectorAwareOperation<TIn1,TIn2,TIn3,TOut>(Func<TIn1, TIn2, TIn3, TOut> op, object a, object b, object c)
        {
            if (a is TIn1 scalar1)
            {
                if (b is TIn2 scalar2)
                {
                    if (c is TIn3 scalar3)
                    {
                        return op(scalar1, scalar2, scalar3);
                    }

                    if (c is TIn3[] vector3)
                    {
                        return vector3.Select(v => op(scalar1, scalar2, v)).ToArray();
                    }
                }
                else if (b is TIn2[] vector2)
                {
                    if (c is TIn3 scalar3)
                    {
                        return vector2.Select(v => op(scalar1, v, scalar3)).ToArray();
                    }

                    if (c is TIn3[] vector3)
                    {
                        if (vector2.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, scalar1, vector2[0], vector3);
                        }

                        if (vector3.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, scalar1, vector2, vector3[0]);
                        }

                        if (vector2.Length != vector3.Length)
                        {
                            throw new SyntaxException("ternary operation of different length arrays not defined");
                        }

                        return vector2.Zip(vector3, (v1, v2) => op(scalar1, v1, v2)).ToArray();
                    }
                }
            }
            else if (a is TIn1[] vector1)
            {
                if (b is TIn2 scalar2)
                {
                    if (c is TIn3 scalar3)
                    {
                        return vector1.Select(v => op(v, scalar2, scalar3)).ToArray();
                    }
                    
                    if (c is TIn3[] vector3)
                    {
                        if (vector1.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, vector1[0], scalar2, vector3);
                        }

                        if (vector3.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, vector1, scalar2, vector3[0]);
                        }

                        if (vector1.Length != vector3.Length)
                        {
                            throw new SyntaxException("ternary operation of different length arrays not defined");
                        }

                        return vector1.Zip(vector3, (v1, v2) => op(v1, scalar2, v2)).ToArray();
                    }
                }
                else if (b is TIn2[] vector2)
                {
                    if (c is TIn3 scalar3)
                    {
                        if (vector1.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, vector1[0], vector2, scalar3);
                        }

                        if (vector2.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, vector1, vector2[0], scalar3);
                        }

                        if (vector1.Length != vector2.Length)
                        {
                            throw new SyntaxException("ternary operation of different length arrays not defined");
                        }

                        return vector1.Zip(vector2, (v1, v2) => op(v1, v2, scalar3)).ToArray();
                    }
                    
                    if (c is TIn3[] vector3)
                    {
                        if (vector1.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, vector1[0], vector2, vector3);
                        }

                        if (vector2.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, vector1, vector2[0], vector3);
                        }

                        if (vector3.Length == 1)
                        {
                            return TernaryVectorAwareOperation(op, vector1, vector2, vector3[0]);
                        }

                        if ((vector1.Length != vector2.Length) || (vector2.Length != vector3.Length))
                        {
                            throw new SyntaxException("ternary operation of different length arrays not defined");
                        }

                        return Enumerable.Range(0, vector1.Length).Select(it => op(vector1[it], vector2[it], vector3[it])).ToArray();
                    }
                }
            }

            throw new SyntaxException($"ternary operation cannot handle types {a?.GetType().ToString() ?? "null"}, {b?.GetType().ToString() ?? "null"} and {c?.GetType().ToString() ?? "null"}");
        }

        public static object BinaryVectorAwareOperation<TIn1,TIn2,TOut>(Func<TIn1, TIn2, TOut> op, object a, object b)
        {
            if (a is TIn1 scalar1)
            {
                if (b is TIn2 scalar2)
                {
                    return op(scalar1, scalar2);
                }

                if (b is TIn2[] vector2)
                {
                    return vector2.Select(v => op(scalar1, v)).ToArray();
                }
            }
            
            if (a is TIn1[] vector1)
            {
                if (b is TIn2 scalar2)
                {
                    return vector1.Select(v => op(v, scalar2)).ToArray();
                }

                if (b is TIn2[] vector2)
                {
                    if (vector1.Length == 1)
                    {
                        return BinaryVectorAwareOperation(op, vector1[0], vector2);
                    }

                    if (vector2.Length == 1)
                    {
                        return BinaryVectorAwareOperation(op, vector1, vector2[0]);
                    }

                    if (vector1.Length != vector2.Length)
                    {
                        throw new SyntaxException("binary operation of different length arrays not defined");
                    }

                    return vector1.Zip(vector2, op).ToArray();
                }
            }

            throw new SyntaxException($"binary operation cannot handle types {a?.GetType().ToString() ?? "null"} and {b?.GetType().ToString() ?? "null"}");
        }

        public static object UnaryVectorAwareOperation<TIn,TOut>(Func<TIn, TOut> op, object a)
        {
            if (a is TIn scalar)
            {
                return op(scalar);
            }

            if (a is TIn[] vector)
            {
                return vector.Select(op).ToArray();
            }

            throw new SyntaxException($"unary operation cannot handle type {a?.GetType().ToString() ?? "null"}");
        }
    }
}
