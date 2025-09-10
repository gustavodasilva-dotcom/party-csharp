namespace Party.Core.Expressions;

public sealed class Unary(Token opr, Expr right) : Expr
{
    public Token Operator { get; } = opr;

    public Expr Right { get; } = right;

    public override string Accept(IExprVisitor<string> visitor)
        => visitor.VisitUnary(this);
}
