using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VectorExpressionEngine
{
    public class Parser
    {
        public Parser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        private readonly Tokenizer _tokenizer;

        public INode[] ParseExpression()
        {
            var expressions = new List<INode>();

            while (true)
            {
                while (_tokenizer.Token == Token.EndOfExpression)
                {
                    expressions.Add(new NodeEmpty());
                    _tokenizer.NextToken();
                }

                if (_tokenizer.Token == Token.EndOfFile)
                {
                    break;
                }

                var expression = ParseAssignment();
                expressions.Add(expression);

                if (_tokenizer.Token == Token.EndOfFile)
                {
                    break;
                }
                
                if (_tokenizer.Token == Token.EndOfExpression)
                {
                    continue;
                }

                throw new SyntaxException("Unexpected characters at end of expression");
            }

            // remove empty nodes if not at the end
            return expressions.Where(n => !(n is NodeEmpty && expressions.Last() != n)).ToArray();
        }

        private INode ParseAssignment()
        {
            var lhs = ParseTernary();

            while (true)
            {
                if (_tokenizer.Token != Token.Assignment)
                {
                    return lhs;
                }

                if (!(lhs is NodeVariable varNode))
                {
                    throw new SyntaxException("Cannot assign value to non variable node");
                }

                _tokenizer.NextToken();
                var rhs = ParseTernary();
                lhs = new NodeAssignment(varNode.VariableName, rhs);
            }
        }

        private INode ParseTernary()
        {
            var lhs = ParseLogicalOr();

            while (true)
            {
                Func<object, object, object, object> functionCall = null;
                switch (_tokenizer.Token)
                {
                    case Token.TernaryIf:
                        functionCall = BasicOperations.TernaryIf;
                        break;
                }

                if (functionCall == null)
                {
                    return lhs;
                }

                _tokenizer.NextToken();

                var middle = ParseLogicalOr();

                if (_tokenizer.Token != Token.TernaryElse)
                {
                    throw new SyntaxException("missing ternary else operator");
                }

                _tokenizer.NextToken();

                var rhs = ParseLogicalOr();

                lhs = new NodeTernaryOperation(functionCall, lhs, middle, rhs);
            }
        }

        private INode ParseLogicalOr()
        {
            var lhs = ParseLogicalAnd();

            while (true)
            {
                Func<object, object, object> functionCall = null;
                switch (_tokenizer.Token)
                {
                    case Token.LogicalOr:
                        functionCall = BasicOperations.Or;
                        break;
                }

                if (functionCall == null)
                {
                    return lhs;
                }

                _tokenizer.NextToken();

                var rhs = ParseLogicalAnd();

                lhs = new NodeBinaryOperation(functionCall, lhs, rhs);
            }
        }

        private INode ParseLogicalAnd()
        {
            var lhs = ParseLogical();

            while (true)
            {
                Func<object, object, object> functionCall = null;
                switch (_tokenizer.Token)
                {
                    case Token.LogicalAnd:
                        functionCall = BasicOperations.And;
                        break;
                }

                if (functionCall == null)
                {
                    return lhs;
                }

                _tokenizer.NextToken();

                var rhs = ParseLogical();

                lhs = new NodeBinaryOperation(functionCall, lhs, rhs);
            }
        }

        private INode ParseLogical()
        {
            var lhs = ParseEqualAndNonEqual();

            while (true)
            {
                Func<object, object, object> functionCall = null;
                switch (_tokenizer.Token)
                {
                    case Token.Lesser:
                        functionCall = BasicOperations.LessThan;
                        break;
                    case Token.LesserOrEqual:
                        functionCall = BasicOperations.LessThanOrEqual;
                        break;
                    case Token.Greater:
                        functionCall = BasicOperations.GreaterThan;
                        break;
                    case Token.GreaterOrEqual:
                        functionCall = BasicOperations.GreaterThanOrEqual;
                        break;
                }

                if (functionCall == null)
                {
                    return lhs;
                }

                _tokenizer.NextToken();

                var rhs = ParseEqualAndNonEqual();

                lhs = new NodeBinaryOperation(functionCall, lhs, rhs);
            }
        }

        private INode ParseEqualAndNonEqual()
        {
            var lhs = ParseAddSubtract();

            while (true)
            {
                Func<object, object, object> functionCall = null;
                switch (_tokenizer.Token)
                {
                    case Token.Equal:
                        functionCall = BasicOperations.Equal;
                        break;
                    case Token.NotEqual:
                        functionCall = BasicOperations.NotEqual;
                        break;
                }

                if (functionCall == null)
                {
                    return lhs;
                }

                _tokenizer.NextToken();

                var rhs = ParseAddSubtract();

                lhs = new NodeBinaryOperation(functionCall, lhs, rhs);
            }
        }

        private INode ParseAddSubtract()
        {
            var lhs = ParseMultiplyDivide();

            while (true)
            {
                Func<object, object, object> functionCall = null;
                switch (_tokenizer.Token)
                {
                    case Token.Add:
                        functionCall = BasicOperations.Add;
                        break;
                    case Token.Subtract:
                        functionCall = BasicOperations.Subtract;
                        break;
                }

                if (functionCall == null)
                {
                    return lhs;
                }

                _tokenizer.NextToken();

                var rhs = ParseMultiplyDivide();

                lhs = new NodeBinaryOperation(functionCall, lhs, rhs);
            }
        }

        private INode ParseMultiplyDivide()
        {
            var lhs = ParseUnary();

            while (true)
            {
                Func<object, object, object> functionCall = null;
                switch (_tokenizer.Token)
                {
                    case Token.Multiply:
                        functionCall = BasicOperations.Multiply;
                        break;
                    case Token.Divide:
                        functionCall = BasicOperations.Divide;
                        break;
                }

                if (functionCall == null)
                {
                    return lhs;
                }

                _tokenizer.NextToken();

                var rhs = ParseUnary();

                lhs = new NodeBinaryOperation(functionCall, lhs, rhs);
            }
        }

        private INode ParseUnary()
        {
            while (true)
            {
                if (_tokenizer.Token == Token.Add)
                {
                    _tokenizer.NextToken();
                    continue;
                }

                if (_tokenizer.Token == Token.Subtract)
                {
                    _tokenizer.NextToken();

                    var rhs = ParseUnary();

                    return new NodeUnaryOperation(BasicOperations.Negate, rhs);
                }

                if (_tokenizer.Token == Token.LogicalNot)
                {
                    _tokenizer.NextToken();

                    var rhs = ParseUnary();

                    return new NodeUnaryOperation(BasicOperations.Not, rhs);
                }

                return ParseExponentiate();
            }
        }

        private INode ParseExponentiate()
        {
            var lhs = ParseLeaf();

            while (true)
            {
                if (_tokenizer.Token == Token.OpenBracket)
                {
                    _tokenizer.NextToken();

                    var rhs = ParseUnary();

                    if (_tokenizer.Token != Token.CloseBracket)
                    {
                        throw new SyntaxException("Missing close bracket");
                    }

                    _tokenizer.NextToken();

                    lhs = new NodeBinaryOperation(BasicOperations.ElementAccess, lhs, rhs);
                    continue;
                }

                if (_tokenizer.Token == Token.Exponentiate)
                {
                    _tokenizer.NextToken();

                    var rhs = ParseUnary();

                    lhs = new NodeBinaryOperation(BasicOperations.Pow, lhs, rhs);
                    continue;
                }

                return lhs;
            }
        }

        private INode ParseLeaf()
        {
            if (_tokenizer.Token == Token.Number)
            {
                var node = new NodeObject(_tokenizer.Number);
                _tokenizer.NextToken();
                return node;
            }

            if (_tokenizer.Token == Token.String)
            {
                var node = new NodeObject(_tokenizer.StringValue);
                _tokenizer.NextToken();
                return node;
            }

            if (_tokenizer.Token == Token.OpenParens)
            {
                _tokenizer.NextToken();

                var node = ParseTernary();

                if (_tokenizer.Token != Token.CloseParens)
                {
                    throw new SyntaxException("Missing close parenthesis");
                }

                _tokenizer.NextToken();

                return node;
            }

            if (TryParseArray(out var newNode))
            {
                return newNode;
            }

            if (TryParseVariable(out newNode))
            {
                return newNode;
            }

            throw new SyntaxException($"Unexpected token: {_tokenizer.Token}");
        }

        private bool TryParseArray(out INode node)
        {
            if (_tokenizer.Token != Token.OpenBracket)
            {
                node = null;
                return false;
            }

            _tokenizer.NextToken();

            var elements = new List<INode>();

            if (_tokenizer.Token == Token.CloseBracket)
            {
                _tokenizer.NextToken();
                node = new NodeObjectArray(Array.Empty<INode>());
                return true;
            }

            while (true)
            {
                elements.Add(ParseTernary());

                if (_tokenizer.Token == Token.Comma)
                {
                    _tokenizer.NextToken();
                    continue;
                }

                break;
            }

            if (_tokenizer.Token != Token.CloseBracket)
            {
                throw new SyntaxException("Missing close bracket");
            }

            _tokenizer.NextToken();

            node = new NodeObjectArray(elements);
            return true;
        }

        private bool TryParseVariable(out INode node)
        {
            if (_tokenizer.Token != Token.Identifier)
            {
                node = null;
                return false;
            }

            var name = _tokenizer.Identifier;
            _tokenizer.NextToken();

            if (_tokenizer.Token == Token.OpenParens)
            {
                _tokenizer.NextToken();

                var arguments = new List<INode>();
                while (true)
                {
                    if (_tokenizer.Token == Token.CloseParens)
                    {
                        break;
                    }

                    arguments.Add(ParseTernary());

                    if (_tokenizer.Token == Token.Comma)
                    {
                        _tokenizer.NextToken();
                        continue;
                    }

                    break;
                }

                if (_tokenizer.Token != Token.CloseParens)
                {
                    throw new SyntaxException("Missing close parenthesis");
                }

                _tokenizer.NextToken();

                node = new NodeFunctionCall(name, arguments);
                return true;
            }

            // Element access
            if (_tokenizer.Token == Token.OpenBracket)
            {
                _tokenizer.NextToken();

                var argument = ParseTernary();

                if (_tokenizer.Token != Token.CloseBracket)
                {
                    throw new SyntaxException("Missing closing bracket");
                }

                _tokenizer.NextToken();

                node = new NodeBinaryOperation(BasicOperations.ElementAccess, new NodeVariable(name), argument);
                return true;
            }

            if (name == "true")
            {
                node = new NodeObject(true);
                return true;
            }

            if (name == "false")
            {
                node = new NodeObject(false);
                return true;
            }

            // Variable
            node = new NodeVariable(name);
            return true;
        }


        #region Convenience Helpers
        
        public static INode ParseSingle(string str)
        {
            return Parse(str).Single();
        }

        public static INode[] Parse(string str)
        {
            using (var reader = new StringReader(str))
            {
                return Parse(new Tokenizer(reader));
            }
        }

        public static INode[] Parse(Tokenizer tokenizer)
        {
            var parser = new Parser(tokenizer);
            return parser.ParseExpression();
        }

        #endregion
    }
}
