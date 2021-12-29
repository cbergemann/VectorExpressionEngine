using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace VectorExpressionEngine
{
    public class Tokenizer : ITokenizer
    {
        public Tokenizer(TextReader reader)
        {
            _reader = reader;
            NextChar();
            NextToken();
        }

        private readonly TextReader _reader;
        private char _currentChar;

        public Token Token { get; private set; }

        public double Number { get; private set; }

        public string Identifier { get; private set; }

        public string StringValue { get; private set; }

        private void NextChar()
        {
            int ch = _reader.Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        public void NextToken()
        {
            while (_currentChar == ' ' || _currentChar == '\t')
            {
                NextChar();
            }

            if (TryParseOperator())
            {
                return;
            }

            if (TryParseNumber())
            {
                return;
            }

            if (TryParseIdentifier())
            {
                return;
            }

            if (TryParseString())
            {
                return;
            }

            throw new SyntaxException($"unexpected character '{_currentChar}'");
        }

        private bool TryParseOperator()
        {
            switch (_currentChar)
            {
                case '\0':
                    Token = Token.EndOfFile;
                    return true;

                case ';':
                    NextChar();
                    Token = Token.EndOfExpression;
                    return true;

                case '\r':
                    NextChar();
                    if (_currentChar == '\n')
                    {
                        NextChar();
                    }
                    Token = Token.EndOfExpression;
                    return true;

                case '\n':
                    NextChar();
                    Token = Token.EndOfExpression;
                    return true;

                case '+':
                    NextChar();
                    Token = Token.Add;
                    return true;

                case '-':
                    NextChar();
                    Token = Token.Subtract;
                    return true;

                case '*':
                    NextChar();
                    Token = Token.Multiply;
                    return true;

                case '/':
                    NextChar();
                    Token = Token.Divide;
                    return true;

                case '^':
                    NextChar();
                    Token = Token.Exponentiate;
                    return true;

                case '(':
                    NextChar();
                    Token = Token.OpenParens;
                    return true;

                case ')':
                    NextChar();
                    Token = Token.CloseParens;
                    return true;

                case ',':
                    NextChar();
                    Token = Token.Comma;
                    return true;

                case '?':
                    NextChar();
                    Token = Token.TernaryIf;
                    return true;

                case ':':
                    NextChar();
                    Token = Token.TernaryElse;
                    return true;

                case '[':
                    NextChar();
                    Token = Token.OpenBracket;
                    return true;

                case ']':
                    NextChar();
                    Token = Token.CloseBracket;
                    return true;

                case '!':
                    NextChar();
                    if (_currentChar == '=')
                    {
                        Token = Token.NotEqual;
                        NextChar();
                        return true;
                    }
                    Token = Token.LogicalNot;
                    return true;

                case '=':
                    NextChar();
                    if (_currentChar == '=')
                    {
                        Token = Token.Equal;
                        NextChar();
                        return true;
                    }
                    Token = Token.Assignment;
                    return true;

                case '|':
                    NextChar();
                    if (_currentChar == '|')
                    {
                        Token = Token.LogicalOr;
                        NextChar();
                        return true;
                    }
                    throw new SyntaxException("unexpected character after '|' sign");

                case '&':
                    NextChar();
                    if (_currentChar == '&')
                    {
                        Token = Token.LogicalAnd;
                        NextChar();
                        return true;
                    }
                    throw new SyntaxException("unexpected character after '&' sign");

                case '<':
                    NextChar();
                    if (_currentChar == '=')
                    {
                        Token = Token.LesserOrEqual;
                        NextChar();
                        return true;
                    }
                    Token = Token.Lesser;
                    return true;

                case '>':
                    NextChar();
                    if (_currentChar == '=')
                    {
                        Token = Token.GreaterOrEqual;
                        NextChar();
                        return true;
                    }
                    Token = Token.Greater;
                    return true;
            }

            return false;
        }

        private static readonly Dictionary<char, char> EscapeCharacters = new Dictionary<char, char>
            {
                { '\\', '\\' },
                { '\'', '\'' },
                { '"', '"' },
                { 'r', '\r' },
                { 'n', '\n' },
                { 't', '\t' },
            };

        private bool TryParseString()
        {
            if (_currentChar != '"' && _currentChar != '\'')
            {
                return false;
            }

            var sb = new StringBuilder();

            var startChar = _currentChar;
            NextChar();

            while (_currentChar != startChar)
            {
                if (_currentChar == '\0')
                {
                    throw new SyntaxException("Unterminated string constant");
                }

                if (_currentChar == '\\')
                {
                    NextChar();
                    if (_currentChar == '\0')
                    {
                        throw new SyntaxException("Unterminated string constant");
                    }

                    if (!EscapeCharacters.TryGetValue(_currentChar, out var replacement))
                    {
                        throw new SyntaxException($"Undefined escape sequence '\\{_currentChar}'");
                    }

                    sb.Append(replacement);
                }
                else
                {
                    sb.Append(_currentChar);
                }

                NextChar();
            }

            NextChar();

            StringValue = sb.ToString();
            Token = Token.String;

            return true;
        }

        private bool TryParseIdentifier()
        {
            if (!char.IsLetter(_currentChar) && _currentChar != '_')
            {
                return false;
            }

            var sb = new StringBuilder();

            while (char.IsLetterOrDigit(_currentChar) || _currentChar == '_')
            {
                sb.Append(_currentChar);
                NextChar();
            }

            // Setup token
            Identifier = sb.ToString();
            Token = Token.Identifier;

            return true;
        }

        private bool TryParseNumber()
        {
            if (!char.IsDigit(_currentChar) && _currentChar != '.')
            {
                return false;
            }

            var sb = new StringBuilder();
            var hasDecimalPoint = false;

            while (char.IsDigit(_currentChar) || (!hasDecimalPoint && _currentChar == '.'))
            {
                sb.Append(_currentChar);
                hasDecimalPoint = hasDecimalPoint || _currentChar == '.';
                NextChar();
            }

            if (_currentChar == 'e' || _currentChar == 'E')
            {
                var hasExponentSign = false;

                sb.Append(_currentChar);
                NextChar();
                while (char.IsDigit(_currentChar) || (!hasExponentSign && (_currentChar == '-' || _currentChar == '+')))
                {
                    sb.Append(_currentChar);
                    hasExponentSign = hasExponentSign || _currentChar == '-' || _currentChar == '+';
                    NextChar();
                }
            }

            try
            {
                Number = double.Parse(sb.ToString(), CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new SyntaxException($"could not parse number: '{sb}'");
            }
            Token = Token.Number;

            return true;
        }
    }
}
