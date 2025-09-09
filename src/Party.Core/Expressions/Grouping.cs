namespace Party.Core.Expressions;

public sealed class Grouping(Expr expression) : Expr
{
    public Expr Expression { get; } = expression;

    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitGrouping(this);
}
