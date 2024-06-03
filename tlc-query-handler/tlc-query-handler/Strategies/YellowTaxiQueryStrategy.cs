using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategies.Interfaces;
using tlc_query_handler.Models;

namespace tlc_query_handler.Strategies
{
    public class YellowTaxiQueryStrategy : IQueryStrategy
    {
        public string BuildQuery(QueryRequest request)
        {
            return $@"
                    SELECT * 
                    FROM yellow 
                    WHERE tpep_pickup_datetime >= date_parse('{request.Year}-{request.Month}-01', '%Y-%m-%d') 
                    AND tpep_pickup_datetime < date_add('month', 1, date_parse('{request.Year}-{request.Month}-01', '%Y-%m-%d'))
                    AND fare_amount >= {request.MinimumFare}
                    LIMIT 100";
        }
    }

}
