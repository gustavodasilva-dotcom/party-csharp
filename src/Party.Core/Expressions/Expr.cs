namespace Party.Core.Expressions;

/// <summary>
/// The base expression class that represents all expressions.
/// </summary>
public abstract class Expr : IAstComponent
{
    public abstract string Accept(IExprVisitor<string> visitor);
}
