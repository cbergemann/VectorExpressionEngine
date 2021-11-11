using System;

namespace VectorExpressionEngine
{
    // Exception for syntax errors
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) 
            : base(message)
        {
        }

        public SyntaxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
