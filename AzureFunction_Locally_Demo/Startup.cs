
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using SentimentAnalysisFunctionsApp;
using System.IO;
using System;
using AzureFunction_Locally_Demo.DataModels;
using Microsoft.Extensions.ML;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SentimentAnalysisFunctionsApp
{
    public class Startup : FunctionsStartup
    {
        private readonly string _environment;
        private readonly string _modelPath;

        public Startup()
        {
            _environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

            if (_environment == "Development")
            {
                _modelPath = Path.Combine("MLModels", "sentiment_model.zip");
            }
            else
            {
                string deploymentPath = @"D:\home\site\wwwroot\";
                _modelPath = Path.Combine(deploymentPath, "MLModels", "sentiment_model.zip");
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddPredictionEnginePool<SentimentData, SentimentPrediction>()
                .FromFile(modelName: "SentimentAnalysisModel", filePath: _modelPath, watchForChanges: true);
        }
    }
}
