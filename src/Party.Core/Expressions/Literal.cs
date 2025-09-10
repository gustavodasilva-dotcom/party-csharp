namespace Party.Core.Expressions;

/// <summary>
/// Represents a literal expression.
/// </summary>
/// <param name="value">The expression value.</param>
public sealed class Literal(object? value) : Expr
{
    public object? Value { get; } = value;

    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitLiteral(this);
}
