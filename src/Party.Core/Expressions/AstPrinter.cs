namespace Party.Core.Expressions;

public sealed class AstPrinter
{
    public static void Execute(IExprVisitor<string> visitor, params IAstComponent[] components)
    {
        foreach (var component in components)
        {
            Console.WriteLine(component.Accept(visitor));
        }
    }
}
