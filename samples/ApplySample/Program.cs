// See https://aka.ms/new-console-template for more information


using ApplySample.Models;
using ApplySample.Wrappers;

List<Sale> sales = new List<Sale>
{
    new Sale { Id = 1, Name = "abc", Amount = 1, Prod = new Product { Cat = new Category { Title = "Food" }, SubCat = new Category { Title = "DogFood" } } },
    new Sale { Id = 2, Name = "efg", Amount = 2, Prod = new Product { Cat = new Category { Title = "Meat" }, SubCat = new Category { Title = "Beef" }  } },
    new Sale { Id = 3, Name = "abc", Amount = 5, Prod = new Product { Cat = new Category { Title = "Food" }, SubCat = new Category { Title = "CatFood" }  } },
    new Sale { Id = 4, Name = "abc", Amount = 3, Prod = new Product { Cat = new Category { Title = "Meat" }, SubCat = new Category { Title = "Pork" }  } },
    new Sale { Id = 5, Name = "eft", Amount = 2, Prod = new Product { Cat = new Category { Title = "Veg" }, SubCat = new Category { Title = "Lettce" }  } },
    new Sale { Id = 6, Name = "abc", Amount = 1, Prod = new Product { Cat = new Category { Title = "Food" }, SubCat = new Category { Title = "DogFood" }  } }
};

//public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector);

var groupbyResult = sales.GroupBy(s => s.Prod.Cat.Title);
PrintGroupBy(groupbyResult);

var groupbyResult1 = sales.GroupBy(s => new { s.Prod.Cat.Title, s.Name });
PrintGroupBy(groupbyResult1);

var groupbyResult2 = sales.GroupBy(s => new GroupByWrapper
{
    Container = new NamedProperty<string>
    {
        Name = "Name",
        Value = s.Name,
        Next = null
    }
});
PrintGroupBy(groupbyResult2);

static void PrintGroupBy<TKey, TElement>(IEnumerable<IGrouping<TKey, TElement>> grouping)
{
    Console.WriteLine();
    int groupId = 1;
    foreach (var nameGroup in grouping)
    {
        Console.WriteLine($"{groupId} Key: {nameGroup.Key} -- KEY TYPE: {typeof(TKey).Name}, Element TYPE: {typeof(TElement).Name}");
        foreach (var element in nameGroup)
        {
            Console.WriteLine($"\t{element}");
        }
        ++groupId;
    }
}
