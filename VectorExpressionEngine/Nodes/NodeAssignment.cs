namespace VectorExpressionEngine
{
    public class NodeAssignment : INode
    {
        public NodeAssignment(string variableName, INode assignment)
        {
            VariableName = variableName;
            Assignment = assignment;
        }

        public string VariableName { get; }

        public INode Assignment { get; }

        public object Eval(IContext ctx)
        {
            var value = Assignment.Eval(ctx);
            ctx.AssignVariable(VariableName, value);
            return value;
        }
    }
}
