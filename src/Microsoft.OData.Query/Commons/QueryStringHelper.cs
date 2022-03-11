//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query
{
    internal static class QueryStringHelper
    {
        /// <summary>
        /// Built-in OData query options key words
        /// </summary>
        public static ISet<string> ODataQueryOptions = new HashSet<string>
        {
            "$apply",
            "$compute",
            "$count",
            "$deltatoken",
            "$expand",
            "$filter",
            "$format",
            "$id",
            "$orderby",
            "$select",
            "$skip",
            "$skiptoken",
            "$top",
            "$search",
            "$index",
        };

        /// <summary>
        /// Judge if optionName belongs to OData query option key word based on the case sensitive setting.
        /// </summary>
        /// <param name="optionName">The name of a query option.</param>
        /// <returns>True if optionName is OData query option, vise versa.</returns>
        public static bool IsODataQueryOption(this string optionName)
        {
            return ODataQueryOptions.Contains(optionName);
        }

        /// <summary>
        /// Parse the raw query string into key/value list.
        /// The raw query string should be encoded, at least the '&' is encoded/escaped.
        /// The reason to use IList not IDictionary, because the raw query could have the same key string.
        /// We don't want to throw exception here.
        /// </summary>
        /// <param name="rawQuery">The raw query string.</param>
        /// <returns>null or the parsed list. The output key/value is decoded.</returns>
        public static IList<KeyValuePair<string, string>> ParseRawQuery(string rawQuery)
        {
            if (string.IsNullOrEmpty(rawQuery) || (rawQuery.Length == 1 && rawQuery[0] == '?'))
            {
                return null;
            }

            IList<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            int length = rawQuery.Length;
            int i = rawQuery[0] == '?' ? 1 : 0;

            for (; i < length; ++i)
            {
                int start = i; // start of the one key/value item
                int equalSignIndex = -1;

                while (i < length)
                {
                    char ch = rawQuery[i];
                    if (ch == '=' && equalSignIndex < 0)
                    {
                        equalSignIndex = i;
                    }
                    else if (ch == '&')
                    {
                        break;
                    }

                    ++i;
                }

                string name;
                string value = null;
                if (equalSignIndex >= 0)
                {
                    // name=value
                    name = rawQuery.Substring(start, equalSignIndex - start);
                    value = rawQuery.Substring(equalSignIndex + 1, i - equalSignIndex - 1);
                }
                else
                {
                    // name
                    name = rawQuery.Substring(start, i - start);
                }

                name = Decode(name);
                value = Decode(value);
                result.Add(new KeyValuePair<string, string>(name, value));
            }

            return result;
        }

        private static string Decode(string data)
        {
            if (data is null)
            {
                return data;
            }

            // If the data string is short, check whether we do need the decoding.
            if (data.Length < 16 && data.IndexOfAny(new[] { '%', '+' }) < 0)
            {
                return data.Trim();
            }

            return Uri.UnescapeDataString(data.Replace('+', ' ')).Trim();
        }
    }
}
