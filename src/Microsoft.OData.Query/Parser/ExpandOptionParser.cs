//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

public class ExpandOptionParser : SelectExpandOptionParser, IExpandOptionParser
{
    private IOTokenizerFactory _tokenizerFactory;

    public ExpandOptionParser(IOTokenizerFactory factory)
    {
        _tokenizerFactory = factory;
    }

    public virtual ExpandToken ParseExpand(string expand, QueryOptionParserContext context)
    {
       // return ParseExpressionText(expand);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parse the expand option in the select/expand option text.
    /// </summary>
    /// <param name="pathToken">The path segment token</param>
    /// <returns>The expand option for select/expand</returns>
    protected virtual ExpandToken ParseInnerExpand(IOTokenizer tokenizer, PathSegmentToken pathToken)
    {
        //// advance to the equal sign
        //tokenizer.NextToken();

        //string expandText = UriParserHelper.ReadQueryOption(this.lexer);

        //IEdmStructuredType targetStructuredType = null;
        //if (this.resolver != null && this.parentStructuredType != null)
        //{
        //    var parentProperty = this.resolver.ResolveProperty(parentStructuredType, pathToken.Identifier);

        //    // it is a property, need to find the type.
        //    // Like $expand=Friends($expand=Trips($expand=*)), when expandText becomes "Trips($expand=*)",
        //    // find navigation property Trips of Friends, then get Entity type of Trips.
        //    // or for select query like: $select=Address($expand=City)
        //    if (parentProperty != null)
        //    {
        //        targetStructuredType = parentProperty.Type.ToStructuredType();
        //    }
        //}

        //SelectExpandParser innerExpandParser = new SelectExpandParser(
        //    resolver,
        //    expandText,
        //    targetStructuredType,
        //    this.maxRecursionDepth - 1,
        //    this.enableCaseInsensitiveBuiltinIdentifier,
        //    this.enableNoDollarQueryOptions);

        //return innerExpandParser.ParseExpand();
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parse the level option in the expand option text.
    /// </summary>
    /// <returns>The level option for expand in long type</returns>
    protected virtual long? ParseInnerLevel(IOTokenizer tokenizer)
    {
        //long? levelsOption = null;

        //// advance to the equal sign
        //tokenizer.NextToken();
        //string levelsText = UriParserHelper.ReadQueryOption(this.lexer);
        //long level;

        //if (string.Equals(
        //    ExpressionConstants.KeywordMax,
        //    levelsText,
        //    this.enableCaseInsensitiveBuiltinIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
        //{
        //    levelsOption = long.MinValue;
        //}
        //else if (!long.TryParse(levelsText, NumberStyles.None, CultureInfo.InvariantCulture, out level) || level < 0)
        //{
        //    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidLevelsOption(levelsText));
        //}
        //else
        //{
        //    levelsOption = level;
        //}

        //return levelsOption;
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parse the apply option in the expand option text.
    /// </summary>
    /// <returns>The apply option for expand</returns>
    protected virtual ApplyToken ParseInnerApply(IOTokenizer tokenizer)
    {
        //tokenizer.NextToken();
        //string applyText = UriParserHelper.ReadQueryOption(this.lexer);

        //UriQueryExpressionParser applyParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
        //return applyParser.ParseApply(applyText);
        throw new NotImplementedException();
    }
}
