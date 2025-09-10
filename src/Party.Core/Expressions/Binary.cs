namespace Party.Core.Expressions;

/// <summary>
/// Represents a binary expression.
/// </summary>
/// <param name="left">The left-hand-side expression.</param>
/// <param name="opr">The binary operator.</param>
/// <param name="right">The right-hand-side expression.</param>
public sealed class Binary(Expr left, Token opr, Expr right) : Expr
{
    public Expr Left { get; } = left;

    public Token Operator { get; } = opr;

    public Expr Right { get; } = right;

    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitBinary(this);
}
