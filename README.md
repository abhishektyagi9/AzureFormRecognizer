[![Board Status](https://dev.azure.com/AzureDevOpsOrgSLGCustomers/157502e1-798a-4583-a6e1-cc60008c035e/d91b1853-d3dd-47df-b221-fe0536da08ec/_apis/work/boardbadge/77731713-58ed-4905-a8f5-815310810e88)](https://dev.azure.com/AzureDevOpsOrgSLGCustomers/157502e1-798a-4583-a6e1-cc60008c035e/_boards/board/t/d91b1853-d3dd-47df-b221-fe0536da08ec/Microsoft.RequirementCategory)
# Automated document processing for  organizations with paper-based forms(ASP.net MVC and Function CORE .NET)

This reference architecture shows how to deploy an end-to-end form admissions form information extraction pipeline. It uses a browser page for file/image ingestion, Application Gateway for traffic balancing, Azure Data Lake Storage to store the images, Event Grid for event-based triggering, Azure Functions for serverless invocations, Azure Form Recognizer pre-built General Document API Model (for this solution accelerator, could leverage custom-built model if needed) & CosmosDB to store the results. The SaaS app integration is left out, as this would be case-specific. (PLACEHOLDER FOR POWERBI VISUALIZATION IF WE WANT). 

![](./_images/SolutionArchitecture.jpg)

## Deploy

Before you hit the deploy button, make sure you review the details about the services deployed.

Deploy to Azure Commericial[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fabhishektyagi9%2FAzureFormRecognizer%2Fmaster%2FARMTemplates%2Fformtemplatedeployment.json)

Deploy to Azure Commericial[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)


Deploy to Azure Government  [![Deploy to Azure Government](https://aka.ms/deploytoazurebutton)](https://portal.azure.us/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fabhishektyagi9%2FAzureFormRecognizer%2Fmaster%2FARMTemplates%2Fformtemplatedeployment.json)

Once the resource deployed, you will need to deploy the functions to the Function App (at this time - could be further automated).

> **Important:** This deployment accelerator implements some service features that are still in Public Preview. Please consider those before you plan for a production deployment.
