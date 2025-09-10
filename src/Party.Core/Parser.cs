using Party.Core.Exceptions;
using Party.Core.Expressions;

namespace Party.Core;

public sealed class Parser(List<Token> tokens)
{
    private readonly List<Token> _tokens = tokens;

    private int _current = 0;

    public Expr? Parse()
    {
        try
        {
            return ParseExpression();
        }
        catch (ParserException)
        {
            return null;
        }
    }

    private Expr ParseExpression() => ParseEquality();

    private Expr ParseEquality()
    {
        // This is also the left-hand-side expression of binary expression.
        var expr = ParseComparison();

        while (Match(TokenTypes.EXCLAMATION_EQUAL, TokenTypes.EQUAL_EQUAL))
        {
            var opr = Previous();
            var right = ParseComparison();
            expr = new Binary(expr, opr, right);
        }

        return expr;
    }

    private Expr ParseComparison()
    {
        // This is also the left-hand-side expression of binary expression.
        var expr = ParseTerm();

        while (Match(TokenTypes.GREATER, TokenTypes.GREATER_EQUAL, TokenTypes.LESS, TokenTypes.LESS_EQUAL))
        {
            var opr = Previous();
            var right = ParseTerm();
            expr = new Binary(expr, opr, right);
        }

        return expr;
    }

    private Expr ParseTerm()
    {
        // This is also the left-hand-side expression of binary expression.
        var expr = ParseFactor();

        while (Match(TokenTypes.MINUS, TokenTypes.PLUS))
        {
            var opr = Previous();
            var right = ParseFactor();
            expr = new Binary(expr, opr, right);
        }

        return expr;
    }

    private Expr ParseFactor()
    {
        var expr = ParseUnary();

        while (Match(TokenTypes.FORWARD_SLASH, TokenTypes.STAR))
        {
            var opr = Previous();
            var right = ParseUnary();
            expr = new Binary(expr, opr, right);
        }

        return expr;
    }

    private Expr ParseUnary()
    {
        if (Match(TokenTypes.EXCLAMATION, TokenTypes.MINUS))
        {
            var opr = Previous();
            var right = ParseUnary();
            return new Unary(opr, right);
        }

        return ParsePrimary();
    }

    private Expr ParsePrimary()
    {
        if (Match(TokenTypes.FALSE)) return new Literal(false);

        if (Match(TokenTypes.TRUE)) return new Literal(true);

        if (Match(TokenTypes.NULL)) return new Literal(null);

        if (Match(TokenTypes.NUMBER, TokenTypes.STRING))
            return new Literal(Previous().Literal);

        if (Match(TokenTypes.OPEN_PAREN))
        {
            var expr = ParseExpression();
            Consume(TokenTypes.CLOSE_PAREN, "Expected ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expected expression.");
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenTypes.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenTypes.CLASS:
                case TokenTypes.FUNCTION:
                case TokenTypes.VAR:
                case TokenTypes.FOR:
                case TokenTypes.IF:
                case TokenTypes.WHILE:
                case TokenTypes.PRINT:
                case TokenTypes.RETURN:
                    return;
            }

            Advance();
        }
    }

    private Token Consume(TokenTypes type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private static ParserException Error(Token token, string message)
    {
        Diagnostics.EmitError(token, message);

        return new ParserException();
    }

    private bool Match(params TokenTypes[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();

                return true;
            }
        }

        return false;
    }

    private bool Check(TokenTypes type)
    {
        if (IsAtEnd()) return false;

        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;

        return Previous();
    }

    private bool IsAtEnd() =>
        Peek().Type == TokenTypes.EOF;

    private Token Peek() =>
        _tokens.ElementAt(_current);

    private Token Previous() =>
        _tokens.ElementAt(_current - 1);
}
