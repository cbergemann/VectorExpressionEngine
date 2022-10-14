using System;

namespace VectorExpressionEngine
{
    public class NodeDeferredTernaryOperation : INode
    {
        public delegate object DeferredTernaryOperation(INode p1, INode p2, INode p3, IContext ctx);

        public NodeDeferredTernaryOperation(DeferredTernaryOperation op, INode p1, INode p2, INode p3)
        {
            Operation = op;
            Parameter1 = p1;
            Parameter2 = p2;
            Parameter3 = p3;
        }

        public DeferredTernaryOperation Operation { get; }

        public INode Parameter1 { get; }

        public INode Parameter2 { get; }

        public INode Parameter3 { get; }

        public object Eval(IContext ctx) => Operation(Parameter1, Parameter2, Parameter3, ctx);
    }
}