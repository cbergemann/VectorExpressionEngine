using System;

namespace VectorExpressionEngine
{
    public class NodeUnaryOperation : INode
    {
        public delegate object UnaryOperation(object p2);

        public NodeUnaryOperation(UnaryOperation op, INode p1)
        {
            Operation = op;
            Parameter1 = p1;
        }

        public UnaryOperation Operation { get; }

        public INode Parameter1 { get; }

        public object Eval(IContext ctx)
        {
            var p1 = Parameter1.Eval(ctx);
            return Operation(p1);
        }
    }
}
