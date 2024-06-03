using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tlc_query_handler.Models;

namespace Strategies.Interfaces
{
    public interface IQueryStrategy
    {
        string BuildQuery(QueryRequest request);
    }

}
    