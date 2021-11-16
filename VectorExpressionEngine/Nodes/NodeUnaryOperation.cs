using System;

namespace VectorExpressionEngine
{
    public class NodeUnaryOperation : INode
    {
        public NodeUnaryOperation(Func<object, object> op, INode p1)
        {
            Operation = op;
            Parameter1 = p1;
        }

        public Func<object, object> Operation { get; }

        public INode Parameter1 { get; }

        public object Eval(IContext ctx)
        {
            var p1 = Parameter1.Eval(ctx);
            return Operation(p1);
        }
    }
}
