namespace Party.Core.Expressions;

public interface IAstComponent
{
    string Accept(IExprVisitor<string> visitor);
}
