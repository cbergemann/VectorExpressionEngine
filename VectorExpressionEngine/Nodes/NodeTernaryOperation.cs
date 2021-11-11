using System;

namespace VectorExpressionEngine
{
    public class NodeTernaryOperation : Node
    {
        public NodeTernaryOperation(Func<object, object, object, object> op, Node p1, Node p2, Node p3)
        {
            Operation = op;
            Parameter1 = p1;
            Parameter2 = p2;
            Parameter3 = p3;
        }

        public Func<object, object, object, object> Operation { get; }

        public Node Parameter1 { get; }

        public Node Parameter2 { get; }

        public Node Parameter3 { get; }

        public override object Eval(Context ctx)
        {
            var p1 = Parameter1.Eval(ctx);
            var p2 = Parameter2.Eval(ctx);
            var p3 = Parameter3.Eval(ctx);
            return Operation(p1, p2, p3);
        }
    }
}
