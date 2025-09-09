namespace Party.Core.Expressions;

public interface IExprVisitor<T>
{
    T VisitBinary(Binary expr);

    T VisitGrouping(Grouping expr);

    T VisitLiteral(Literal expr);

    T VisitUnary(Unary expr);
}
