using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Query.Tests
{
    public class ODataQueryBuilderTests
    {
        [Fact]
        public async Task ODataQueryBuilder_BuildThrows_ForNoExpressionGeneratedAsLastStep()
        {
            var builder = new ODataQueryBuilder(null);
            var app = builder.Build();

            var context = new QueryContext();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => app.Invoke(context));

            var expected = "The query reached the end of the pipeline without set a Linq.Expression.";
            Assert.Equal(expected, ex.Message);
        }

        [Fact]
        public void ODataQueryBuilder_BuildReturns_CallableDelegate()
        {
            var builder = new ODataQueryBuilder(null);
            var app = builder.Build();

            var context = new QueryContext();
            context.Expression = Expression.Constant(1);

            app.Invoke(context);
        }

        [Fact]
        public void ODataQueryBuilder_BuildDoCallMiddleware()
        {
            var builder = new ODataQueryBuilder(null);

            var testValue = false;
            builder.Use(context => context =>
            {
                testValue = true;
                // Do not call next
                return Task.CompletedTask;
            });

            var app = builder.Build();
            var context = new QueryContext();

            app.Invoke(context);

            Assert.True(testValue);
        }
    }
}