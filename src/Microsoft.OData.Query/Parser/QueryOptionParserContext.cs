//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;

namespace Microsoft.OData.Query.Parser;

public class QueryOptionParserContext
{
    public bool EnableIdentifierCaseSensitive { get; set; } = true;

    public bool EnableNoDollarSignOption { get; set; } = true;

    /// <summary>
    /// Set of parsed parameters
    /// </summary>
    private readonly HashSet<string> _parameters = new HashSet<string>(StringComparer.Ordinal)
    {
        TokenConstants.It,
        TokenConstants.This
    };

    /// <summary>
    /// The current recursion depth.
    /// </summary>
    private int _recursionDepth = 0;

    public void AddParameter(string parameter)
    {
        if (!_parameters.Add(parameter))
        {
            throw new OQueryParserException(Error.Format(SRResources.QueryOptionParser_RangeVariableAlreadyDeclared, parameter));
        }
    }

    public void RemoveParameter(string parameter)
    {
        _parameters.Remove(parameter);
    }

    public bool ContainsParameter(string parameter)
        => _parameters.Contains(parameter);

    public void EnterRecurse()
    {
        ++_recursionDepth;
    }

    public void LeaveRecurse()
    {
        --_recursionDepth;
    }
}