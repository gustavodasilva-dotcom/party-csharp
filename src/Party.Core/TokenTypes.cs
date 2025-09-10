namespace Party.Core;

public enum TokenTypes
{
    // Single-character tokens.
    OPEN_PAREN,
    CLOSE_PAREN,
    OPEN_CURLY,
    CLOSE_CURLY,
    COMMA,
    DOT,
    MINUS,
    PLUS,
    COLON,
    SEMICOLON,
    FORWARD_SLASH,
    STAR,
    PERCENT,

    // One or two character tokens.
    EXCLAMATION,
    EXCLAMATION_EQUAL,
    EQUAL,
    EQUAL_EQUAL,
    GREATER,
    GREATER_EQUAL,
    LESS,
    LESS_EQUAL,
    QUESTION,

    // Literals.
    IDENTIFIER,
    STRING,
    NUMBER,

    // Keywords
    AND,
    CLASS,
    ELSE,
    FALSE,
    FUNCTION,
    FOR,
    IF,
    NULL,
    OR,
    PRINT,
    RETURN,
    SUPER,
    THIS,
    TRUE,
    VAR,
    WHILE,

    EOF
}
