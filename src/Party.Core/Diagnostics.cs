namespace Party.Core;

public static class Diagnostics
{
    private static bool s_hadError;

    public static bool HadError => s_hadError;

    public static void ResetHadError() =>
        s_hadError = false;

    public static void EmitError(int line, string message) =>
        Report(line, "", message);

    public static void EmitError(Token token, string message)
    {
        if (token.Type == TokenTypes.EOF)
            Report(token.Line, " at end", message);
        else
            Report(token.Line, " at '" + token.Lexeme + "'", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.WriteLine("[line " + line + "] Error" + where + ": " + message);

        s_hadError = true;
    }
}
