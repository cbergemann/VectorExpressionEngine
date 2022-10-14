using System;

namespace VectorExpressionEngine
{
    public class NodeTernaryOperation : INode
    {
        public delegate object TernaryOperation(object p1, object p2, object p3);

        public NodeTernaryOperation(TernaryOperation op, INode p1, INode p2, INode p3)
        {
            Operation = op;
            Parameter1 = p1;
            Parameter2 = p2;
            Parameter3 = p3;
        }

        public TernaryOperation Operation { get; }

        public INode Parameter1 { get; }

        public INode Parameter2 { get; }

        public INode Parameter3 { get; }

        public object Eval(IContext ctx)
        {
            var p1 = Parameter1.Eval(ctx);
            var p2 = Parameter2.Eval(ctx);
            var p3 = Parameter3.Eval(ctx);
            return Operation(p1, p2, p3);
        }
    }
}
