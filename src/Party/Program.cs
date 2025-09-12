using Party.Constants;
using Party.Core;
using Party.Core.Expressions;

namespace Party;

internal class Program
{
    private static readonly string s_partyExtensionFile = ".pa";

    private static void Main(string[] args)
    {
#if DEBUG
        RunDebugRepl();
#endif

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: party [OPTIONS] SCRIPT");

            Environment.Exit(ExitCodes.Usage);
        }

        if (args.Any(x => x == Arguments.HelpShortOption || x == Arguments.HelpLongOption))
        {
            RunHelpCommads();

            Environment.Exit(ExitCodes.Success);
        }
        else if (args.Any(x => x == Arguments.InteractiveShortOption || x == Arguments.InteractiveLongOption))
        {
            RunRepl(args);
        }
        else
        {
            RunFile(args);
        }
    }

    private static void RunDebugRepl()
    {
        string? input;

        while (true)
        {
            Console.Write("> ");

            input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) break;

            // Always displays AST flat representation when debugging.
            RunInput(input, Arguments.AstLongOption);
        }
    }

    private static void RunHelpCommads()
    {
        Console.WriteLine("Usage: party [OPTIONS] SCRIPT\n");

        Console.WriteLine("{0}|{1}\t\tShow command line help.",
            Arguments.HelpShortOption, Arguments.HelpLongOption);

        Console.WriteLine("{0}|{1}\tExecute interpreter in interactive mode.",
            Arguments.InteractiveShortOption, Arguments.InteractiveLongOption);

        Console.WriteLine("{0}|{1}\t\tDisplays the abstract syntax tree (AST) of each expression parsed.",
            Arguments.AstShortOption, Arguments.AstLongOption);
    }

    private static void RunFile(params string[] args)
    {
        var file = new FileInfo(args[^1]);

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

        RunInput(input, args);

        if (Diagnostics.HadError) Environment.Exit(ExitCodes.DataErr);
    }

    private static void RunRepl(params string[] args)
    {
        while (true)
        {
            Console.Write("> ");

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) break;

            RunInput(input, args);

            Diagnostics.ResetHadError();
        }
    }

    private static void RunInput(string source, params string[] args)
    {
        var lexer = new Lexer(source);
        var tokens = lexer.ScanTokens();

        var parser = new Parser(tokens);
        var expression = parser.Parse();

        if (args.Any(arg => arg == Arguments.AstShortOption || arg == Arguments.AstLongOption))
        {
            var printer = new AstPrinter();
            printer.Print(expression!);
        }

        if (Diagnostics.HadError) return;
    }
}
