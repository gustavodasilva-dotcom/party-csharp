namespace Party.Core.Expressions;

public sealed class Binary(Expr left, Token opr, Expr right) : Expr
{
    public Expr Left { get; } = left;

    public Token Operator { get; } = opr;

    public Expr Right { get; } = right;

    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitBinary(this);
}
