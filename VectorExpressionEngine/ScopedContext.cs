using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorExpressionEngine
{
    public class ScopedContext : Context
    {
        public ScopedContext(Context parentContext)
        {
            Parent = parentContext;
        }

        public Context Parent { get; }

        private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();

        public override object ResolveVariable(string name)
        {
            if (_variables.TryGetValue(name, out var value))
            {
                return value;
            }

            return Parent.ResolveVariable(name);
        }

        public override void AssignVariable(string name, object value)
        {
            _variables[name] = value;
        }

        public override object CallFunction(string name, object[] arguments)
        {
            return Parent.CallFunction(name, arguments);
        }

        public override bool IsConstantExpressionVariable(string name)
        {
            if (_variables.ContainsKey(name))
            {
                return true;
            }

            return Parent.IsConstantExpressionVariable(name);
        }

        public override bool IsConstantExpressionCall(string name, Type[] arguments)
        {
            return Parent.IsConstantExpressionCall(name, arguments);
        }

    }
}
