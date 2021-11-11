namespace VectorExpressionEngine
{
    public class NodeVariable : Node
    {
        public NodeVariable(string variableName)
        {
            VariableName = variableName;
        }

        public string VariableName { get; }

        public override object Eval(Context ctx)
        {
            return ctx.ResolveVariable(VariableName);
        }
    }
}
