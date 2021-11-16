using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorExpressionEngine
{
    public class ScopedContext : IContext
    {
        public ScopedContext(IContext parentContext)
        {
            Parent = parentContext;
        }

        public IContext Parent { get; }

        private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();

        public object ResolveVariable(string name)
        {
            if (_variables.TryGetValue(name, out var value))
            {
                return value;
            }

            return Parent.ResolveVariable(name);
        }

        public void AssignVariable(string name, object value)
        {
            _variables[name] = value;
        }

        public object CallFunction(string name, object[] arguments)
        {
            return Parent.CallFunction(name, arguments);
        }

        public bool IsConstantExpressionVariable(string name)
        {
            if (_variables.ContainsKey(name))
            {
                return true;
            }

            return Parent.IsConstantExpressionVariable(name);
        }

        public bool IsConstantExpressionCall(string name, Type[] arguments)
        {
            return Parent.IsConstantExpressionCall(name, arguments);
        }

    }
}
