using System;

namespace VectorExpressionEngine
{
    public class NodeBinaryOperation : Node
    {
        public NodeBinaryOperation(Func<object, object, object> op, Node p1, Node p2)
        {
            Operation = op;
            Parameter1 = p1;
            Parameter2 = p2;
        }

        public Func<object, object, object> Operation { get; }

        public Node Parameter1 { get; }

        public Node Parameter2 { get; }

        public override object Eval(Context ctx)
        {
            var p1 = Parameter1.Eval(ctx);
            var p2 = Parameter2.Eval(ctx);
            return Operation(p1, p2);
        }
    }
}
