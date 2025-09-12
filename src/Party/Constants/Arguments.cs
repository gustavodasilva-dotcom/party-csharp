namespace Party.Constants;

/// <summary>
/// Arguments accepted by the CLI tool.
/// </summary>
internal static class Arguments
{
    public const string InteractiveShortOption = "-i";

    public const string InteractiveLongOption = "--interactive";

    public const string HelpShortOption = "-h";

    public const string HelpLongOption = "--help";

    public const string AstShortOption = "-a";

    public const string AstLongOption = "--ast";

    public static readonly string[] Listing =
    [
        InteractiveShortOption,
        InteractiveLongOption,
        HelpShortOption,
        HelpLongOption,
        AstShortOption,
        AstLongOption
    ];
}
