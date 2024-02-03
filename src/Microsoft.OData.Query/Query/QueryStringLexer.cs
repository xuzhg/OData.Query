//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query;

/// <summary>
/// Parse a query string into its component key and value parts.
/// </summary>
internal class QueryStringLexer
{
    private ReadOnlyMemory<char> _query;
    private ReadOnlyMemory<char> _currentName;
    private ReadOnlyMemory<char> _currentValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringLexer" /> class.
    /// </summary>
    /// <param name="query">The query string.</param>
    public QueryStringLexer(string query)
        : this(query.AsMemory())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringLexer" /> class.
    /// </summary>
    /// <param name="query">The query string.</param>
    public QueryStringLexer(ReadOnlyMemory<char> query)
    {
        _query = query.IsEmpty || query.Span[0] != '?' ? query : query.Slice(1);
        _currentName = default;
        _currentValue = default;
    }

    /// <summary>
    /// Gets the name from this name/value pair in its original encoded form.
    /// </summary>
    public ReadOnlyMemory<char> CurrentName => _currentName;

    /// <summary>
    /// Gets the value from this name/value pair in its original encoded form.
    /// </summary>
    public ReadOnlyMemory<char> CurrentValue => _currentValue;

    /// <summary>
    /// Moves to the next key/value pair in the query string being enumerated.
    /// </summary>
    /// <returns>True if there is another key/value pair, otherwise false.</returns>
    public bool MoveNext()
    {
        while (!_query.IsEmpty)
        {
            // First, split using '&'
            ReadOnlyMemory<char> segment;
            var delimiterIndex = _query.Span.IndexOf('&');
            if (delimiterIndex >= 0)
            {
                segment = _query.Slice(0, delimiterIndex);
                _query = _query.Slice(delimiterIndex + 1);
            }
            else
            {
                segment = _query;
                _query = default;
            }

            // Then, split using '='
            var equalIndex = segment.Span.IndexOf('=');
            if (equalIndex >= 0)
            {
                _currentName = segment.Slice(0, equalIndex);
                _currentValue = segment.Slice(equalIndex + 1);
                return true;
            }
            else if (!segment.IsEmpty)
            {
                _currentName = segment;
                _currentValue = default;
                return true;
            }
        }

        _currentName = default;
        _currentValue = default;
        return false;
    }
}
