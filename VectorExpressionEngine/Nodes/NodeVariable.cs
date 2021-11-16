namespace VectorExpressionEngine
{
    public class NodeVariable : INode
    {
        public NodeVariable(string variableName)
        {
            VariableName = variableName;
        }

        public string VariableName { get; }

        public object Eval(IContext ctx)
        {
            return ctx.ResolveVariable(VariableName);
        }
    }
}
