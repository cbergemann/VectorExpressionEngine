namespace VectorExpressionEngine
{
    public class NodeObject : Node
    {
        public NodeObject(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override object Eval(Context ctx)
        {
            return Value;
        }
    }
}
