//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Settings used by <see cref="ODataUriParser"/>.
/// </summary>
public class ODataQueryParserOptions
{ }

/// <summary>
/// Query parser context.
/// </summary>
public class QueryParserContext
{
    private QueryTokenizerContext _tokenizerContext = new QueryTokenizerContext();

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParserContext" /> class.
    /// </summary>
    /// <param name="elementType">The target element type for this query.</param>
    public QueryParserContext(Type elementType)
    {
        ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));

        ImplicitRangeVariable = new RangeVariable("$it", elementType);
        RangeVariables = new Stack<RangeVariable>();
        RangeVariables.Push(ImplicitRangeVariable);

        Resolver = new MetadataResolver();
    }

    public bool IgnoreUnknownQuery { get; set; }

    public bool EnableIdentifierCaseSensitive
    {
        get => _tokenizerContext.EnableIdentifierCaseSensitive;
        set => _tokenizerContext.EnableIdentifierCaseSensitive = value;
    }

    /// <summary>
    /// Gets or Sets an option whether no dollar query options is enabled.
    /// If it is enabled, the '$' prefix of system query options becomes optional.
    /// For example, "select" and "$select" are equivalent in this case.
    /// </summary>
    public bool EnableNoDollarSignOption { get; set; } = true;

    public IMetadataResolver Resolver { get; set; }

    /// <summary>
    /// The element type of the odata query.
    /// </summary>
    public Type ElementType { get; }

    /// <summary>
    /// If there is a  $filter or $orderby, then this member holds the reference to the parameter node for the
    /// implicit parameter ($it) for all expressions.
    /// </summary>
    public RangeVariable ImplicitRangeVariable { get; }

    /// <summary>
    /// The dictionary used to store mappings between Any visitor and corresponding segment paths
    /// </summary>
    public Stack<RangeVariable> RangeVariables { get; }

    public RangeVariable GetRangeVariable(string name)
        => RangeVariables.FirstOrDefault(x => x.Name == name);

    internal QueryTokenizerContext TokenizerContext => _tokenizerContext;

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
            throw new OQueryParserException(Error.Format(SRResources.QueryTokenizer_RangeVariableAlreadyDeclared, parameter));
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