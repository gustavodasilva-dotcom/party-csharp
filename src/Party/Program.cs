using Party.Constants;
using Party.Core;
using Party.Core.Expressions;

namespace Party;

internal class Program
{
    private static readonly string s_partyExtensionFile = ".pa";

    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            RunRepl();
        }
        else
        {
            var arg1 = args[0];

            if (!Arguments.Listing.Any(x => x == arg1))
            {
                RunFile(arg1);
            }
            else
            {
                if (arg1 == Arguments.HelpShortOption || arg1 == Arguments.HelpLongOption)
                {
                    Console.WriteLine("Usage: party [OPTIONS] SCRIPT");
                    Environment.Exit(ExitCodes.Success);
                }

                RunFile(args[1], arg1);
            }
        }
    }

    private static void RunFile(string filePath, params string[] arguments)
    {
        var file = new FileInfo(filePath);
        if (!file.Exists)
        {
            Console.WriteLine("File does not exist.");
            Environment.Exit(ExitCodes.Usage);
        }

        if (file.Extension != s_partyExtensionFile)
        {
            Console.WriteLine("Invalid Party file.");
            Environment.Exit(ExitCodes.Usage);
        }

        var input = File.ReadAllText(file.FullName);

        RunInput(input, arguments);

        if (Diagnostics.HadError)
        {
            Environment.Exit(ExitCodes.DataErr);
        }
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

    private static void RunInput(string source, params string[] args)
    {
        var lexer = new Lexer(source);
        var tokens = lexer.ScanTokens();

        var parser = new Parser(tokens);
        var expression = parser.Parse();

        if (Diagnostics.HadError) return;

        if (args.Any(arg => arg == Arguments.AstShortOption || arg == Arguments.AstLongOption))
        {
            var visitor = new AstPrinterVisitor();
            AstPrinter.Execute(visitor, expression!);
        }
    }
}
