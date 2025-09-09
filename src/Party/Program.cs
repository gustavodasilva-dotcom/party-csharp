using Party.Constants;
using Party.Core;

namespace Party;

internal class Program
{
    private static string PartyExtensionFile = ".pa";

    private static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: party <script.pa>");
            Environment.Exit(ExitCodes.Usage);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunRepl();
        }
    }

    private static void RunFile(string filePath)
    {
        var file = new FileInfo(filePath);
        if (!file.Exists)
        {
            Console.WriteLine("File does not exist.");
            Environment.Exit(ExitCodes.Usage);
        }

        if (file.Extension != PartyExtensionFile)
        {
            Console.WriteLine("Invalid Party file.");
            Environment.Exit(ExitCodes.Usage);
        }

        var input = File.ReadAllText(file.FullName);

        RunInput(input);

        if (Diagnostics.HadError)
            Environment.Exit(ExitCodes.DataErr);
    }

    private static void RunRepl()
    {
        while (true)
        {
            Console.Write("> ");

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) break;

            RunInput(input);

            Diagnostics.ResetHadError();
        }
    }

    private static void RunInput(string source)
    {
        var lexer = new Lexer(source);

        var tokens = lexer.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
