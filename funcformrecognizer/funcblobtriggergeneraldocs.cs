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
              ConnectionStringSetting = "CosmosDBConnection", CreateIfNotExists = true)] IAsyncCollector<object> analysisresults
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
               // List<AnalysedDocuments> fields = new List<AnalysedDocuments>();
                Analysisdocs dataDict = new Analysisdocs();
                dataDict.keyvaluepair = new Dictionary<string, string>();
                //AnalysedDocuments objAnalysisdoc = new AnalysedDocuments();
                //objAnalysisdoc.content = result.Content;
                //List<KeyValuePair> objlistvalue = new List<KeyValuePair>();
                //List<Enttities> objentities = new List<Enttities>();
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
                                dataDict.keyvaluepair.Add(result.KeyValuePairs[i].Key.Content + "2", result.KeyValuePairs[i].Value.Content);
                            }


                        }
                    }
                    catch
                    {
                        throw;
                    }
                 
                    //objlistvalue.Add(objkey);

                }
                //for (int i = 0; i < result.Entities.Count; i++)
                //{

                //    Enttities objkey = new Enttities();
                //    if (result.KeyValuePairs[i].Key != null)
                //    {
                //        objkey.Category = Convert.ToString(result.Entities[i].Category);
                //        objkey.SubCategory = Convert.ToString(result.Entities[i].SubCategory);
                //        objkey.Value = Convert.ToString(result.Entities[i].Content);
                //    }

                //    objentities.Add(objkey);

                //}
                //objAnalysisdoc.keyValuePairs = objlistvalue;
                //objAnalysisdoc.entities = objentities;

                var jsonconvert = JsonConvert.SerializeObject(dataDict.keyvaluepair);

                await analysisresults.AddAsync(jsonconvert);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}