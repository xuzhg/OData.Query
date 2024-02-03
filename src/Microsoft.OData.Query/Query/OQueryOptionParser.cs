//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Parser;
using Microsoft.OData.Query.Tokenization;
using System;

namespace Microsoft.OData.Query;

public interface IODataQueryOptionParser
{
    /// <summary>
    /// Query string must be in escaped and delimited format with a leading '?' character for a non-empty query.
    /// So, as example, a query string looks like:
    /// "?$filter=Name eq 'Sam'&$select=Id,Name&..."
    /// For each key/value, they should be fully encoded.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    ValueTask<ODataQueryOption> ParseQueryAsync(string query, QueryOptionParserContext context);

    //   QueryToken ParseQuery(QueryOptionParserContext context);
}

public class ODataQueryOptionParser : IODataQueryOptionParser
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ODataQueryOptionParser" /> class.
    /// </summary>
    /// <param name="serviceProvider">The required tokenizer.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ODataQueryOptionParser(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public virtual ValueTask<ODataQueryOption> ParseQueryAsync(string query, QueryOptionParserContext context)
    {
        IOTokenizerFactory _tokenizerFactory = _serviceProvider.GetService(typeof(IOTokenizerFactory)) as IOTokenizerFactory;
        IOTokenizer tokenizer = _tokenizerFactory.CreateTokenizer(query, new OTokenizerContext());

        while (tokenizer.NextToken())
        {
            OToken token = tokenizer.CurrentToken;

            if (token.Kind == OTokenKind.Dollar)
            {
                if (!tokenizer.NextToken())
                {
                    break;
                }

                token = tokenizer.CurrentToken;
            }

            if (token.Kind != OTokenKind.Identifier)
            {
                continue;
            }

            ReadOnlySpan<char> identifier = token.Text.StartsWith("$", StringComparison.Ordinal) ? token.Text[1..] : token.Text;
            if (identifier.Equals("filter", StringComparison.OrdinalIgnoreCase))
            {

            }
         }

        throw new NotImplementedException();
    }
}
