//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

/// <summary>
/// A default parser to parse a $expand clause.
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
    /// Parses the $select expression to a search tree.
    /// </summary>
    /// <param name="select">The $select expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter token.</returns>
    public virtual async ValueTask<ExpandClause> ParseAsync(string select, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(select))
        {
            throw new ArgumentNullException(nameof(select));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IQueryToken token = await Tokenizer.TokenizeAsync(select, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        QueryNode expressionNode = Bind(token, context);

        ExpandClause expandClause = new ExpandClause(/*expressionResultNode, context.ImplicitRangeVariable*/);

        return expandClause;
    }
}