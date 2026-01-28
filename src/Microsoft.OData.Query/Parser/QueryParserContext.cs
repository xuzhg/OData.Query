//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Tokenizations;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// Settings used by <see cref="ODataUriParser"/>.
/// </summary>
public class ODataQueryParserOptions
{ }

public class QueryParserContext<T> : QueryParserContext
{
    public QueryParserContext()
        : base (typeof(T))
    {
    }

    public QueryParserContext(QueryParserSettings settings)
        : base(typeof(T), settings)
    {
    }

    public QueryParserContext(IServiceProvider serviceProvider)
        : base(typeof(T), serviceProvider)
    {
    }
}

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
        : this (elementType, new QueryParserSettings())
    {
    }

    public QueryParserContext(Type elementType, QueryParserSettings settings)
    {
        ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));

        ImplicitRangeVariable = new RangeVariable("$it", elementType);
        RangeVariables = new Stack<RangeVariable>();
        RangeVariables.Push(ImplicitRangeVariable);

        Resolver = new MetadataResolver();
    }

    public QueryParserContext(Type elementType, IServiceProvider serviceProvider)
    {
        ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        Settings = ServiceProvider.GetService<QueryParserSettings>() ?? new QueryParserSettings();
    }

    /// <summary>
    /// The settings for this instance of <see cref="QueryParserContext"/>. Refer to the documentation for the individual properties of <see cref="QueryParserSettings"/> for more information.
    /// </summary>
    public QueryParserSettings Settings { get; }

    /// <summary>
    /// The optional dependency injection container to get related services for query option parsing.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    public IFilterOptionParser FilterParser
    {
        get
        {
            if (ServiceProvider != null)
            {
                return ServiceProvider.GetService<IFilterOptionParser>();
            }

            return FilterOptionParser.Default;
        }
    }

    internal IFilterParser GetOrCreateFilterParser()
    {
        if (ServiceProvider != null)
        {
            return ServiceProvider.GetService<IFilterParser>();
        }


        return new FilterParser();
    }

    public bool IgnoreUnknownQuery { get; set; }

    public bool EnableCaseInsensitive
    {
        get => _tokenizerContext.EnableCaseInsensitive;
        set => _tokenizerContext.EnableCaseInsensitive = value;
    }

    /// <summary>
    /// Gets or Sets an option whether no dollar query options is enabled.
    /// If it is enabled, the '$' prefix of system query options becomes optional.
    /// For example, "select" and "$select" are equivalent in this case.
    /// </summary>
    public bool EnableNoDollarPrefix { get; set; } = true;

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
            throw new QueryParserException(Error.Format(SRResources.QueryTokenizer_RangeVariableAlreadyDeclared, parameter));
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