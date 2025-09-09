namespace Party.Core.Expressions;

public abstract class Expr : IAstComponent
{
    public abstract string Accept(IExprVisitor<string> visitor);
}
