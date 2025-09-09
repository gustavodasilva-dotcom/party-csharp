namespace Party.Core;

public static class Diagnostics
{
    private static bool _hadError;

    public static bool HadError => _hadError;

    public static void ResetHadError() =>
        _hadError = false;

    public static void EmitError(int line, string message) =>
        Report(line, "", message);

    private static void Report(int line, string where, string message)
    {
        Console.WriteLine("[line " + line + "] Error" + where + ": " + message);

        _hadError = true;
    }
}
