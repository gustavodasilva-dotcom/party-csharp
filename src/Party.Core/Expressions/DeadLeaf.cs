namespace Party.Core.Expressions;

/// <summary>
/// Represents an expression that could not be parsed.
/// </summary>
public sealed class DeadLeaf : Expr
{
    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitDeadLeaf();
}
