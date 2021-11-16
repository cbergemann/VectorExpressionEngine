namespace VectorExpressionEngine
{
    public class NodeObject : INode
    {
        public NodeObject(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public object Eval(IContext ctx)
        {
            return Value;
        }
    }
}
