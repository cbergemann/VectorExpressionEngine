namespace VectorExpressionEngine
{
    public class NodeAssignment : Node
    {
        public NodeAssignment(string variableName, Node assignment)
        {
            VariableName = variableName;
            Assignment = assignment;
        }

        public string VariableName { get; }

        public Node Assignment { get; }

        public override object Eval(Context ctx)
        {
            var value = Assignment.Eval(ctx);
            ctx.AssignVariable(VariableName, value);
            return value;
        }
    }
}
