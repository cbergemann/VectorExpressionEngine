using System.Linq;

namespace VectorExpressionEngine
{
    public static class TreeOptimizer
    {
        public static void Optimize(INode[] nodes, IContext ctx = null)
        {
            var doneSomething = true;
            while (doneSomething)
            {
                for (int it = 0; it < nodes.Length; it++)
                {
                    nodes[it] = OptimizeNode(nodes[it], ctx, out doneSomething);
                    if (doneSomething)
                    {
                        break;
                    }
                }
            }
        }

        public static INode Optimize(INode node, IContext ctx = null)
        {
            var doneSomething = true;
            while (doneSomething)
            {
                node = OptimizeNode(node, ctx, out doneSomething);
            }
            return node;
        }

        private static INode OptimizeNode(INode node, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;

            if (node is NodeFunctionCall nodeFc)
            {
                node = OptimizeFunctionCall(nodeFc, ctx, out bool doneSomethingFc);
                if (doneSomethingFc)
                {
                    doneSomething = true;
                }
            }

            if (node is NodeUnaryOperation nodeUnaryOp)
            {
                node = OptimizeUnaryOperation(nodeUnaryOp, ctx, out bool doneSomethingOp);
                if (doneSomethingOp)
                {
                    doneSomething = true;
                }
            }

            if (node is NodeBinaryOperation nodeBinaryOp)
            {
                node = OptimizeBinaryOperation(nodeBinaryOp, ctx, out bool doneSomethingOp);
                if (doneSomethingOp)
                {
                    doneSomething = true;
                }
            }

            if (node is NodeTernaryOperation nodeTernaryOp)
            {
                node = OptimizeTernaryOperation(nodeTernaryOp, ctx, out bool doneSomethingOp);
                if (doneSomethingOp)
                {
                    doneSomething = true;
                }
            }

            if (node is NodeObjectArray nodeArr)
            {
                node = OptimizeObjectArray(nodeArr, ctx, out bool doneSomethingArr);
                if (doneSomethingArr)
                {
                    doneSomething = true;
                }
            }

            if (node is NodeVariable nodeVar)
            {
                node = OptimizeVariable(nodeVar, ctx, out bool doneSomethingVar);
                if (doneSomethingVar)
                {
                    doneSomething = true;
                }
            }

            if (node is NodeAssignment nodeAssignment)
            {
                node = OptimizeAssignment(nodeAssignment, ctx, out bool doneSomethingAss);
                if (doneSomethingAss)
                {
                    doneSomething = true;
                }
            }

            return node;
        }

        private static INode OptimizeFunctionCall(NodeFunctionCall nodeFc, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;
            var isObjectOnly = true;

            var arguments = new INode[nodeFc.Arguments.Length];
            for (int it = 0; it < nodeFc.Arguments.Length; it++)
            {
                arguments[it] = OptimizeNode(nodeFc.Arguments[it], ctx, out bool doneSomethingArg);
                if (!(arguments[it] is NodeObject))
                {
                    isObjectOnly = false;
                }

                if (doneSomethingArg)
                {
                    doneSomething = true;
                }
            }

            if (doneSomething)
            {
                nodeFc = new NodeFunctionCall(nodeFc.FunctionName, arguments);
            }

            if (ctx != null && isObjectOnly)
            {
                var argumentTypes = nodeFc.Arguments.Cast<NodeObject>().Select(a => a.Value.GetType()).ToArray();
                if (ctx.IsConstantExpressionCall(nodeFc.FunctionName, argumentTypes))
                {
                    doneSomething = true;
                    return new NodeObject(nodeFc.Eval(ctx));
                }
            }

            return nodeFc;
        }

        private static INode OptimizeUnaryOperation(NodeUnaryOperation nodeOp, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;

            var p1 = OptimizeNode(nodeOp.Parameter1, ctx, out bool doneSomethingPar);
            if (doneSomethingPar)
            {
                doneSomething = true;
            }

            if (p1 is NodeObject)
            {
                doneSomething = true;
                return new NodeObject(nodeOp.Eval(null));
            }

            if (doneSomething)
            {
                return new NodeUnaryOperation(nodeOp.Operation, p1);
            }

            return nodeOp;
        }

        private static INode OptimizeBinaryOperation(NodeBinaryOperation nodeOp, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;

            var p1 = OptimizeNode(nodeOp.Parameter1, ctx, out bool doneSomethingPar);
            if (doneSomethingPar)
            {
                doneSomething = true;
            }

            var p2 = OptimizeNode(nodeOp.Parameter2, ctx, out doneSomethingPar);
            if (doneSomethingPar)
            {
                doneSomething = true;
            }

            if (p1 is NodeObject && p2 is NodeObject)
            {
                doneSomething = true;
                return new NodeObject(nodeOp.Eval(null));
            }

            if (doneSomething)
            {
                return new NodeBinaryOperation(nodeOp.Operation, p1, p2);
            }

            return nodeOp;
        }

        private static INode OptimizeTernaryOperation(NodeTernaryOperation nodeOp, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;

            var p1 = OptimizeNode(nodeOp.Parameter1, ctx, out bool doneSomethingPar);
            if (doneSomethingPar)
            {
                doneSomething = true;
            }

            var p2 = OptimizeNode(nodeOp.Parameter2, ctx, out doneSomethingPar);
            if (doneSomethingPar)
            {
                doneSomething = true;
            }

            var p3 = OptimizeNode(nodeOp.Parameter3, ctx, out doneSomethingPar);
            if (doneSomethingPar)
            {
                doneSomething = true;
            }

            if (p1 is NodeObject && p2 is NodeObject && p3 is NodeObject)
            {
                doneSomething = true;
                return new NodeObject(nodeOp.Eval(null));
            }

            if (doneSomething)
            {
                return new NodeTernaryOperation(nodeOp.Operation, p1, p2, p3);
            }

            return nodeOp;
        }

        private static INode OptimizeObjectArray(NodeObjectArray nodeArr, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;
            var isObjectOnly = true;

            var elements = new INode[nodeArr.Elements.Length];
            for (int it = 0; it < nodeArr.Elements.Length; it++)
            {
                elements[it] = OptimizeNode(nodeArr.Elements[it], ctx, out bool doneSomethingPar);
                if (!(elements[it] is NodeObject))
                {
                    isObjectOnly = false;
                }

                if (doneSomethingPar)
                {
                    doneSomething = true;
                }
            }

            if (doneSomething)
            {
                nodeArr = new NodeObjectArray(elements);
            }

            if (isObjectOnly)
            {
                doneSomething = true;
                return new NodeObject(nodeArr.Eval(null));
            }

            return nodeArr;
        }

        private static INode OptimizeVariable(NodeVariable nodeVar, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;

            if (ctx != null && ctx.IsConstantExpressionVariable(nodeVar.VariableName))
            {
                doneSomething = true;
                return new NodeObject(nodeVar.Eval(ctx));
            }

            return nodeVar;
        }

        private static INode OptimizeAssignment(NodeAssignment nodeAssignment, IContext ctx, out bool doneSomething)
        {
            doneSomething = false;

            var assignment = OptimizeNode(nodeAssignment.Assignment, ctx, out bool doneSomethingAss);
            if (doneSomethingAss)
            {
                doneSomething = true;
                return new NodeAssignment(nodeAssignment.VariableName, assignment);
            }

            return nodeAssignment;
        }
    }
}
