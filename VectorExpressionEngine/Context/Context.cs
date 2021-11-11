using System;

namespace VectorExpressionEngine
{
    public abstract class Context
    {
        public abstract object ResolveVariable(string name);

        public abstract void AssignVariable(string name, object value);

        public abstract object CallFunction(string name, object[] arguments);

        public virtual bool IsConstantExpressionVariable(string name) { return false; }

        public virtual bool IsConstantExpressionCall(string name, Type[] arguments) { return false; }
    }
}
