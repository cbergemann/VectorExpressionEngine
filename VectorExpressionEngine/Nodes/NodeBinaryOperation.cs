using System;

namespace VectorExpressionEngine
{
    public class NodeBinaryOperation : INode
    {
        public NodeBinaryOperation(Func<object, object, object> op, INode p1, INode p2)
        {
            Operation = op;
            Parameter1 = p1;
            Parameter2 = p2;
        }

        public Func<object, object, object> Operation { get; }

        public INode Parameter1 { get; }

        public INode Parameter2 { get; }

        public object Eval(IContext ctx)
        {
            var p1 = Parameter1.Eval(ctx);
            var p2 = Parameter2.Eval(ctx);
            return Operation(p1, p2);
        }
    }
}
