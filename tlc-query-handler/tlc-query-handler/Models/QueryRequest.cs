using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tlc_query_handler.Models
{
    public class QueryRequest
    {
        public string TaxiType { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal MinimumFare { get; set; }

    }
}
