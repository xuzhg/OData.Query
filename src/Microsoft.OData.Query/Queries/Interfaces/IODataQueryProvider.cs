using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Query.Queries.Interfaces
{
    public interface IODataQueryProvider
    {
        IQueryable ApplyTo(IQueryable query, string rawQuery, ODataQuerySettings settings);
    }
}
