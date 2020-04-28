# Nexus Broker Sample Application

## Disclaimer with the Nexus retail crypto broker sample application

The Nexus sample application for retail crypto brokers is an open source C# web application providing a public front-end to the end-customers. At the back-end the sample web application is connected with the Nexus API to handle accounts and transactions.

The sample application supports crypto buy transactions (with the assumption of manual processing of incoming bank transfers) and crypto sell transactions (with the assumption of manual processing of payouts as bank transfers).

The sample application as-is should not be used for production purposes, but as a proof-of-concept example or base for further or own development. Some changes are required to turn the sample application into a minimal viable and safe production product. On top of that you can develop all kind of additional customizations.

### Minimal adaptation for production
The sample application has a very simplified model for account management. Customers can create accounts, white-list additional addresses, delete accounts and view account details directly in the sample web page without use of a second confirmation method. For security and privacy reasons it is advised to at least add a confirmation mail for account creation, white-listing additional addresses and account deletion. Also the account info could be send by mail.

### Further customization options
1) In the sample application the customer details are stored in Nexus. For privacy reasons it can be important to not share this information. Therefor the sample application can be extended with a local database to store the privacy sensitive information and only share an anonymous customer reference code with Nexus.
2) Instant payment methods can be added to the sample application, allowing for full automation of buy transactions and near-instant crypto delivery. Using specific payment methods in Nexus and call-back functionality of payment processors to create and update Nexus transactions.
3) All other kind of customer experience improving extensions, like price charts, referral programs, price alerts services.

The Quantoz Nexus team can help you with advice and support.
