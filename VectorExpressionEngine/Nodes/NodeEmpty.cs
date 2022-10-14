namespace VectorExpressionEngine
{
    public class NodeEmpty : INode
    {
        public static NodeEmpty Empty { get; } = new NodeEmpty();

        private NodeEmpty() { }

        public object Eval(IContext ctx)
        {
            return null;
        }
    }
}
