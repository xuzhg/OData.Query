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
/// A default parser to parse a $compute clause.
/// </summary>
public class ComputeOptionParser : QueryOptionParser, IComputeOptionParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComputeOptionParser" /> class.
    /// </summary>
    public ComputeOptionParser()
        : this(ComputeOptionTokenizer.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputeOptionParser" /> class.
    /// </summary>
    /// <param name="tokenizer">The $compute option tokenizer.</param>
    public ComputeOptionParser(IComputeOptionTokenizer tokenizer)
    {
        Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    /// <summary>
    /// Gets the tokenizer.
    /// </summary>
    public IComputeOptionTokenizer Tokenizer { get; }

    /// <summary>
    /// Parses the $compute expression to a search tree.
    /// </summary>
    /// <param name="select">The $compute expression string to parse.</param>
    /// <param name="context">The query parser context.</param>
    /// <returns>The filter token.</returns>
    public virtual async ValueTask<ComputeClause> ParseAsync(string compute, QueryParserContext context)
    {
        if (string.IsNullOrEmpty(compute))
        {
            throw new ArgumentNullException(nameof(compute));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IQueryToken token = await Tokenizer.TokenizeAsync(compute, context.TokenizerContext);
        if (token == null)
        {
            throw new QueryParserException("ODataErrorStrings.MetadataBinder_FilterExpressionNotSingleValue");
        }

        QueryNode expressionNode = Bind(token, context);

        ComputeClause computeClause = new ComputeClause(/*expressionResultNode, context.ImplicitRangeVariable*/);

        return computeClause;
    }
}