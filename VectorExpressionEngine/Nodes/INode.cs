namespace VectorExpressionEngine
{
    public interface INode
    {
        object Eval(IContext ctx);
    }
}
