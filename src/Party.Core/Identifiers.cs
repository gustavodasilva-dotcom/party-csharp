namespace Party.Core;

internal static class Identifiers
{
    public static string Class = "class";

    public static string Else = "else";

    public static string False = "false";

    public static string For = "for";

    public static string Function = "function";

    public static string If = "if";

    public static string Null = "null";

    public static string Print = "print";

    public static string Return = "return";

    public static string Super = "super";

    public static string This = "this";

    public static string True = "true";

    public static string Var = "var";

    public static string While = "while";

    public static Dictionary<string, TokenTypes> Keywords = new()
    {
        { Class, TokenTypes.CLASS },
        { Else, TokenTypes.ELSE },
        { False, TokenTypes.FALSE },
        { For, TokenTypes.FOR },
        { Function, TokenTypes.FUNCTION },
        { If, TokenTypes.IF },
        { Null, TokenTypes.NULL },
        { Print, TokenTypes.PRINT },
        { Return, TokenTypes.RETURN },
        { Super, TokenTypes.SUPER },
        { This, TokenTypes.THIS },
        { True, TokenTypes.TRUE },
        { Var, TokenTypes.VAR },
        { While, TokenTypes.WHILE }
    };
}
