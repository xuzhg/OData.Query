//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Xunit;

namespace Microsoft.OData.Query.Tests.Commons
{
    public class QueryStringHelperTests
    {
        public delegate void ParsedVerifyAction(IList<KeyValuePair<string, string>> pairs);

        public static TheoryData<string, ParsedVerifyAction> RawQueryStrings =>
            new TheoryData<string, ParsedVerifyAction>
            {
                { null, d => Assert.Null(d) },
                { "",   d => Assert.Null(d) },
                { "?",  d => Assert.Null(d) },
                {
                    "?abc", d =>
                    {
                        var keyValuePair = Assert.Single(d);
                        Assert.Equal("abc", keyValuePair.Key);
                        Assert.Null(keyValuePair.Value);
                    }
                },
                {
                    "?abc&abc", d =>
                    {
                        Assert.Equal(2, d.Count);
                        Assert.Collection(d,
                            e =>
                            {
                                Assert.Equal("abc", e.Key);
                                Assert.Null(e.Value);
                            },
                            e =>
                            {
                                Assert.Equal("abc", e.Key);
                                Assert.Null(e.Value);
                            });
                    }
                },
                {
                    "?select=name&$expand=abc",
                    d =>
                    {
                        Assert.Equal(2, d.Count);
                        Assert.Collection(d,
                            e =>
                            {
                                Assert.Equal("select", e.Key);
                                Assert.Equal("name", e.Value);
                            },
                            e =>
                            {
                                Assert.Equal("$expand", e.Key);
                                Assert.Equal("abc", e.Value);
                            });
                    }
                },
                { 
                    "filter=name+eq+'a%26b'",
                    d =>
                    {
                        var keyValuePair = Assert.Single(d);
                        Assert.Equal("filter", keyValuePair.Key);
                        Assert.Equal("name eq 'a&b'", keyValuePair.Value);
                    }
                }
            };

        [Theory]
        [MemberData(nameof(RawQueryStrings))]
        public void ParseRawQuery_ReturnsResult_ForRawQueryString(string rawQuery, ParsedVerifyAction verifyAction)
        {
            IList<KeyValuePair<string, string>> querys = QueryStringHelper.ParseRawQuery(rawQuery);
            verifyAction(querys);
        }

        [Theory]
        [InlineData("?abc")]
        [InlineData("abc")]
        public void ParseRawQuery_ReturnsResult_ForRawQueryStringWithOrWithoutLeadingQuestionMark(string rawQuery)
        {
            IList<KeyValuePair<string, string>> querys = QueryStringHelper.ParseRawQuery(rawQuery);

            var keyValuePair = Assert.Single(querys);
            Assert.Equal("abc", keyValuePair.Key);
            Assert.Null(keyValuePair.Value);
        }
    }
}
