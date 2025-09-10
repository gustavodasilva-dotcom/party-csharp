namespace Party.Core;

public sealed class Lexer(string source)
{
    private readonly string _source = source;
    private readonly List<Token> _tokens = [];

    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenTypes.EOF, "", _line));

        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();

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
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                else if (Match(Lexemes.Star))
                    ScanBlockComment();
                else
                    AddToken(TokenTypes.FORWARD_SLASH);
                break;
            case Lexemes.WhiteSpace:
            case Lexemes.CarriageReturn:
            case Lexemes.Tab:
                break;
            case Lexemes.LineFeed:
                _line++;
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
                    Diagnostics.EmitError(_line, "Unexpected character.");
                break;
        }
    }

    private void ScanBlockComment()
    {
        while (Peek() != Lexemes.Star || PeekNext() != Lexemes.ForwardSlash)
        {
            if (Peek() == Lexemes.LineFeed) _line++;

            // Safely unwinds the recursion (and the loop) at the end of the file
            // in case the end comment mark (*/) is missing.
            if (IsAtEnd()) break;

            var c = Advance();

            // Recursively scans nested block comments.
            if (c == Lexemes.ForwardSlash && Peek() == Lexemes.Star)
                ScanBlockComment();
        }

        if (Peek() != Lexemes.Star && PeekNext() != Lexemes.ForwardSlash)
        {
            Diagnostics.EmitError(_line, "Unterminated block comment.");
            return;
        }

        // Consumes remaining star (*).
        Advance();

        // Consumes remaining forward slash (/).
        Advance();
    }

    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        var text = _source[_start.._current];

        if (!Identifiers.Keywords.TryGetValue(text, out var type))
            type = TokenTypes.IDENTIFIER;

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
            TokenTypes.NUMBER, double.Parse(_source[_start.._current]));
    }

    private void ScanString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;

            Advance();
        }

        if (IsAtEnd())
        {
            Diagnostics.EmitError(_line, "Unterminated string.");
            return;
        }

        Advance();

        // Because it has to cut off the last double quote ("), this length subtraction is -2.
        var value = _source.Substring(_start + 1, _current - _start - 2);

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
        if (_current + 1 >= _source.Length) return '\0';

        return _source[_current + 1];
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';

        return _source[_current];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_current] != expected) return false;

        _current++;

        return true;
    }

    private char Advance()
    {
        _current++;

        return _source[_current - 1];
    }

    private void AddToken(TokenTypes type) => AddToken(type, null);

    private void AddToken(TokenTypes type, object? literal)
    {
        var text = _source[_start.._current];

        _tokens.Add(new Token(type, text, literal, _line));
    }

    private bool IsAtEnd() => _current >= _source.Length;
}
