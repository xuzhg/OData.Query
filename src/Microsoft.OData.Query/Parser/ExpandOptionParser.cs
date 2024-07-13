//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Clauses;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse an $expand clause.
/// </summary>
public class ExpandOptionParser : QueryOptionParser, IExpandOptionParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandOptionParser" /> class.
    /// </summary>
    public ExpandOptionParser()
        : this(ExpandOptionTokenizer.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandOptionParser" /> class.
    /// </summary>
    /// <param name="tokenizer">The $expand option tokenizer.</param>
    public ExpandOptionParser(IExpandOptionTokenizer tokenizer)
    {
        Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    /// <summary>
    /// Gets the tokenizer.
    /// </summary>
    public IExpandOptionTokenizer Tokenizer { get; }

    /// <summary>
    /// Parses the $expand expression to a search tree.
    /// </summary>
    /// <param name="expand">The $expand expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The expand clause parsed.</returns>
    public virtual async ValueTask<ExpandClause> ParseAsync(string expand, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(expand))
        {
            throw new ArgumentNullException(nameof(expand));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        ExpandToken token = await Tokenizer.TokenizeAsync(expand, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        // TODO: Normalize the expand token

        // $expand=Nav1;Nav2($expand=...;$select=...)
        ExpandClause expandClause = new ExpandClause();
        foreach (ExpandItemToken item in token)
        {
            expandClause.Add(BindExpandItem(item, context));
        }

        return expandClause;
    }

    protected virtual ExpandedItem BindExpandItem(ExpandItemToken expandItem, QueryParserContext context)
    {
        return null;
    }
}