

namespace ApplySample.Models;

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string SSN { get; set; }

    public string[] Titles { get; set; }

    public IList<Sale> Sales { get; set; }
}

public class Sale
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Amount { get; set; }

    public Product Prod { get; set; }

    public override string ToString()
    {
        return $"({Id}): {Name}";
    }
}

public class Product
{
    public Category Cat { get; set; }

    public Category SubCat { get; set; }
}

public class Category
{
    public string Title { get; set; }
}
