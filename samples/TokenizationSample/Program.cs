// See https://aka.ms/new-console-template for more information

using Microsoft.OData.Query.Lexers;
using TokenizerSample;

OtherTests.Test();

LexerOptions options = new LexerOptions();

bool isIgnoreWhiteSpace = true;
while (true)
{
    Console.Write(">");
    string line = Console.ReadLine();

    if (line == "#cls")
    {
        Console.Clear();
        continue;
    }

    if (line == "#ws")
    {
        isIgnoreWhiteSpace = !isIgnoreWhiteSpace;
        Console.WriteLine(isIgnoreWhiteSpace ? "Ignore whitespace now!" : "Not ignore whitespace now!");
        continue;
    }
    options.IgnoreWhitespace = isIgnoreWhiteSpace;

    if (line == "#q")
    {
        Console.Clear();
        break;
    }

    ConsoleColor color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Green;

    IExpressionLexer lexer = new ExpressionLexer(line, options);

    try
    {
        while (lexer.NextToken())
        {
            ExpressionToken token = lexer.CurrentToken;
            Console.WriteLine(" ├──" + token.ToString());
        }
    }
    catch (ExpressionLexerException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
    }

    Console.ForegroundColor = color;
}

