using System.Text;

namespace Party.Core.Expressions;

public sealed class AstPrinterVisitor : IExprVisitor<string>
{
    public string VisitTernary(Ternary expr)
    {
        var builder = new StringBuilder();

        builder
            .Append('(')
            .Append("ternary")
            .Append(' ')
            .Append(expr.Condition.Accept(this))
            .Append(' ')
            .Append('?')
            .Append(' ');

        builder
            .Append(expr.ThenBranch.Accept(this))
            .Append(' ')
            .Append(':')
            .Append(' ');

        builder
            .Append(expr.ElseBranch.Accept(this))
            .Append(')');

        return builder.ToString();
    }

    public string VisitBinary(Binary expr) =>
        Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public string VisitGrouping(Grouping expr) =>
        Parenthesize("group", expr.Expression);

    public string VisitLiteral(Literal expr)
    {
        if (expr.Value is null) return Identifiers.Null;

        return expr.Value.ToString()!;
    }

    public string VisitUnary(Unary expr) =>
        Parenthesize(expr.Operator.Lexeme, expr.Right);

    public string VisitDeadLeaf() => Parenthesize("error");

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var builder = new StringBuilder();

        builder.Append('(').Append(name);

        foreach (var expr in exprs)
        {
            builder.Append(' ');
            builder.Append(expr.Accept(this));
        }

        builder.Append(')');

        return builder.ToString();
    }
}
