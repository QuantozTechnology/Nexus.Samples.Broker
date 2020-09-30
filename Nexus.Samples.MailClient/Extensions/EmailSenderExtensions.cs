using Nexus.Samples.MailClient.Models;
using Nexus.Samples.Sdk.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexus.Samples.MailClient
{
    public static class EmailSenderExtensions
    {
        public static async Task<(string, string)> SendNewAccountRequestedAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, string activationLink, GetAccountResponse account)
        {
            string subject = $"{BaseModel.ApplicationName} account: Request confirmation";
            string body = @$"Welcome {email}<br><br>
                {BaseModel.ApplicationName} <b>accepted</b> your {BaseModel.ApplicationName} account request.<br><br>
                The Account code of your new {BaseModel.ApplicationName} account is: <b> {account.AccountCode}</b><br><br>
                <b>Please click <a href=" + "\"" + $"{activationLink}" + "\"" + @$"> this link</a> to confirm and activate the account:</b><br>
                <ul>
                    <li>EmailAddress: {email}</li>
                    <li>{account.DcCode} Address: {account.CustomerCryptoAddress}
                </ul>";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendNewAccountActivatedAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, GetAccountResponse account)
        {
            string subject = BaseModel.ApplicationName + " account: Activated";
            string body = @$"Welcome {email},<br><br>
                    {BaseModel.ApplicationName} <b>activated</b> your {BaseModel.ApplicationName} account.<br><br>
                    The Account code of your new {BaseModel.ApplicationName} account is: <b>{account.AccountCode}</b><br>
                    <ul>EmailAddress: {email}<br>
                    {account.DcCode}Address: {account.CustomerCryptoAddress} <br><i>(your address to receive {account.DcCode})</i><br>";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendAccountDeletedByRequestAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, GetAccountResponse account)
        {
            string subject = $"{BaseModel.ApplicationName} account: Deleted";

            string body = @$"Dear {email},<br><br>
                As requested {BaseModel.ApplicationName} <b>deleted</b> the following {BaseModel.ApplicationName} account:<br>
                <ul>AccountCode: {account.AccountCode}<br>
                EmailAddress: {email}<br>
                {account.DcCode} Address: {account.CustomerCryptoAddress}<br>
                You can no longer sell {account.DcCode} by sending them to: {account.DcReceiveAddress}<br>
                <br>For your information:<br>
                * {account.DcCode} that {BaseModel.ApplicationName} still receives for this account <b>CAN NO LONGER BE TRADED OR RETURNED</b><br>
                * Any existing unfinished transaction will continue and be finished with the old account details<br>
                * New transactions are no longer possible with this deleted account<br>";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendTransactionBuyFinishAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, GetAccountResponse account, GetBrokerTransactionResponse transaction)
        {
            string ratetext = "";
            //
            string receivetext = "receiving the transaction notification";
            string notifytimestring = "";
            if (transaction.Notified != null)
            {
                notifytimestring = $"NotifyTimestamp: {transaction.Notified.Value:yyyy-MM-dd|HH:mm:ss} UTC <br><i>({receivetext})</i><br>";
                receivetext = $"initiation of transaction on the {BaseModel.ApplicationName} page";
            }
            //
            string bankfee = "", servicefee = "", networkfee = "", payfeesplit = "";
            if (transaction.BankCommission.GetValueOrDefault(0) > 0)
            {
                bankfee = $"Bank fee {transaction.BankCommission.GetValueOrDefault(0):F2} {transaction.CurrencyCode}";
            }
            if (transaction.PartnerCommission.GetValueOrDefault(0) > 0)
            {
                servicefee = $"Service fee {transaction.PartnerCommission.GetValueOrDefault(0):F2} {transaction.CurrencyCode}";
            }
            if (transaction.NetworkCommission.GetValueOrDefault(0) > 0)
            {
                networkfee = $"Network fee {transaction.NetworkCommission.GetValueOrDefault(0):F2} {transaction.CurrencyCode}";
            }
            if ((bankfee != "") && (servicefee != "") && (networkfee != ""))
            {
                payfeesplit = $" ({bankfee}; {servicefee}; {networkfee})";
            }
            else if ((bankfee != "") && (servicefee != ""))
            {
                payfeesplit = $" ({bankfee}; {servicefee})";
            }
            else if (bankfee != "")
            {
                payfeesplit = $" ({bankfee})";
            }
            else if (servicefee != "")
            {
                payfeesplit = $" ({servicefee})";
            }
            //

            string subject = BaseModel.ApplicationName + $" Sent {transaction.CryptoSent.GetValueOrDefault(0):F8} {transaction.CryptoCurrencyCode}";

            string body = @$"Dear {email},<br><br>
                {BaseModel.ApplicationName} has <b>finished</b> your {transaction.CryptoCurrencyCode} buy transaction:<br>
                <ul>AccountCode: {transaction.AccountCode}<br>
                TransactionId: {transaction.TransactionCode}<br>
                PaymentAmount: {transaction.TradeValue.GetValueOrDefault(0):F2} {transaction.CurrencyCode}<br>
                CreateTimestamp: {transaction.Created:yyyy-MM-dd|HH:mm:ss} UTC <br><i>({receivetext })</i><br>
                {notifytimestring}
                FinishTimestamp: {transaction.Finished.GetValueOrDefault():yyyy-MM-dd|HH:mm:ss} UTC <br><i>(confirmation of sending the {transaction.CryptoCurrencyCode} to your wallet)</i><br>
                {transaction.CryptoCurrencyCode} Address: {account.CustomerCryptoAddress}<br><i> (the address where your {transaction.CryptoCurrencyCode} are sent to)</i><br>
                {transaction.CryptoCurrencyCode} TransactionId: {transaction.CryptoSendTxId}</ul><br>
                Financial result:<br>
                <ul>Received payment: {transaction.TradeValue.GetValueOrDefault(0):F2} {transaction.CurrencyCode}<br>
                {transaction.CryptoCurrencyCode} price: {transaction.CryptoPrice.GetValueOrDefault(0):F5} {transaction.CurrencyCode}/{transaction.CryptoCurrencyCode}{ratetext}<br>
                Transaction amount: {transaction.CryptoAmount.GetValueOrDefault(0):F8} {transaction.CryptoCurrencyCode}<br>
                Transaction fee: {transaction.CryptoAmount.GetValueOrDefault(0) - transaction.CryptoSent.GetValueOrDefault(0):F8} {transaction.CryptoCurrencyCode} {payfeesplit}<br>
                <b>Amount sent to your wallet: {transaction.CryptoSent.GetValueOrDefault(0):F8} {transaction.CryptoCurrencyCode}</b></ul><br>
                The {transaction.CryptoCurrencyCode} have been sent to the above mentioned {transaction.CryptoCurrencyCode} Address. In approximately 10 minutes you will have the transaction confirmation from the {transaction.CryptoCurrencyCode} network.
                You can check the confirmation status of this transaction on the <a href={"https://btc.com/"}{transaction.CryptoSendTxId}>BTC.com</a> webpage.<br>
                <br /><hr /><br /><i> {transaction.CryptoCurrencyCode} is a form of electronic money that can be used for payments.
                However, {transaction.CryptoCurrencyCode} is not legal tender because it is not (yet) recognized in any country as such.
                Since {transaction.CryptoCurrencyCode} is used for the same purposes as money, it should be treated equally (based on the principle of neutrality).
                As such (and for the time being), {BaseModel.ApplicationName} considers the servicing of purchase and sale of {transaction.CryptoCurrencyCode} to be exempt for VAT purposes.</i><br />";


            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendTrustLevelUpdatedAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, GetCustomerResponse customer, List<GetAccountResponse> accounts)
        {
            string subject = BaseModel.ApplicationName + " accounts: Trust level update";
            string accountCodes = "AccountCode(s): ";
            foreach (var account in accounts)
            {
                accountCodes = $"{accountCodes}{account.AccountCode} ,";
            }
            accountCodes.TrimEnd(',');
            string body = @$"Dear {email},<br><br>
                 {BaseModel.ApplicationName} <b>updated</b> your {BaseModel.ApplicationName} account(s):<br>
                 <ul>EmailAddress: {email} <br>
                 Your trust level is: <b>{customer.Trustlevel}</b> {(customer.IsHighRisk ? "(currently limited)" : "")} <br>
                 (this trust level applies to all your {BaseModel.ApplicationName} accounts sharing the same IBAN bank account)<br>";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendAccountInfoRequestedAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC,
            List<GetAccountResponse> accounts, List<GetBrokerTransactionResponse> transactions, GetCustomerResponse customer)
        {
            string accountlisttext = "<br><b>No active accounts found.</b><br>";
            string transactionslisttext = "<br><b>No recent transactions found.</b><br>";

            if (accounts.Any())
            {
                accountlisttext = @"<br><u>Active accounts overview:</u><style>table { border : 1 } td { border : 0 }</style>
                    <table><tr><td>AccountCode</td><td>Level</td><td>BankAccount</td>
                    <td>Sell Address (send to address)</td><td>Buy/Return Address (your receive address)</td></tr>";

                foreach (GetAccountResponse account in accounts)
                {
                    accountlisttext += "<tr><td><b>" + account.AccountCode + "</b></td><td>" + customer.Trustlevel + "</td>";
                    accountlisttext += "<td>" + customer.BankAccount;
                    accountlisttext += "<td>" + account.DcReceiveAddress + "</td><td>" + account.CustomerCryptoAddress + "</td></tr>";
                }

                accountlisttext += "</table></br>";
            }

            if (transactions.Any())
            {
                transactionslisttext = @"<br><u>Transactions overview (last 12 months):</u><br><style>table { border : 1 } td
                    { border : 0 }</style><table><tr><td>Date</td><td>Type</td><td>TransactionId</td>
                    <td>Status</td><td>AccountCode</td><td>BankAccount</td><td>PayMethod</td><td>ReceivedFromYou
                    </td><td>SendToYou</td></tr>";

                foreach (GetBrokerTransactionResponse transaction in transactions)
                {
                    string row = "<tr><td>" + transaction.Created.ToString("yyyy-MM-dd") + "</td><td>" + transaction.TransactionType + "</td><td><b>" + transaction.TransactionCode + "</b></td><td>"
                               + transaction.Status + "</td><td>" + transaction.AccountCode + "</td><td>";

                    if (transaction.TransactionType == "BUY")
                    {
                        row += (transaction.TradeValue == null ? "" : transaction.TradeValue.Value.ToString("F2") + " " + transaction.CurrencyCode) + "</td><td>";
                        row += (transaction.CryptoSent == null ? "" : transaction.CryptoSent.Value.ToString("F8") + $" {transaction.CryptoCurrencyCode}") + "</td></tr>";
                    }

                    if (transaction.TransactionType == "SELL")
                    {
                        if (transaction.Status.ToUpper().Contains("RETURN"))
                        {
                            row += (transaction.CryptoAmount == null ? "" : transaction.CryptoAmount.Value.ToString("F8") + $" {transaction.CryptoCurrencyCode}") + "</td><td>";
                            row += (transaction.CryptoSent == null ? "" : transaction.CryptoSent.Value.ToString("F8") + $" {transaction.CryptoCurrencyCode}") + "</td></tr>";
                        }
                        else
                        {
                            row += (transaction.CryptoAmount == null ? "" : transaction.CryptoAmount.Value.ToString("F8") + $" {transaction.CryptoCurrencyCode}") + "</td><td>";
                            row += (transaction.Payout == null ? "" : transaction.Payout.Value.ToString("F2") + " " + transaction.CurrencyCode) + "</td></tr>";
                        }
                    }

                    if (transaction.TransactionType == "GIFT")
                    {
                        row += "</td><td>" + (transaction.CryptoSent == null ? "" : transaction.CryptoSent.Value.ToString("F8") + $" {transaction.CryptoCurrencyCode}") + "</td></tr>";
                    }

                    transactionslisttext += row;
                }

                transactionslisttext += "</table><br>";
            }

            string subject = BaseModel.ApplicationName + " accounts: Information";
            string body = $@"Dear {email},<br><br>
                As requested, you hereby receive the details about your {BaseModel.ApplicationName} account(s):<br> {accountlisttext}{transactionslisttext}";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendAccountDeleteRequestedAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, string deleteLink, GetAccountResponse account)
        {
            string subject = $"{BaseModel.ApplicationName} account: Delete confirmation";
            string body = @$"Dear {email},<br><br>
                 You requested <b>deletion</b> of your {BaseModel.ApplicationName} account.<br>
                 <b>Click <a href=" + "\"" + $"{deleteLink}" + "\"" + @$"> this link</a> to confirm to delete the account:</b><br>
                 <ul>AccountCode: {account.AccountCode} <br>
                 EmailAddress: {email} <br>
                 {account.DcCode} ReceiveAddress: {account.CustomerCryptoAddress} <br><i>(your address to receive {account.DcCode})</i><br>
                 {account.DcCode} SellAddress: { account.DcReceiveAddress} <br><i>(address to sell {account.DcCode})</i><br>";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendTransactionBuySendDelayAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, GetAccountResponse account, 
            GetBrokerTransactionResponse transaction)
        {
            string subject = $"{BaseModel.ApplicationName}: Starting holding period for {transaction.CryptoSent.GetValueOrDefault().ToString("F8")} {transaction.CryptoCurrencyCode}";

            string body = @$"Dear {email},<br><br>
             {BaseModel.ApplicationName} has <b>executed</b> your {transaction.CryptoCurrencyCode} buy transaction:<br>
             <ul>AccountCode: {account.AccountCode}<br>
             TransactionId: {transaction.TransactionCode} <br>
             PaymentAmount: {transaction.TradeValue.GetValueOrDefault(0).ToString("F2")} {transaction.CurrencyCode}<br>
             CreateTimestamp: {transaction.Created.ToString("yyyy-MM-dd|HH:mm:ss")} UTC<br>
             {transaction.CryptoCurrencyCode}Address: {account.CustomerCryptoAddress} (the address where your {transaction.CryptoCurrencyCode} will be sent to)</ul><br>
             Our payment processor requires a <b>holding period</b> before we can send the {transaction.CryptoCurrencyCode} to you. The {transaction.CryptoCurrencyCode} 
             will therefor be sent to your above mentioned {transaction.CryptoCurrencyCode}Address after a delay.
             You will receive an email immediately after we have sent the {transaction.CryptoCurrencyCode} to you.<br>";
  
            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendBlockedTransactionAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, GetAccountResponse account,
            GetBrokerTransactionResponse transaction)
        {
            string subject = $"{BaseModel.ApplicationName} Transaction blocked";

            var body = $@"Dear {email},<br><br>
                Your buy transaction has been temporarily <b>blocked</b>:<br>
                <ul>AccountCode: {account.AccountCode}<br>
                TransactionId: {transaction.TransactionCode}<br>
                PaymentAmount: {transaction.TradeValue.GetValueOrDefault(0).ToString("F2")} {transaction.CurrencyCode}<br>
                ReceiveTimestamp: {transaction.Created.ToString("yyyy-MM-dd|HH:mm:ss")} UTC <br><br>
                {transaction.CryptoCurrencyCode}Address: {account.CustomerCryptoAddress}<br><i>(the address where your {transaction.CryptoCurrencyCode} should be sent to)</i></ul><br>
                Blocking reason: <b>{transaction.Comment}</b><br><br>";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

        public static async Task<(string, string)> SendTransactionSellFinishAsync(this IEmailSender emailSender, string email, List<string> CC, List<string> BCC, GetAccountResponse account,
            GetBrokerTransactionResponse transaction)
        {
            string subject = $"{BaseModel.ApplicationName}: Payment confirmation {transaction.CryptoAmount.Value.ToString("F8")} {transaction.CryptoCurrencyCode}";

            string body = @$"Dear {email},<br><br>
                {BaseModel.ApplicationName} has <b>finished</b> your {transaction.CryptoCurrencyCode} sell transaction:<br>
                <ul>AccountCode: {transaction.AccountCode}<br>
                TransactionId: {transaction.TransactionCode}<br>
                {transaction.CryptoCurrencyCode} Amount: {transaction.CryptoAmount.GetValueOrDefault(0).ToString("F8")} {transaction.CryptoCurrencyCode}<br>
                {transaction.CryptoCurrencyCode} Sell Address: {account.DcReceiveAddress} (the address where you sent the {transaction.CryptoCurrencyCode})<br>
                {transaction.CryptoCurrencyCode} Transaction Id: {transaction.CryptoReceiveTxId}<br>
                CreateTimestamp: {transaction.Created:yyyy-MM-dd|HH:mm:ss} UTC<br>
                ConfirmationTimestamp: {transaction.Confirmed?.ToString("yyyy-MM-dd|HH:mm: ss") + " UTC" ?? "MISSING"}<br>
                FinishTimestamp: {transaction.Finished.GetValueOrDefault():yyyy-MM-dd|HH:mm:ss} UTC <br><i>(confirmation of sending the {transaction.CryptoCurrencyCode} to your wallet)</i></ul><br>
                Financial result:<br>
                <ul>{transaction.CryptoCurrencyCode} price: {transaction.CryptoPrice.GetValueOrDefault(0).ToString("F5")} {transaction.CurrencyCode}/{transaction.CryptoCurrencyCode}<br>
                Transaction value: {transaction.TradeValue.GetValueOrDefault(0).ToString("F2")} {transaction.CurrencyCode}<br>
                Bank costs: {transaction.BankCommission.GetValueOrDefault(0).ToString("F2")} {transaction.CurrencyCode}<br>
                Service fee: {transaction.PartnerCommission.GetValueOrDefault(0).ToString("F2")} {transaction.CurrencyCode}<br>
                <b>Payout on your bank account: {transaction.Payout.GetValueOrDefault(0).ToString("F2")} {transaction.CurrencyCode}</b></ul><br>
                <br /><hr /><br /><i> {transaction.CryptoCurrencyCode} is a form of electronic money that can be used for payments.
                However, {transaction.CryptoCurrencyCode} is not legal tender because it is not (yet) recognized in any country as such.
                Since {transaction.CryptoCurrencyCode} is used for the same purposes as money, it should be treated equally (based on the principle of neutrality).
                As such (and for the time being), {BaseModel.ApplicationName} considers the servicing of purchase and sale of {transaction.CryptoCurrencyCode} to be exempt for VAT purposes.</i><br />";

            await emailSender.SendEmailAsync(email, CC, BCC, subject, body);

            return (subject, body);
        }

    }
}
