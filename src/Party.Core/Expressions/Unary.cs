namespace Party.Core.Expressions;

/// <summary>
/// Represents a unary expression.
/// </summary>
/// <param name="opr">The unary operator token.</param>
/// <param name="right">The expression.</param>
public sealed class Unary(Token opr, Expr right) : Expr
{
    public Token Operator { get; } = opr;

    public Expr Right { get; } = right;

    public override string Accept(IExprVisitor<string> visitor)
        => visitor.VisitUnary(this);
}
