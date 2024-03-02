namespace ApplySample.Wrappers;

/// <summary>
/// Represents a container class that contains properties that are grouped by using $apply.
/// </summary>
public abstract class DynamicTypeWrapper
{
    /// <summary>
    /// Gets values stored in the wrapper
    /// </summary>
    public abstract Dictionary<string, object> Values { get; }

    public bool TryGetPropertyValue(string propertyName, out object value)
    {
        return Values.TryGetValue(propertyName, out value);
    }
}
