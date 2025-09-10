namespace Party.Constants;

/// <summary>
/// Arguments accepted by the CLI tool.
/// </summary>
internal static class Arguments
{
    public const string HelpShortOption = "-h";

    public const string HelpLongOption = "--help";

    public const string AstShortOption = "-a";

    public const string AstLongOption = "--ast";

    public static readonly string[] Listing =
    [
        HelpShortOption,
        HelpLongOption,
        AstShortOption,
        AstLongOption
    ];
}
