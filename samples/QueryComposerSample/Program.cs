using Microsoft.OData.Query.Binders;
using System.Linq.Expressions;
using System.Linq;

Console.WriteLine("Hello, World!");


// Test
// string query = "$filter=Id eq 1&$compute=Weight mul 8 as Ratio,Price div 2.0 as HalfPrice&$index=-98";
//string query = "$filter=Id eq 1";
string query = "$filter=Name eq 'Sam'";
IQueryCompositer queryComposer = new QueryCompositer(query.AsMemory(), new QueryCompositerContext());
QueryCompositerSettings settings = new QueryCompositerSettings
{

};

IQueryable<Customer> source = new List<Customer> { new Customer { Id = 1, Name = "Sam" }, new Customer { Id = 2, Name = "John" }, new Customer { Id = 3, Name = "Alice" } }.AsQueryable();

IQueryable result = await queryComposer.CompositeAsync(source, settings);

foreach (var item in result.Cast<Customer>().ToList())
{
    Console.WriteLine(item.Name);
}

Expression<Func<int, string>> myExpr = x =>
    x == 1 ? "Result 1" :
    x == 2 ? "Result 2" :
    "Default Other";

var others1 = source.Select(c => c.Id == 1 ? "Result 1" :
    c.Id == 2 ? "Result 2" :
     "Default Result" // The 'ELSE' equivalent
);

var others2 = source.Select<Customer, object>(c => c.Id == 1 ? "Result 1" :
    c.Id == 2 ? "Result 2" :
    new string[] { "Default Result" } // The 'ELSE' equivalent
);

Console.WriteLine("Press any key to exit...");
//var others = source.Select(c => c.Id switch
//{
//    1 => "Result 1",
//    2 => "Result 2",
//    _ => "Default Result" // The 'ELSE' equivalent
//});

public class Customer
{
    public int Id { get; set; }

    public required string Name { get; set; }
}