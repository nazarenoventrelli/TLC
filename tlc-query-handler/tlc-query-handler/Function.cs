using Amazon.Lambda.Core;
using Strategies.Interfaces;
using tlc_query_handler.Models;
using tlc_query_handler.Strategies;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace tlc_query_handler;

public class Function
{
    private static readonly string s3Output = "s3://tlc-athena/";
   
    public async Task<List<string[]>> FunctionHandler(QueryRequest request, ILambdaContext context)
    {
        IQueryStrategy strategy;

        switch (request.TaxiType.ToLower())
        {
            case "yellow":
                strategy = new YellowTaxiQueryStrategy();
                break;
            case "green":
                strategy = new GreenTaxiQueryStrategy();
                break;
            case "fhvhv":
                strategy = new FhvhvQueryStrategy();
                break;
            case "fhv":
                strategy = new FhvQueryStrategy();
                break;
            default:
                throw new ArgumentException("Invalid taxi type");
        }

        var queryContext = new QueryContext(strategy);
        return await queryContext.ExecuteQuery(request, s3Output);
    }

}

