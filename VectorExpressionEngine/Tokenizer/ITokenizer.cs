﻿namespace VectorExpressionEngine
{
    public interface ITokenizer
    {
        Token Token { get; }

        double Number { get; }

        string Identifier { get; }

        string StringValue { get; }

        void NextToken();
    }
}
