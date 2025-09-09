namespace Party.Core;

public sealed class Lexer(string source)
{
    private readonly string _source = source;
    private readonly List<Token> _tokens = [];

    private int Start = 0;
    private int Current = 0;
    private int Line = 1;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            Start = Current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenTypes.EOF, "", Line));

        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case Lexemes.OpenParen:
                AddToken(TokenTypes.OPEN_PAREN);
                break;
            case Lexemes.CloseParen:
                AddToken(TokenTypes.CLOSE_PAREN);
                break;
            case Lexemes.OpenCurly:
                AddToken(TokenTypes.OPEN_CURLY);
                break;
            case Lexemes.CloseCurly:
                AddToken(TokenTypes.CLOSE_CURLY);
                break;
            case Lexemes.Comma:
                AddToken(TokenTypes.COMMA);
                break;
            case Lexemes.Dot:
                AddToken(TokenTypes.DOT);
                break;
            case Lexemes.Minus:
                AddToken(TokenTypes.MINUS);
                break;
            case Lexemes.Plus:
                AddToken(TokenTypes.PLUS);
                break;
            case Lexemes.Semicolon:
                AddToken(TokenTypes.SEMICOLON);
                break;
            case Lexemes.Star:
                AddToken(TokenTypes.STAR);
                break;
            case Lexemes.Percent:
                AddToken(TokenTypes.PERCENT);
                break;
            case Lexemes.Exclamation:
                AddToken(Match(Lexemes.Equal) ? TokenTypes.EXCLAMATION_EQUAL : TokenTypes.EXCLAMATION);
                break;
            case Lexemes.Equal:
                AddToken(Match(Lexemes.Equal) ? TokenTypes.EQUAL_EQUAL : TokenTypes.EQUAL);
                break;
            case Lexemes.Less:
                AddToken(Match(Lexemes.Equal) ? TokenTypes.LESS_EQUAL : TokenTypes.LESS);
                break;
            case Lexemes.Greater:
                AddToken(Match(Lexemes.Equal) ? TokenTypes.GREATER_EQUAL : TokenTypes.GREATER);
                break;
            case Lexemes.ForwardSlash:
                if (Match(Lexemes.ForwardSlash))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenTypes.FORWARD_SLASH);
                }
                break;
            case Lexemes.WhiteSpace:
            case Lexemes.CarriageReturn:
            case Lexemes.Tab:
                break;
            case Lexemes.LineFeed:
                Line++;
                break;
            case Lexemes.DoubleQuotes:
                ScanString();
                break;
            default:
                if (IsLogicalAnd(c))
                    AddToken(TokenTypes.AND);
                else if (IsLogicalOr(c))
                    AddToken(TokenTypes.OR);
                else if (IsDigit(c))
                    ScanNumber();
                else if (IsAlpha(c))
                    ScanIdentifier();
                else
                    Diagnostics.EmitError(Line, "Unexpected character.");
                break;
        }
    }

    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        var text = _source.Substring(Start, Current - Start);

        if (!Identifiers.Keywords.TryGetValue(text, out TokenTypes type))
        {
            type = TokenTypes.IDENTIFIER;
        }

        AddToken(type);
    }

    private void ScanNumber()
    {
        while (IsDigit(Peek())) Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(
            TokenTypes.NUMBER, double.Parse(_source[Start..Current]));
    }

    private void ScanString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') Line++;

            Advance();
        }

        if (IsAtEnd())
        {
            Diagnostics.EmitError(Line, "Unterminated string.");
            return;
        }

        Advance();

        // Because it has to cut off the last double quote ("), this length subtraction is -2.
        var value = _source.Substring(Start + 1, Current - Start - 2);

        AddToken(TokenTypes.STRING, value);
    }

    private static bool IsAlphaNumeric(char c)
        => IsAlpha(c) || IsDigit(c);

    private static bool IsAlpha(char c)
        => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

    private static bool IsDigit(char c) =>
        c >= '0' && c <= '9';

    private bool IsLogicalAnd(char c) =>
        c == Lexemes.Ampersand && Match(Lexemes.Ampersand);

    private bool IsLogicalOr(char c) =>
        c == Lexemes.Pipe && Match(Lexemes.Pipe);

    private char PeekNext()
    {
        if (Current + 1 >= _source.Length) return '\0';

        return _source[Current + 1];
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';

        return _source[Current];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[Current] != expected) return false;

        Current++;

        return true;
    }

    private char Advance()
    {
        Current++;

        return _source[Current - 1];
    }

    private void AddToken(TokenTypes type) => AddToken(type, null);

    private void AddToken(TokenTypes type, object? literal)
    {
        var text = _source.Substring(Start, Current - Start);

        _tokens.Add(new Token(type, text, literal, Line));
    }

    private bool IsAtEnd() => Current >= _source.Length;
}
