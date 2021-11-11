using System;

namespace VectorExpressionEngine
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class ExpressionAttribute : Attribute
    {
        public ExpressionAttribute(string expressionName = null, bool isConstant = false)
        {
            ExpressionName = expressionName;
            IsConstant = isConstant;
        }

        public string ExpressionName { get; }

        public bool IsConstant { get; }
    }
}
