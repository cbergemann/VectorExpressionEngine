using System;

namespace VectorExpressionEngine
{
    public interface IContext
    {
        object ResolveVariable(string name);

        void AssignVariable(string name, object value);

        object CallFunction(string name, object[] arguments);

        bool IsConstantExpressionVariable(string name);

        bool IsConstantExpressionCall(string name, Type[] arguments);
    }
}
