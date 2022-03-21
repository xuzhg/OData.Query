using Microsoft.OData.Query.Queries.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Query.Queries
{
    public class ODataQueryProvider : IODataQueryProvider
    {
        public virtual IQueryable ApplyTo(IQueryable query, string rawQuery, ODataQuerySettings settings)
        {
            return query;
        }
    }
}
