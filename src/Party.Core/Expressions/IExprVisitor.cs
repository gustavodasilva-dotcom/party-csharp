namespace Party.Core.Expressions;

public interface IExprVisitor<T>
{
    T VisitTernary(Ternary expr);

    T VisitBinary(Binary expr);

    T VisitGrouping(Grouping expr);

    T VisitLiteral(Literal expr);

    T VisitUnary(Unary expr);
}
