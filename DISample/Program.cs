using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Query.Binders;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


IServiceCollection services = new ServiceCollection();
//services.AddSingleton<ILexer, ExpressionLexer>();
services.AddSingleton<ILexer>(sp => new ExpressionLexer("abc", null));
services.AddSingleton<ILexer, QueryLexer>();

//services.AddSingleton<IComposer, AComposer>();
services.AddSingleton<IComposer, BComposer>();

IServiceProvider sp = services.BuildServiceProvider();

IEnumerable<ILexer> lexers = sp.GetServices<ILexer>();

foreach (var lexer in lexers)
{
    lexer.DoSomething();
}

IEnumerable<IComposer> composers = sp.GetServices<IComposer>();

foreach (var composer in composers)
{
    composer.Compose();
}



public interface IComposer
{
    string Text { get; }

    void Compose();
}

public class AComposer : IComposer
{
    public string Text { get; }
    public AComposer(string text)
    {
        Text = text;
    }
    public void Compose()
    {
        Console.WriteLine($"In AComposer.Compose, Text = {Text}");
    }
}

public class BComposer : IComposer
{
    public string Text { get; set; }

    public void Compose()
    {
        Console.WriteLine($"In BComposer.Compose, Text = {Text}");
    }
}

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