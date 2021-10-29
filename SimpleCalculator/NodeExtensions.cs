﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorExpressionEngine;

namespace SimpleCalculator
{
    public static class NodeExtensions
    {
        public static void PrintNodes(this Node[] nodes)
        {
            foreach (var node in nodes)
            {
                node.PrintNode();
            }
        }

        public static void PrintNode(this Node node, int indent = 0)
        {
            var ind = new string(' ', indent);

            if (node is NodeAssignment nodeAssignment)
            {
                Console.WriteLine($"{ind}NodeAssignment: {nodeAssignment.VariableName}:");
                nodeAssignment.Assignment.PrintNode(indent + 2);
            }
            else if (node is NodeEmpty)
            {
                Console.WriteLine($"{ind}NodeEmpty");
            }
            else if (node is NodeFunctionCall nodeFunctionCall)
            {
                Console.WriteLine($"{ind}NodeFunctionCall: {nodeFunctionCall.FunctionName}");
                foreach (var arg in nodeFunctionCall.Arguments)
                {
                    arg.PrintNode(indent + 2);
                }
            }
            if (node is NodeObject nodeObject)
            {
                Console.WriteLine($"{ind}NodeObject: {nodeObject.Value}:");
            }
            else if (node is NodeObjectArray nodeObjectArray)
            {
                Console.WriteLine($"{ind}NodeObjectArray");
                foreach (var arg in nodeObjectArray.Elements)
                {
                    arg.PrintNode(indent + 2);
                }
            }
            else if (node is NodeUnaryOperation nodeUnaryOperation)
            {
                Console.WriteLine($"{ind}NodeUnaryOperation: {nodeUnaryOperation.Operation}");
                foreach (var arg in new Node[] { nodeUnaryOperation.Parameter1 })
                {
                    arg.PrintNode(indent + 2);
                }
            }
            else if (node is NodeBinaryOperation nodeBinaryOperation)
            {
                Console.WriteLine($"{ind}NodeBinaryOperation: {nodeBinaryOperation.Operation}");
                foreach (var arg in new Node[] { nodeBinaryOperation.Parameter1, nodeBinaryOperation.Parameter2 })
                {
                    arg.PrintNode(indent + 2);
                }
            }
            else if (node is NodeTernaryOperation nodeTernaryOperation)
            {
                Console.WriteLine($"{ind}NodeTernaryOperation: {nodeTernaryOperation.Operation}");
                foreach (var arg in new Node[] { nodeTernaryOperation.Parameter1, nodeTernaryOperation.Parameter2, nodeTernaryOperation.Parameter3 })
                {
                    arg.PrintNode(indent + 2);
                }
            }
            else if (node is NodeVariable nodeVariable)
            {
                Console.WriteLine($"{ind}NodeVariable: {nodeVariable.VariableName}");
            }
            else
            {
                Console.WriteLine($"{ind}Node: {typeof(Node).Name}");
            }
        }
    }
}
