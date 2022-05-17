using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace funcformrecognizer
{
    public static class formrecogniblobtrigger
    {
        [FunctionName("formrecogniblobtrigger")]
        public static async Task Run([BlobTrigger("invoices/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            [CosmosDB(
              databaseName: "dbformrecognizer",
              collectionName: "invoices",
              ConnectionStringSetting = "CosmosDBConnection",PartitionKey ="/InvoiceDate", CreateIfNotExists = true)] IAsyncCollector<object> analysisresults
          , ILogger log)
        {


            try
            {
                string endpoint = "https://formdeploymentdemo022.cognitiveservices.azure.com/";
                string apiKey = "2c15beb8e33f45a08704b6f159a21b3b";
                var credential = new AzureKeyCredential(apiKey);
                var client = new DocumentAnalysisClient(new Uri(endpoint), credential);
                //Uri fileUri = new Uri("<fileUri>");

                AnalyzeDocumentOperation operation = await client.StartAnalyzeDocumentAsync("prebuilt-invoice", myBlob);

                await operation.WaitForCompletionAsync();
                InvoiceData Objinvoice = new InvoiceData();
                AnalyzeResult result = operation.Value;

                for (int i = 0; i < result.Documents.Count; i++)
                {
                    Console.WriteLine($"Document {i}:");

                    AnalyzedDocument document = result.Documents[i];
                   
                    //await analysisresults.AddAsync(document.Fields);
                    if (document.Fields.TryGetValue("VendorName", out DocumentField? vendorNameField))
                    {
                        if (vendorNameField.ValueType == DocumentFieldType.String)
                        {
                            string vendorName = vendorNameField.AsString();
                            Objinvoice.VendorName = vendorName;
                            
                        }
                    }
                    if (document.Fields.TryGetValue("VendorAddress", out DocumentField? VendorAddressField))
                    {
                        if (VendorAddressField.ValueType == DocumentFieldType.String)
                        {
                            string vendorAddress = VendorAddressField.AsString();
                            Objinvoice.VendorAddress = vendorAddress;

                        }
                    }
                    if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField? InvoiceTotalField))
                    {
                        if (InvoiceTotalField.ValueType == DocumentFieldType.Currency)
                        {
                            CurrencyValue Invoicestotal = InvoiceTotalField.AsCurrency();
                            Objinvoice.InvoiceTotal = Invoicestotal.Amount;

                        }
                    }
                    if (document.Fields.TryGetValue("CustomerName", out DocumentField? customerNameField))
                    {
                        if (customerNameField.ValueType == DocumentFieldType.String)
                        {
                            string customerName = customerNameField.AsString(); 
                            Objinvoice.CustomerName = customerName;
                        }
                    }
                    if (document.Fields.TryGetValue("DueDate", out DocumentField? DueDatefield))
                    {
                        if (DueDatefield.ValueType == DocumentFieldType.Date)
                        {
                            DateTime strDuedate = DueDatefield.AsDate();
                            Objinvoice.DueDate = strDuedate.ToShortDateString();
                        }
                    }
                    if (document.Fields.TryGetValue("InvoiceId", out DocumentField? InvoiceIdfield))
                    {
                        if (InvoiceIdfield.ValueType == DocumentFieldType.String)
                        {
                            string strDInvoiceIdfield = InvoiceIdfield.AsString();
                            Objinvoice.InvoiceId = strDInvoiceIdfield;
                        }
                    }
                    if (document.Fields.TryGetValue("BillingAddress", out DocumentField? BillingAddressfield))
                    {
                        if (BillingAddressfield.ValueType == DocumentFieldType.String)
                        {
                            string strDInvoiceIdfield = BillingAddressfield.AsString();
                            Objinvoice.BillingAddress = strDInvoiceIdfield;
                        }
                    }
                    if (document.Fields.TryGetValue("ShippingAddress", out DocumentField? ShippingAddressfield))
                    {
                        if (ShippingAddressfield.ValueType == DocumentFieldType.String)
                        {
                            string strShippingAddressfield = ShippingAddressfield.AsString();
                            Objinvoice.ShippingAddress = strShippingAddressfield;
                        }
                    }
                    if (document.Fields.TryGetValue("TotalTax", out DocumentField? TotalTaxfield))
                    {
                        if (TotalTaxfield.ValueType == DocumentFieldType.Currency)
                        {
                            CurrencyValue strShippingAddressfield = TotalTaxfield.AsCurrency();
                            Objinvoice.TotalTax = strShippingAddressfield.Amount;
                        }
                    }
                    if (document.Fields.TryGetValue("InvoiceDate", out DocumentField? InvoiceDateield))
                    {
                        if (InvoiceDateield.ValueType == DocumentFieldType.Date)
                        {
                            DateTime strInvoicedate = InvoiceDateield.AsDate();
                            Objinvoice.InvoiceDate = strInvoicedate.ToShortDateString();
                        }
                    }

                    if (document.Fields.TryGetValue("Items", out DocumentField? itemsField))
                    {
                        if (itemsField.ValueType == DocumentFieldType.List)
                        {
                            foreach (DocumentField itemField in itemsField.AsList())
                            {
                                

                                if (itemField.ValueType == DocumentFieldType.Dictionary)
                                {
                                    IReadOnlyDictionary<string, DocumentField> itemFields = itemField.AsDictionary();

                                    if (itemFields.TryGetValue("Description", out DocumentField? itemDescriptionField))
                                    {
                                        if (itemDescriptionField.ValueType == DocumentFieldType.String)
                                        {
                                            string itemDescription = itemDescriptionField.AsString();

                                          
                                        }
                                    }

                                    if (itemFields.TryGetValue("Amount", out DocumentField? itemAmountField))
                                    {
                                        if (itemAmountField.ValueType == DocumentFieldType.Double)
                                        {
                                            double itemAmount = itemAmountField.AsDouble();
                                            Objinvoice.SubTotal = itemAmount.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (document.Fields.TryGetValue("SubTotal", out DocumentField? subTotalField))
                    {
                        if (subTotalField.ValueType == DocumentFieldType.Double)
                        {
                            double subTotal = subTotalField.AsDouble();
                            Console.WriteLine($"Sub Total: '{subTotal}', with confidence {subTotalField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("TotalTax", out DocumentField? totalTaxField))
                    {
                        if (totalTaxField.ValueType == DocumentFieldType.Double)
                        {
                            double totalTax = totalTaxField.AsDouble();
                            Console.WriteLine($"Total Tax: '{totalTax}', with confidence {totalTaxField.Confidence}");
                        }
                    }

                    if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField? invoiceTotalField))
                    {
                        if (invoiceTotalField.ValueType == DocumentFieldType.Double)
                        {
                            double invoiceTotal = invoiceTotalField.AsDouble();
                            Console.WriteLine($"Invoice Total: '{invoiceTotal}', with confidence {invoiceTotalField.Confidence}");
                        }
                    }
                    await analysisresults.AddAsync(Objinvoice);
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}