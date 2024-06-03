using Amazon.Athena.Model;
using Amazon.Athena;
using Strategies.Interfaces;
using tlc_query_handler.Models;

namespace tlc_query_handler.Strategies
{
    public class QueryContext
    {
        private readonly IQueryStrategy _strategy;
        private static readonly IAmazonAthena athenaClient = new AmazonAthenaClient();

        public QueryContext(IQueryStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<List<string[]>> ExecuteQuery(QueryRequest request, string s3Output)
        {
            string query = _strategy.BuildQuery(request);

            var startQueryRequest = new StartQueryExecutionRequest
            {
                QueryString = query,
                QueryExecutionContext = new QueryExecutionContext { Database = "glue-db" },
                ResultConfiguration = new ResultConfiguration { OutputLocation = s3Output }
            };

            var startQueryResponse = await athenaClient.StartQueryExecutionAsync(startQueryRequest);
            return await WaitForQueryAndFetchResults( startQueryResponse.QueryExecutionId, query);
        }

        public async Task<List<string[]>> WaitForQueryAndFetchResults(string queryExecutionId, string query)
        {
            try
            {
                QueryExecutionState state;
                do
                {
                    var queryExecution = await athenaClient.GetQueryExecutionAsync(new GetQueryExecutionRequest
                    {
                        QueryExecutionId = queryExecutionId
                    });
                    state = queryExecution.QueryExecution.Status.State;

                    await Task.Delay(500); 
                }
                while (state == QueryExecutionState.RUNNING || state == QueryExecutionState.QUEUED);

                if (state == QueryExecutionState.SUCCEEDED)
                {
                    var getQueryResultsRequest = new GetQueryResultsRequest
                    {
                        QueryExecutionId = queryExecutionId
                    };

                    var getQueryResultsResponse = await athenaClient.GetQueryResultsAsync(getQueryResultsRequest);

                    var results = getQueryResultsResponse.ResultSet.Rows
                        .Select(row => row.Data.Select(d => d.VarCharValue).ToArray())
                        .ToList();

                    return results;
                }
                else
                {
                    var queryExecution = await athenaClient.GetQueryExecutionAsync(new GetQueryExecutionRequest
                    {
                        QueryExecutionId = queryExecutionId
                    });
                    var stateChangeReason = queryExecution.QueryExecution.Status.StateChangeReason;
                    throw new Exception($"Query failed to complete. Status: {state} - {query}. Reason: {stateChangeReason}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception in WaitForQueryAndFetchResults: {ex.Message}");
            }
        }
    }


}
