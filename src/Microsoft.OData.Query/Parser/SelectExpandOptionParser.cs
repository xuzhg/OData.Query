//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

public abstract class SelectExpandOptionParser : QueryOptionParser
{
    /// <summary>
    /// Uses the ExpressionLexer to visit the next ExpressionToken, and delegates parsing of segments, type segments, identifiers,
    /// and the star token to other methods.
    /// </summary>
    /// <param name="previousSegment">Previously parsed PathSegmentToken, or null if this is the first token.</param>
    /// <param name="allowRef">Whether the $ref operation is valid in this token.</param>
    /// <returns>A parsed PathSegmentToken representing the next segment in this path.</returns>
    protected PathSegmentToken ParseSegment(IOTokenizer tokenizer, PathSegmentToken previousSegment, bool isSelect, bool allowRef)
    {
        OToken currentToken = tokenizer.CurrentToken;
        if (currentToken.Text.StartsWith("$", StringComparison.Ordinal)
            && (!allowRef || !currentToken.Text.Equals("$ref", StringComparison.Ordinal))
            && !currentToken.Text.Equals("$count", StringComparison.Ordinal))
        {
            throw new Exception("ODataErrorStrings.UriSelectParser_SystemTokenInSelectExpand(this.lexer.CurrentToken.Text, this.lexer.ExpressionText)");
        }

        // Some check here to throw exception, prop1/*/prop2 and */$ref/prop and prop1/$count/prop2 will throw exception, all are $expand cases.
        if (!isSelect)
        {
            if (previousSegment != null && previousSegment.Identifier == "*" && !tokenizer.GetIdentifier().Equals("$ref", StringComparison.Ordinal))
            {
                // Star can only be followed with $ref. $count is not supported with star as expand option
                throw new Exception("ODataErrorStrings.ExpressionToken_OnlyRefAllowWithStarInExpand");
            }
            else if (previousSegment != null && previousSegment.Identifier == "$ref")
            {
                // $ref should not have more property followed.
                throw new Exception("ODataErrorStrings.ExpressionToken_NoPropAllowedAfterRef");
            }
            else if (previousSegment != null && previousSegment.Identifier == "$count")
            {
                // $count should not have more property followed. e.g $expand=NavProperty/$count/MyProperty
                throw new Exception("ODataErrorStrings.ExpressionToken_NoPropAllowedAfterDollarCount");
            }
        }

        if (currentToken.Text.Equals("$count", StringComparison.Ordinal) && isSelect)
        {
            // $count is not allowed in $select e.g $select=NavProperty/$count
            throw new Exception("ODataErrorStrings.ExpressionToken_DollarCountNotAllowedInSelect");
        }

        ReadOnlySpan<char> propertyName;

        //if (this.lexer.PeekNextToken().Kind == OTokenKind.Dot)
        //{
        //    propertyName = this.lexer.ReadDottedIdentifier(this.isSelect);
        //}
        //else if (this.lexer.CurrentToken.Kind == OTokenKind.Star)
        //{
        //    // "*/$ref" is supported in expand
        //    if (this.lexer.PeekNextToken().Kind == OTokenKind.Slash && isSelect)
        //    {
        //        throw new Exception("ODataErrorStrings.ExpressionToken_IdentifierExpected(this.lexer.Position)");
        //    }
        //    else if (previousSegment != null && !isSelect)
        //    {
        //        // expand option like "customer?$expand=VIPCustomer/*" is not allowed as specification does not allowed any property before *.
        //        throw new Exception("ODataErrorStrings.ExpressionToken_NoSegmentAllowedBeforeStarInExpand");
        //    }

        //    propertyName = tokenizer.CurrentToken.Text;
        //    tokenizer.NextToken();
        //}
        //else
        //{
        //    propertyName = tokenizer.GetIdentifier();
        //    tokenizer.NextToken();
        //}

        // return new NonSystemToken(propertyName, null, previousSegment);
        throw new NotImplementedException();
    }
}
