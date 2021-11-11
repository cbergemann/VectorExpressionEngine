using System;

namespace VectorExpressionEngine
{
    public class NodeUnaryOperation : Node
    {
        public NodeUnaryOperation(Func<object, object> op, Node p1)
        {
            Operation = op;
            Parameter1 = p1;
        }

        public Func<object, object> Operation { get; }

        public Node Parameter1 { get; }

        public override object Eval(Context ctx)
        {
            var p1 = Parameter1.Eval(ctx);
            return Operation(p1);
        }
    }
}
