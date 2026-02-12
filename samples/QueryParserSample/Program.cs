// See https://aka.ms/new-console-template for more information


using Microsoft.OData.Query;
using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Parser;
using QueryParserSample;
using System.Runtime.CompilerServices;

//DateOnly date = new DateOnly(2025, 10, 27);
//TimeSpan duration = new TimeSpan(3, 0, 0, 0); // 3 days
////DateOnly newDate = date + duration;

//DateOnly ndate = date;

TimeSpan t1 = new TimeSpan(1, 0, 0); // 1 hour
TimeSpan t2 = new TimeSpan(0, 30, 0); // 30 minutes
TimeSpan sum = t1 + t2; // sum is 1 hour and 30 minutes

DateTimeOffset date1 = new DateTimeOffset(2025, 2, 5, 10, 0, 0, new TimeSpan(-5, 0, 0)); // 10:00 AM UTC-5
TimeSpan duration = new TimeSpan(2, 30, 0); // 2 hours, 30 minutes

// Addition (using the + operator)
DateTimeOffset date2 = date1 + duration;

QueryParser parser = new QueryParser();

QueryParserContext context = new QueryParserContext(typeof(Customer));

while (true)
{
    Console.Write("Please input your OData query string (e.g. $filter=Id eq 1). Type #cls to clear the console, type #q to quit.\n");
    Console.Write(">");
    string line = Console.ReadLine();

    if (line == "#cls")
    {
        Console.Clear();
        continue;
    }

    if (line == "#q")
    {
        Console.Clear();
        Console.WriteLine("Bye!");
        break;
    }

    if (string.IsNullOrEmpty(line))
    {
        line = "$compute=Weight mul 8 as Ratio,Price div 2.0 as HalfPrice&$filter=Name eq 'Sam'&$index=-98";
    }

    ConsoleColor color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Green;

    try
    {
        // $compute=Weight mul 8 as Ratio
        QueryParsedResult result = await parser.ParseAsync(line.AsMemory(), context);

        ShowResult(result);
    }
    catch (QueryParserException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
    }

    Console.ForegroundColor = color;
}

void ShowResult(QueryParsedResult result)
{
    ShowFilter(result.Filter);
    ShowCompute(result.Compute);

    if (result.Count.HasValue)
    {
        Console.WriteLine($"   |- $count: {result.Count.Value}");
    }

    if (result.Top.HasValue)
    {
        Console.WriteLine($"   |- $top: {result.Top.Value}");
    }

    if (result.Skip.HasValue)
    {
        Console.WriteLine($"   |- $skip: {result.Skip.Value}");
    }

    if (result.Index.HasValue)
    {
        Console.WriteLine($"   |- $index: {result.Index.Value}");
    }
}

void ShowFilter(FilterClause filter)
{
    if (filter == null)
    {
        return;
    }

    Console.WriteLine($"  |- $filter: ");

    new QueryNodeVisitor().Visit(filter.Expression, 4);
}

void ShowCompute(ComputeClause compute)
{
    if (compute == null || compute.Count == 0)
    {
        return;
    }

    Console.WriteLine($"  |- $compute: '{compute.Count}'");

    foreach (var item in compute)
    {
        new QueryNodeVisitor().Visit(item.Expression, 4);

        Console.WriteLine($"        |- Alias = {item.Alias}\n");
    }
}



public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int Weight { get; set; }

    public double Price { get; set; }
}