using Party.Constants;
using Party.Core;
using Party.Core.Expressions;

namespace Party;

internal class Program
{
    private static readonly string PartyExtensionFile = ".pa";

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

        if (file.Extension != PartyExtensionFile)
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

        if (args.Any(arg => arg == Arguments.AstShortOption || arg == Arguments.AstLongOption))
        {
            var expr = new Binary(
                new Unary(
                    new Token(TokenTypes.MINUS, "-", 1),
                    new Literal(123)
                ),
                new Token(TokenTypes.STAR, "*", 1),
                new Grouping(new Literal(45.67))
            );

            var visitor = new AstPrinterVisitor();
            AstPrinter.Execute(visitor, expr);
        }
    }
}
