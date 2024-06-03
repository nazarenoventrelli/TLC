using System.Globalization;
using Amazon.Lambda.Core;
using Amazon.S3;
using HtmlAgilityPack;
using Amazon.Glue;
using Amazon.Glue.Model;
using tlc_query_handler.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace tlc_scraper;

public class Function
{

    private static readonly HttpClient client = new HttpClient();
    private static readonly IAmazonS3 s3Client = new AmazonS3Client();
    private static readonly IAmazonGlue glueClient = new AmazonGlueClient();
    private HashSet<string> knownTypes = new HashSet<string>();

    public async Task FunctionHandler(ScrapeParams request, ILambdaContext context)
    {
        int currentYear = DateTime.Now.Year;
        int currentMonthNumber = DateTime.Now.Month;

        if (request != null)
        {
            currentYear = request.Year;
            currentMonthNumber = request.Month;
        }

        knownTypes = await LoadKnownTypesFromGlue();
        string baseUrl = "https://www.nyc.gov/site/tlc/about/tlc-trip-record-data.page";
        string currentMonth = new CultureInfo("en-US").DateTimeFormat.GetMonthName(currentMonthNumber);

        var response = await client.GetAsync(baseUrl);
        var pageContents = await response.Content.ReadAsStringAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(pageContents);

        var yearDiv = doc.DocumentNode.SelectSingleNode($"//div[@id='faq{currentYear}']");
        if (yearDiv != null)
        {
            var monthNode = yearDiv.SelectSingleNode($".//p/strong[contains(text(), '{currentMonth}')]");
            if (monthNode != null)
            {
                var ulNode = monthNode.ParentNode.SelectSingleNode("./following-sibling::ul");
                if (ulNode != null)
                {
                    var links = ulNode.SelectNodes(".//a[@href]");
                    if (links != null)
                    {
                        bool newTypeFound = false;

                        foreach (var link in links)
                        {
                            string hrefValue = link.Attributes["href"].Value;
                            if (hrefValue.Trim().EndsWith(".parquet"))
                            {
                                string fileUrl = hrefValue.StartsWith("http") ? hrefValue : $"https://{hrefValue.Trim()}";
                                string type = ExtractTypeFromUrl(fileUrl);

                                newTypeFound = !knownTypes.Contains(type);

                                await DownloadAndStoreFile(fileUrl);
                            }
                        }

                        if (newTypeFound)
                        {
                            await StartGlueCrawler("tlc-crawler");
                        }
                    }
                }
            }
        }
    }

    private async Task<HashSet<string>> LoadKnownTypesFromGlue()
    {
        var tablesResponse = await glueClient.GetTablesAsync(new GetTablesRequest
        {
            DatabaseName = "glue-db"
        });

        HashSet<string> types = new HashSet<string>();
        foreach (var table in tablesResponse.TableList)
        {
            var location = table.StorageDescriptor.Location;
            var type = location.Split('/')[3];
            if (!string.IsNullOrEmpty(type))
            {
                types.Add(type);
            }
        }
        return types;
    }
    private string ExtractTypeFromUrl(string fileUrl)
    {
        var fileName = Path.GetFileNameWithoutExtension(fileUrl);
        var parts = fileName.Split('_');
        return parts[0];
    }
    private string BuildS3Key(string fileUrl)
    {
        string url = fileUrl.StartsWith("http") ? fileUrl : $"https://{fileUrl.Trim()}";
        string type = ExtractTypeFromUrl(url);
        string fileName = fileUrl.Split('/').Last().Replace('_', '-');

        return $"{type}/{fileName}";
    }

    private async Task DownloadAndStoreFile(string fileUrl)
    {

        string s3Key = BuildS3Key(fileUrl);
        var response = await client.GetAsync(fileUrl);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to download {fileUrl}");
            return;
        }

        using (var responseStream = await response.Content.ReadAsStreamAsync())
        {
            var putRequest = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = "tlc-container",
                Key = s3Key,
                InputStream = responseStream
            };
            await s3Client.PutObjectAsync(putRequest);
        }
    }

    private async Task StartGlueCrawler(string crawlerName)
    {
        var response = await glueClient.StartCrawlerAsync(new StartCrawlerRequest { Name = crawlerName });
        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine($"Error starting Glue crawler: {crawlerName}");
        }
    }
}
