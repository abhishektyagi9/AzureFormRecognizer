using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace funcformrecognizer
{
    public static class funcblobtriggergeneraldocs
    {
        [FunctionName("funcblobtriggergeneraldocs")]
        public static async Task Run([BlobTrigger("forms/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            [CosmosDB(
              databaseName: "dbformrecognizer",
              collectionName: "dbformscontainer",
              ConnectionStringSetting = "CosmosDBConnection",PartitionKey ="/processingdate" ,CreateIfNotExists = true)] IAsyncCollector<object> analysisresults
          , ILogger log)
        {


            try
            {
                string endpoint = "https://formdeploymentdemo022.cognitiveservices.azure.com/";
                string apiKey = "2c15beb8e33f45a08704b6f159a21b3b";
                var credential = new AzureKeyCredential(apiKey);
                var client = new DocumentAnalysisClient(new Uri(endpoint), credential);
                //Uri fileUri = new Uri("<fileUri>");

                AnalyzeDocumentOperation operation = await client.StartAnalyzeDocumentAsync("prebuilt-document", myBlob);

                await operation.WaitForCompletionAsync();

                AnalyzeResult result = operation.Value;
                Analysisdocs dataDict = new Analysisdocs();
                dataDict.keyvaluepair = new Dictionary<string, string>();
          
                for (int i = 0; i < result.KeyValuePairs.Count; i++)
                {

                    try
                    {
                        if (result.KeyValuePairs[i].Key != null && result.KeyValuePairs[i].Value != null)
                        {


                            if (!dataDict.keyvaluepair.ContainsKey(result.KeyValuePairs[i].Key.Content))
                            {
                                dataDict.keyvaluepair.Add(result.KeyValuePairs[i].Key.Content, result.KeyValuePairs[i].Value.Content);
                            }
                            else
                            {
                                //handling duplicate keys in form if any
                                dataDict.keyvaluepair.Add(result.KeyValuePairs[i].Key.Content + "2", result.KeyValuePairs[i].Value.Content);
                            }


                        }
                    }
                    catch
                    {
                        throw;
                    }                   

                }
                if (dataDict.keyvaluepair != null)
                {
                    dataDict.keyvaluepair.Add("processingdate", DateTime.Now.ToShortDateString());
                }
                var jsonconvert = JsonConvert.SerializeObject(dataDict.keyvaluepair);

                //adding to cosmos db

                await analysisresults.AddAsync(jsonconvert);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}