//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

public class FilterOptionParser : QueryOptionParser, IFilterOptionParser
{
    private IOTokenizerFactory _tokenizerFactory;

    public FilterOptionParser(IOTokenizerFactory factory)
    {
        _tokenizerFactory = factory;
    }

    public QueryToken ParseFilter(string filter, QueryOptionParserContext context)
    {
        IOTokenizer tokenizer = _tokenizerFactory.CreateTokenizer(filter, OTokenizerContext.Default);
        tokenizer.NextToken(); // move to first token

        return ParseExpression(tokenizer, context);
    }
}
