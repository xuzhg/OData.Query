using System.Diagnostics.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApplySample.Wrappers;

public abstract class PropertyContainer
{
    public PropertyContainer Next { get; set; }

    public void ToDictionary(IDictionary<string, object> container)
    {
        Contract.Assert(container != null);
        ToDictionaryCore(container);
    }

    public abstract void ToDictionaryCore(IDictionary<string, object> dictionary);
}

public class NamedProperty<T> : PropertyContainer
{
    public string Name { get; set; }

    public T Value { get; set; }

    public override void ToDictionaryCore(IDictionary<string, object> dictionary)
    {
        Contract.Assert(dictionary != null);

        if (Name != null)
        {
            dictionary.Add(Name, GetValue());
        }
    }

    public virtual object GetValue()
    {
        return Value;
    }
}
