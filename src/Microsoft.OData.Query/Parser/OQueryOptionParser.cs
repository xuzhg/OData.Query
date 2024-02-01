//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

public class OQueryOptionParser : IOQueryOptionParser
{
    private readonly IOTokenizerFactory _tokenizerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OQueryOptionParser" /> class.
    /// </summary>
    /// <param name="tokenizerFactory">The required tokenizer.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OQueryOptionParser(IOTokenizerFactory tokenizerFactory)
    {
        _tokenizerFactory = tokenizerFactory ?? throw new ArgumentNullException(nameof(tokenizerFactory));
    }

    public virtual QueryToken ParseQuery(string query, QueryOptionParserContext context)
    {
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
