//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Xml;

namespace Microsoft.OData.Query.Commons;

internal static class StringUtils
{
    /// <summary>
    /// Converts a string to a GUID value.
    /// </summary>
    /// <param name="text">String text to convert.</param>
    /// <param name="targetValue">After invocation, converted value.</param>
    /// <returns>true if the value was converted; false otherwise.</returns>
    internal static bool TryConvertToGuid(string text, out Guid targetValue)
    {
        try
        {
            // ABNF shows guidValue defined as
            // guidValue = 8HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 4HEXDIG "-" 12HEXDIG
            // which comes to length of 36
            string trimmedText = text.Trim();
            if (trimmedText.Length != 36 || trimmedText.IndexOf('-') != 8)
            {
                targetValue = default(Guid);
                return false;
            }


            targetValue = XmlConvert.ToGuid(text);
            return true;
        }
        catch (FormatException)
        {
            targetValue = default(Guid);
            return false;
        }
    }
}
