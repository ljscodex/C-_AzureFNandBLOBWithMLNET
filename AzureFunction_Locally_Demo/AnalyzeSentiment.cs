
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunction_Locally_Demo.DataModels;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using AzureFunction_Locally_Demo;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using Microsoft.Extensions.ML;

namespace SentimentAnalysisFunctionsApp
{
    public class AnalyzeSentiment
    {
        private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;

        // AnalyzeSentiment class constructor
        public AnalyzeSentiment(PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [FunctionName("AnalyzeSentiment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Parse HTTP Request Body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SentimentData data = JsonConvert.DeserializeObject<SentimentData>(requestBody);

            //Make The Prediction
            SentimentPrediction prediction = _predictionEnginePool.Predict(modelName: "SentimentAnalysisModel", example: data);

            //Convert prediction to string
            string sentiment = Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative";


            //Return Prediction
            return new OkObjectResult(sentiment);
        }


    }

    public class AzureFunctionSentimentData

    {
        public string Text { get; set; }
        public string Answer { get; set; }
        public bool Rate { get; set; }

        public AzureFunctionSentimentData(string Text, string Answer, bool Rate)
        {
            this.Text = Text;
            this.Answer = Answer;
            this.Rate = Rate;
        }

    }
        public class SentimentalIdentity : TableEntity
    {
        public SentimentalIdentity(AzureFunctionSentimentData data)
        {
            this.PartitionKey = data.Text;
            this.RowKey = data.Answer;
            this.Rate = data.Rate;
        }
        public SentimentalIdentity() { } // the parameter-less constructor must be provided
        public Boolean Rate { get; set; }
    }


}
