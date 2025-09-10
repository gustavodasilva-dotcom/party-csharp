namespace Party.Core;

public sealed class Token(TokenTypes type, string lexeme, object? literal, int line)
{
    public Token(TokenTypes type, string lexeme, int line)
        : this(type, lexeme, null, line)
    {
    }

    public TokenTypes Type { get; } = type;

    public string Lexeme { get; } = lexeme;

    public object? Literal { get; } = literal;

    public int Line { get; } = line;

    public override string ToString()
        => Type + " " + Lexeme + " " + Literal;
}
