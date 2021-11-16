using System.Collections.Generic;
using System.Linq;

namespace VectorExpressionEngine
{
    public class NodeFunctionCall : INode
    {
        public NodeFunctionCall(string functionName, IEnumerable<INode> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments.ToArray();
        }

        public string FunctionName { get; }

        public INode[] Arguments { get; }

        public object Eval(IContext ctx)
        {
            var argumentValues = new object[Arguments.Length];
            for (int i=0; i< Arguments.Length; i++)
            {
                argumentValues[i] = Arguments[i].Eval(ctx);
            }

            return ctx.CallFunction(FunctionName, argumentValues);
        }
    }
}
