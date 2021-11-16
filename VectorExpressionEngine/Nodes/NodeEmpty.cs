namespace VectorExpressionEngine
{
    public class NodeEmpty : INode
    {
        public object Eval(IContext ctx)
        {
            return null;
        }
    }
}
