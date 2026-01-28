using Microsoft.Extensions.DependencyInjection;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


IServiceCollection services = new ServiceCollection();
services.AddSingleton<ILexer, ExpressionLexer>();
services.AddSingleton<ILexer, QueryLexer>();

IServiceProvider sp = services.BuildServiceProvider();

ILexer lexer = sp.GetService<ILexer>();

lexer.DoSomething();

public interface ILexer
{
    void DoSomething();
}

public class ExpressionLexer : ILexer
{
    public ExpressionLexer(string text, object options)
    {
    }

    public ExpressionLexer(ReadOnlyMemory<char> text, object options)
    {
    }

    public void DoSomething()
    {
        Console.WriteLine("In ExpressionLexer.DoSomething");

    }
}

public class QueryLexer : ILexer
{
    public void DoSomething()
    {
        Console.WriteLine("In QueryLexer.DoSomething");
    }
}