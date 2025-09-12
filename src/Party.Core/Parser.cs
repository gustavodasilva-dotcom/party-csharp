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

    private Expr ParseExpression() => ParseComma();

    private Expr ParseComma()
    {
        Expr expr;

        // Checks if an expression starts with a comma operator.
        if (Match(TokenTypes.COMMA))
        {
            // If so, reports an error.
            Diagnostics.EmitError(Previous().Line, "Expected expression before operator.");

            // Parses the right-hand-side operand and discards it.
            ParseTernary();

            // Symbolizes a poor binary expression.
            expr = new DeadLeaf();
        }
        else
        {
            // The comma operator is a binary expression, so this is also the left-hand-side operand.
            expr = ParseTernary();
        }

        while (Match(TokenTypes.COMMA))
        {
            var opr = Previous();
            var right = ParseTernary();
            expr = new Binary(expr, opr, right);
        }

        return expr;
    }

    private Expr ParseTernary()
    {
        // In a ternary expression, this is also the condition.
        var expr = ParseEquality();

        if (Match(TokenTypes.QUESTION))
        {
            var thenOperator = Previous();
            var thenBranch = ParseExpression();

            var elseOpr = Consume(TokenTypes.COLON, "Expected ':' after expression.");
            var elseBranch = ParseTernary();

            expr = new Ternary(expr, thenOperator, thenBranch, elseOpr, elseBranch);
        }

        return expr;
    }

    private Expr ParseEquality()
    {
        Expr expr;

        // Checks if an expression starts with one of the equality operators.
        if (Match(TokenTypes.EXCLAMATION_EQUAL, TokenTypes.EQUAL_EQUAL))
        {
            // If so, reports an error.
            Diagnostics.EmitError(Previous().Line, "Expected expression before operator.");

            // Parses the right-hand-side operand and discards it.
            ParseComparison();

            // Symbolizes a poor binary expression.
            expr = new DeadLeaf();
        }
        else
        {
            // Left-hand-side operand of a binary expression.
            expr = ParseComparison();
        }

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
        Expr expr;

        // Checks whether an expression starts with one of the following comparison operators.
        if (Match(TokenTypes.GREATER, TokenTypes.GREATER_EQUAL, TokenTypes.LESS, TokenTypes.LESS_EQUAL))
        {
            Diagnostics.EmitError(Previous().Line, "Expected expression before operator.");

            // Parses the right-hand-side operand and discards it.
            ParseTerm();

            // Symbolizes a poor binary expression.
            expr = new DeadLeaf();
        }
        else
        {
            // Left-hand-side operand of a binary expression.
            expr = ParseTerm();
        }

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
        Expr expr;

        // Checks whether an expression starts with one of the following operators.
        if (Match(TokenTypes.FORWARD_SLASH, TokenTypes.STAR))
        {
            Diagnostics.EmitError(Previous().Line, "Expected expression before operator.");

            // Parses the right-hand-side operand and discards it.
            ParseUnary();

            // Symbolizes a poor binary expression.
            expr = new DeadLeaf();
        }
        else
        {
            // Left-hand-side operand of a binary expression.
            expr = ParseUnary();
        }

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
