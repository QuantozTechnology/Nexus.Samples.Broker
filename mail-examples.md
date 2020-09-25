
## Account related Mails

### NewAccountRequested
Mail that is created when a customer has created a new account. If required, this mail is used to also activate the newly created account via the activation link.

Created by Broker Sample

```
Subject: Broker Sample account: Request confirmation

Welcome user@test.com,

Broker Sample accepted your Broker Sample account request.

The Account code of your new Broker Sample account is: APNHBCEU

Please click this link to confirm and activate the account:
EmailAddress: user@test.com
XLM Address: GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H
```

### NewAccountActivated
Mail that is created to inform the customer that their account has been successfully activated.

Created by Broker Sample

```
Subject: Broker Sample account: Activated

Welcome user@test.com,

Broker Sample activated your Broker Sample account.

The Account code of your new Broker Sample account is: APNHBCEU
EmailAddress: user@test.com 
XLMAddress: GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H 
(your address to receive XLM)
```

### AccountDeleteRequested

Mail that is created upon a customer requesting to delete their existing account. The customer needs to confirm this action via the link in the mail.

Created by Broker Sample

```
Subject: Broker Sample account: Delete confirmation
Dear user@test.com ,

You requested deletion of your Broker Sample account.
Click this link to confirm to delete the account:
AccountCode: APNHBCEU 
EmailAddress: user@test.com 
XLM ReceiveAddress: GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H 
(your address to receive XLM)
XLM SellAddress: GDU7FZUS2IOMA4NQB3ENSDMQIKSOJALK2BG3A7MOMA2GQLSQ3BL23TYL 
(address to sell XLM)
```

### AccountDeletedByRequest
Mail that is created to inform the customer that their account has successfully been deleted.

Created by Broker Sample

```
Subject: Broker Sample account: Deleted

Dear user@test.com,

As requested Broker Sample deleted the following Broker Sample account:
AccountCode: APNHBCEU
EmailAddress: user@test.com 
XLM Address: GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H
You can no longer sell XLM by sending them to: GDU7FZUS2IOMA4NQB3ENSDMQIKSOJALK2BG3A7MOMA2GQLSQ3BL23TYL

For your information:
* XLM that Broker Sample still receives for this account CAN NO LONGER BE TRADED OR RETURNED
* Any existing unfinished transaction will continue and be finished with the old account details
* New transactions are no longer possible with this deleted account
```
## Customer related Mails

### AccountInfoRequest
Mail that is created when a customer requests their account information.

Created by Broker Sample

```
Subject: Broker Sample accounts: Information

Dear user@test.com ,

As requested, you hereby receive the details about your Broker Sample account(s):

Active accounts overview:
AccountCode	Level	BankAccount	Sell Address (send to address)	Buy/Return Address (your receive address)
APNHBCEU	Trusted		GDU7FZUS2IOMA4NQB3ENSDMQIKSOJALK2BG3A7MOMA2GQLSQ3BL23TYL	GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H


No recent transactions found.

```
### TrustLevelUpdated
Created when a customer's trust level is updated. The trust level can be updated by an operator via Nexus Portal or via Nexus API.

Created by Nexus or Broker Sample
```
Subject: Broker Sample accounts: Trust level update

Dear user@test.com,

Broker Sample updated your Broker Sample account(s):
EmailAddress: user@test.com 
Your trust level is: Trusted 
(this trust level applies to all your Broker Sample accounts sharing the same IBAN bank account)
```

## Transaction related Mails

### TransactionBuySendDelay
Mail that is created when a sending delay is configured for a payment method. 

Created by Nexus

```
Subject: 
Broker Sample: Starting holding period for 0.00000000 LTC

Dear user@test.com,

Broker Sample has executed your LTC buy transaction:
AccountCode: APNHBCEU
TransactionId: TxCode100 
PaymentAmount: 10.00 EUR
CreateTimestamp: 2020-09-15|10:34:09 UTC
LTCAddress: GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H (the address where your LTC will be sent to)

Our payment processor requires a holding period before we can send the LTC to you. The LTC will therefor be sent to your above mentioned LTCAddress after a delay. You will receive an email immediately after we have sent the LTC to you.
```

### BlockedTransaction
Mail that is created when a transaction is blocked. The blocking of a transaction can be done manually by an operator via Nexus Portal or automatically by Nexus.

Created by Nexus

```
Subject: Broker Sample Transaction blocked

Dear user@test.com,

Your buy transaction has been temporarily blocked:
AccountCode: APNHBCEU
TransactionId: TxCode100
PaymentAmount: 10.00 EUR
ReceiveTimestamp: 2020-09-15|10:34:09 UTC 

LTCAddress: GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H
(the address where your LTC should be sent to)

Blocking reason: Test Comment
```

### TransactionBuyFinish
Mail that is created when a BUY transaction has successfully been completed by a customer.

Created by Nexus

Subject: Broker Sample Sent 100.00000000 LTC

Dear user@test.com,

Broker Sample has finished your LTC buy transaction:
AccountCode: L3TWSCSB
TransactionId: TxCode100
PaymentAmount: 10.00 EUR
CreateTimestamp: 2020-09-15|10:34:09 UTC 
(receiving the transaction notification)
FinishTimestamp: 2020-09-15|10:34:09 UTC
(confirmation of sending the LTC to your wallet)
LTC Address: GCZUMMYGOJLQYPMHBCKUBNS46L62SX673MAH2USBIDD4TB2KNAGEIY5H
(the address where your LTC are sent to)
LTC TransactionId:

Financial result:
Received payment: 10.00 EUR
LTC price: 100.00000 EUR/LTC
Transaction amount: 100.00000000 LTC
Transaction fee: 1.00000000 LTC (Service fee 0.00 EUR)
Amount sent to your wallet: 100.00000000 LTC

The LTC have been sent to the above mentioned LTC Address. In approximately 10 minutes you will have the transaction confirmation from the LTC network. You can check the confirmation status of this transaction on the BTC.com webpage.


LTC is a form of electronic money that can be used for payments. However, LTC is not legal tender because it is not (yet) recognized in any country as such. Since LTC is used for the same purposes as money, it should be treated equally (based on the principle of neutrality). As such (and for the time being), Broker Sample considers the servicing of purchase and sale of LTC to be exempt for VAT purposes.
```