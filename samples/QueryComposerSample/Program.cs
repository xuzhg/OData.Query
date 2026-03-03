using Microsoft.OData.Query.Binders;

Console.WriteLine("Hello, World!");


// Test
// string query = "$filter=Id eq 1&$compute=Weight mul 8 as Ratio,Price div 2.0 as HalfPrice&$index=-98";
//string query = "$filter=Id eq 1";
string query = "$filter=Name eq 'Sam'";
IQueryCompositer queryComposer = new QueryCompositer(query.AsMemory(), new QueryCompositerContext());
QueryCompositerSettings settings = new QueryCompositerSettings
{

};

IQueryable result = new List<Customer> { new Customer { Id = 1, Name = "Sam" }, new Customer { Id = 2, Name = "John" }, new Customer { Id = 3, Name = "Alice" } }.AsQueryable();

result = await queryComposer.CompositeAsync(result, settings);

foreach (var item in result.Cast<Customer>().ToList())
{
    Console.WriteLine(item.Name);
}

public class Customer
{
    public int Id { get; set; }

    public required string Name { get; set; }
}