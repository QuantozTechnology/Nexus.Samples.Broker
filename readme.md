# Nexus Broker Sample Application

## Disclaimer with the Nexus retail crypto broker sample application

The Nexus sample application for retail crypto brokers is an open source C# web application providing a public front-end to the end-customers. At the back-end the sample web application is connected with the Nexus API to handle accounts and transactions.

The sample application supports crypto buy transactions (with the assumption of manual processing of incoming bank transfers) and crypto sell transactions (with the assumption of manual processing of payouts as bank transfers).

The sample application and mail service as-is should not be used for production purposes, but as a proof-of-concept example or base for further or own development. Some changes are required to turn the sample application and mail service into a minimal viable and safe production product. On top of that you can develop all kind of additional customizations.

### Minimal adaptation for production
The sample application has a very simplified model for account management. Customers can create accounts, white-list additional addresses, delete accounts and view account details directly in the sample web page without use of a second confirmation method. For security and privacy reasons it is advised to at least add a confirmation mail for account creation, white-listing additional addresses and account deletion. Also the account info could be send by mail.

### Further customization options
1) In the sample application the customer details are stored in Nexus. For privacy reasons it can be important to not share this information. Therefor the sample application can be extended with a local database to store the privacy sensitive information and only share an anonymous customer reference code with Nexus.
2) Instant payment methods can be added to the sample application, allowing for full automation of buy transactions and near-instant crypto delivery. Using specific payment methods in Nexus and call-back functionality of payment processors to create and update Nexus transactions.
3) All other kind of customer experience improving extensions, like price charts, referral programs, price alerts services.

The Quantoz Nexus team can help you with advice and support.

### Customer communication
To support customer communication Nexus offers a Mail entity that stores information related to the Broker processes. This entity is either created manually via Nexus API or automatically by processes within Nexus. The sending of Mails as e-mail is handle by the Mail Service implemented as an [Azure Function](https://azure.microsoft.com/en-us/services/functions/?&ef_id=Cj0KCQjwqrb7BRDlARIsACwGad4pSj4Y3octbqqli5iLIAAlGZ5yBQpLnRs7j2vmvX4xwJuRfN25l28aArEOEALw_wcB:G:s&OCID=AID2100079_SEM_Cj0KCQjwqrb7BRDlARIsACwGad4pSj4Y3octbqqli5iLIAAlGZ5yBQpLnRs7j2vmvX4xwJuRfN25l28aArEOEALw_wcB:G:s) in this project. The subject and content of the e-mail is set by the Mail Service and an overview of the supported types along with their subject and content is described [here](/mail-examples.md).


## Configuration
The `appsettings.json` file contains the settings and client credentials that you need to run the app, contact Quantoz Nexus support to obtain the needed configuration to connect to your Nexus Tenant.

## Installation
- Install .NET Core 3.1 [Download link](https://dotnet.microsoft.com/download). Download and install the SDK as per your operating system. We always recommend to use LTS version.
- Windows users can install Visual Studio 2019 to build and run the project. Linux or Mac operating system users can use Visual Studio Code to build and run the project.
- To run the project with Visual Studio Code you need to install C# extension.
- Execute `dotnet run` to run the project.