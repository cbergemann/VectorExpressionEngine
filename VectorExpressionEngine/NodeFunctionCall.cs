using System.Collections.Generic;
using System.Collections.Immutable;

namespace VectorExpressionEngine
{
    public class NodeFunctionCall : Node
    {
        public NodeFunctionCall(string functionName, IEnumerable<Node> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments.ToImmutableArray();
        }

        public string FunctionName { get; }

        public ImmutableArray<Node> Arguments { get; }

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
