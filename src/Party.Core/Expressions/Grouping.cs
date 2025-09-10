namespace Party.Core.Expressions;

/// <summary>
/// Represents a grouping expression.
/// </summary>
/// <param name="expression">The grouped expression.</param>
public sealed class Grouping(Expr expression) : Expr
{
    public Expr Expression { get; } = expression;

    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitGrouping(this);
}
