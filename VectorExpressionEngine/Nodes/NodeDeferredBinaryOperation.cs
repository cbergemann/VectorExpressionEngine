using System;

namespace VectorExpressionEngine
{
    public class NodeDeferredBinaryOperation : INode
    {
        public delegate object DeferredBinaryOperation(INode p1, INode p2, IContext ctx);

        public NodeDeferredBinaryOperation(DeferredBinaryOperation op, INode p1, INode p2)
        {
            Operation = op;
            Parameter1 = p1;
            Parameter2 = p2;
        }

        public DeferredBinaryOperation Operation { get; }

        public INode Parameter1 { get; }

        public INode Parameter2 { get; }

        public object Eval(IContext ctx) => Operation(Parameter1, Parameter2, ctx);
    }
}