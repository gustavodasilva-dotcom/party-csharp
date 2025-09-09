namespace Party.Core.Expressions;

public sealed class Literal(object? value) : Expr
{
    public object? Value { get; } = value;

    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitLiteral(this);
}
