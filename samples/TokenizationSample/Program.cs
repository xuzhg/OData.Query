// See https://aka.ms/new-console-template for more information

using Microsoft.OData.Query.Tokenization;

OTokenizerContext context = new OTokenizerContext();

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
    context.IgnoreWhitespace = isIgnoreWhiteSpace;

    if (line == "#q")
    {
        Console.Clear();
        break;
    }

    ConsoleColor color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Green;

    IOTokenizer tokenizer = new OTokenizer(line, context);

    try
    {
        while (tokenizer.NextToken())
        {
            OToken token = tokenizer.GetCurrentToken();
            Console.WriteLine(" ├──" + token.ToString());
        }
    }
    catch (OTokenizationException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
    }

    Console.ForegroundColor = color;
}

