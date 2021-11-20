using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace VectorExpressionEngine.Tests
{
    internal enum ExceptionMessageCompareOptions
    {
        ExactMatch,
        Contains,
    }

    internal class ThrowsAssert
    {
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"exception mismatch: expected: {typeof(TException).Name}, actual: {ex.GetType().Name}", ex);
            }

            throw new AssertFailedException($"no exception caught, expected: {typeof(TException).Name}");
        }

        public static void Throws<TException>(Action action, string message, ExceptionMessageCompareOptions options = ExceptionMessageCompareOptions.ExactMatch)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                if (options == ExceptionMessageCompareOptions.Contains)
                {
                    if (ex.Message.Contains(message))
                    {
                        return;
                    }

                    throw new AssertFailedException($"exception message mismatch: expected to contain: {message}, actual: {ex.Message}", ex);
                }

                if (options == ExceptionMessageCompareOptions.ExactMatch)
                {
                    if (ex.Message == message)
                    {
                        return;
                    }

                    throw new AssertFailedException($"exception message mismatch: expected: {message}, actual: {ex.Message}", ex);
                }

                throw new InvalidOperationException();
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"exception mismatch: expected: {typeof(TException).Name}, actual: {ex.GetType().Name}", ex);
            }

            throw new AssertFailedException($"no exception caught, expected: {typeof(TException).Name}");
        }
    }
}
