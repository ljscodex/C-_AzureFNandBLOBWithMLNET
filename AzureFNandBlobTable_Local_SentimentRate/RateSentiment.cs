using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using SentimentAnalysisFunctionsApp;

namespace AzureFNandBlobTable_Local_SentimentRate
{
    public static class RateSentiment
    {
        [FunctionName("Rate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AzureFunctionSentimentData>(requestBody);

            SentimentalIdentity sentiment = new SentimentalIdentity(data);

            var tableName = "MLSentimentDB";
            try
            {
                var storageAccount = CloudStorageAccount.Parse(Connection);
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();
                Console.WriteLine($"CloudTable name is : {tableClient}");
                await table.ExecuteAsync(TableOperation.Insert(sentiment));
                Console.WriteLine("Records Inserted");
                Console.WriteLine("END");
                return new ObjectResult("OK");

            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered Exception - " + e);
                return new ObjectResult(e);
            }
        }
    }
}
