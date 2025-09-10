namespace Party.Core.Expressions;

/// <summary>
/// Represents a ternary expression.
/// </summary>
/// <param name="condition">The expression condition.</param>
/// <param name="thenOperator">The then operator (?).</param>
/// <param name="thenBranch">The then expression branch.</param>
/// <param name="elseOperator">The else operator (:).</param>
/// <param name="elseBranch">The else expression branch.</param>
public sealed class Ternary(
    Expr condition,
    Token thenOperator,
    Expr thenBranch,
    Token elseOperator,
    Expr elseBranch) : Expr
{
    public Expr Condition { get; } = condition;

    public Token Question { get; } = thenOperator;

    public Expr ThenBranch { get; } = thenBranch;

    public Token Then { get; } = elseOperator;

    public Expr ElseBranch { get; } = elseBranch;

    public override string Accept(IExprVisitor<string> visitor) =>
        visitor.VisitTernary(this);
}
