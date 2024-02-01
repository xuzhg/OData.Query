//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Lexical tokenization is conversion of a text into meaningful lexical tokens.
/// </summary>
public interface IOTokenizerFactory
{
    /// <summary>
    /// Gets the token processed.
    /// </summary>
    IOTokenizer CreateTokenizer(string text, OTokenizerContext context);
}

public class OTokenizerFactory : IOTokenizerFactory
{
    /// <summary>
    /// Gets the token processed.
    /// </summary>
    public IOTokenizer CreateTokenizer(string text, OTokenizerContext context)
    {
        return new OTokenizer(text, context);
    }
}