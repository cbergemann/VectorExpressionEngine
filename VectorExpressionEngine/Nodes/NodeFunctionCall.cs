using System.Collections.Generic;
using System.Linq;

namespace VectorExpressionEngine
{
    public class NodeFunctionCall : Node
    {
        public NodeFunctionCall(string functionName, IEnumerable<Node> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments.ToArray();
        }

        public string FunctionName { get; }

        public Node[] Arguments { get; }

        public override object Eval(Context ctx)
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
