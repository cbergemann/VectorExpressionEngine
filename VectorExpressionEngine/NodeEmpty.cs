namespace VectorExpressionEngine
{
    public class NodeEmpty : Node
    {
        public override object Eval(Context ctx)
        {
            return null;
        }
    }
}
