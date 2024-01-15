﻿//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Lexical tokenization is conversion of a text into meaningful lexical tokens.
/// </summary>
public interface IOTokenizer
{
    /// <summary>
    /// Gets the current Token in this tokenization lexer.
    /// </summary>
    OToken CurrentToken { get; }

    /// <summary>
    /// Reads the next token, advancing the tokenization Lexer.
    /// </summary>
    /// <returns>Boolean value indicating reading the next token succussed or not.</returns>
    bool NextToken();
}
