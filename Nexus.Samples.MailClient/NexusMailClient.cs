using Microsoft.Extensions.Options;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;
using Nexus.Samples.Sdk.Models.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nexus.Samples.MailClient
{
    public class NexusMailClient
    {
        private readonly NexusClient _nexusClient;
        private readonly IEmailSender _mailClient;
        private readonly string ApplicationUrl;

        public static string[] SupportMailTypes = new string[]
        {
            "NewAccountRequested",
            "NewAccountActivated",
            "AccountDeletedByRequest",
            "TransactionBuyFinish",
            "TrustLevelUpdated",
            "AccountInfoRequest",
            "AccountDeleteRequested"
        };

        public NexusMailClient(NexusClient nexusClient, IOptions<AuthMessageSenderOptions> credentials)
        {
            _nexusClient = nexusClient;
            _mailClient = new EmailSender(credentials);
            ApplicationUrl = credentials.Value.ApplicationUrl;
        }

        public async Task SendMailsAsync(int page = 1)
        {
            var mailsToSendResponse = await _nexusClient.GetReadyToSendMails(page, String.Join("|", SupportMailTypes));
            var totalPages = mailsToSendResponse.Values.TotalPages;

            if (!mailsToSendResponse.IsSuccess)
            {
                Console.WriteLine("Error occured fetching all ReadyToSend mails from Nexus");
                return;
            }

            var mailsToSend = mailsToSendResponse.Values.Records;

            foreach(var mailToSend in mailsToSend)
            {
                var isMailSuccessfullySent = false;
                var body = "";
                var subject = "";

                switch (mailToSend.Type)
                {
                    case "NewAccountRequested":
                        (subject, body) = await SendNewAccountRequestedMailAsync(mailToSend);
                        isMailSuccessfullySent = true;
                        break;

                    case "NewAccountActivated":
                        (subject, body) = await SendNewAccountActivatedAsync(mailToSend);
                        isMailSuccessfullySent = true;
                        break;

                    case "AccountDeletedByRequest":
                        (subject, body) = await SendAccountDeletedByRequestAsync(mailToSend);
                        isMailSuccessfullySent = true;
                        break;

                    case "TransactionBuyFinish":
                        (subject, body) = await SendTransactionBuyFinishAsync(mailToSend);
                        isMailSuccessfullySent = true;
                        break;

                    case "TrustLevelUpdated":
                        (subject, body) = await SendTrustLevelUpdatedAsync(mailToSend);
                        isMailSuccessfullySent = true;
                        break;

                    case "AccountInfoRequest":
                        (subject, body) = await SendAccountInfoRequestedAsync(mailToSend);
                        isMailSuccessfullySent = true;
                        break;

                    case "AccountDeleteRequested":
                        (subject, body) = await SendAccountDeleteRequestedMailAsync(mailToSend);
                        isMailSuccessfullySent = true;
                        break;

                    default:
                        Console.WriteLine("Mail Type not supported");
                        break;
                }

                if (isMailSuccessfullySent)
                {
                    var updateResponse = await _nexusClient.UpdateMailContent(mailToSend.Code, new UpdateMailContentRequest
                    {
                        Content = new MailContent
                        {
                            Html = body,
                            Subject = subject
                        },

                    });

                    if (!updateResponse.IsSuccess)
                    {
                        Console.WriteLine($"Error occured updating Mail with code: {mailToSend.Code}.");
                    }

                    var sentResponse = await _nexusClient.MailSent(mailToSend.Code);

                    if (!sentResponse.IsSuccess)
                    {
                        Console.WriteLine($"Error occured updating Mail with code: {mailToSend.Code} as sent.");
                    }
                }

                if (totalPages > page)
                {
                    await SendMailsAsync(page++);
                }
            }
        }

        private async Task<(string, string)> SendNewAccountRequestedMailAsync(GetMailResponse mail)
        {
            var accountCode = mail.References.AccountCode;
            var accountResponse = await _nexusClient.GetAccount(accountCode);

            if (!accountResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Account with code: {accountCode}");
            }

            var activationLink = ApplicationUrl + "/account/activate?code=" + mail.Code;
            var account = accountResponse.Values;

            var cc = mail.Recipient.CC?.Split(',').ToList();
            var bcc = mail.Recipient.BCC?.Split(',').ToList();

            var result = await _mailClient.SendNewAccountRequestedAsync(mail.Recipient.Email, cc, bcc, activationLink, account);

            return result;
        }

        private async Task<(string, string)> SendNewAccountActivatedAsync(GetMailResponse mail)
        {
            var accountCode = mail.References.AccountCode;
            var accountResponse = await _nexusClient.GetAccount(accountCode);

            if (!accountResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Account with code: {accountCode}");
            }

            var account = accountResponse.Values;

            var cc = mail.Recipient.CC?.Split(',').ToList();
            var bcc = mail.Recipient.BCC?.Split(',').ToList();

            var result = await _mailClient.SendNewAccountActivatedAsync(mail.Recipient.Email, cc, bcc, account);

            return result;
        }

        private async Task<(string, string)> SendAccountDeletedByRequestAsync(GetMailResponse mail)
        {
            var accountCode = mail.References.AccountCode;
            var accountResponse = await _nexusClient.GetAccount(accountCode);

            if (!accountResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Account with code: {accountCode}");
            }

            var account = accountResponse.Values;

            var cc = mail.Recipient.CC?.Split(',').ToList();
            var bcc = mail.Recipient.BCC?.Split(',').ToList();

            var result = await _mailClient.SendAccountDeletedByRequestAsync(mail.Recipient.Email, cc, bcc, account);

            return result;
        }

        private async Task<(string, string)> SendTransactionBuyFinishAsync(GetMailResponse mail)
        {
            var transactionCode = mail.References.TransactionCode;
            var transactionResponse = await _nexusClient.GetTransaction(transactionCode);

            if (!transactionResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Transaction with code: {transactionCode}");
            }

            var transaction = transactionResponse.Values;

            var accountCode = mail.References.AccountCode;
            var accountResponse = await _nexusClient.GetAccount(accountCode);

            if (!accountResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Account with code: {accountCode}");
            }

            var account = accountResponse.Values;

            var cc = mail.Recipient.CC?.Split(',').ToList();
            var bcc = mail.Recipient.BCC?.Split(',').ToList();

            var result = await _mailClient.SendTransactionBuyFinishAsync(mail.Recipient.Email, cc, bcc, account, transaction);

            return result;
        }

        private async Task<(string, string)> SendTrustLevelUpdatedAsync(GetMailResponse mail)
        {
            var customerCode = mail.References.CustomerCode;
            var customerResponse = await _nexusClient.GetCustomer(customerCode);

            if (!customerResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Customer with code: {customerCode}");
            }

            var accountsResponse = await _nexusClient.GetAccountsForCustomer(customerCode);

            if (!accountsResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching accounts for Customer with code: {customerCode}");
            }

            var customer = customerResponse.Values;
            var accounts = accountsResponse.Values.Records.ToList();

            var cc = mail.Recipient.CC?.Split(',').ToList();
            var bcc = mail.Recipient.BCC?.Split(',').ToList();

            var result = await _mailClient.SendTrustLevelUpdatedAsync(mail.Recipient.Email, cc, bcc, customer, accounts);

            return result;
        }

        private async Task<(string, string)> SendAccountInfoRequestedAsync(GetMailResponse mail)
        {
            var customerCode = mail.References.CustomerCode;
            var customerResponse = await _nexusClient.GetCustomer(customerCode);

            if (!customerResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Customer with code: {customerCode}");
            }

            var transactionsResponse = await _nexusClient.GetTransactions(customerCode);

            if (!transactionsResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching transactions for customer with code: {customerCode}");
            }

            var accountsResponse = await _nexusClient.GetAccountsForCustomer(customerCode);

            if (!accountsResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching accounts for customer with code: {customerCode}");
            }

            var customer = customerResponse.Values;
            var transactions = transactionsResponse.Values.Records.ToList();
            var accounts = accountsResponse.Values.Records.ToList();

            var cc = mail.Recipient.CC?.Split(',').ToList();
            var bcc = mail.Recipient.BCC?.Split(',').ToList();

            var result = await _mailClient.SendAccountInfoRequestedAsync(mail.Recipient.Email, cc, bcc, accounts, transactions, customer);

            return result;
        }

        private async Task<(string, string)> SendAccountDeleteRequestedMailAsync(GetMailResponse mail)
        {
            var accountCode = mail.References.AccountCode;
            var accountResponse = await _nexusClient.GetAccount(accountCode);

            if (!accountResponse.IsSuccess)
            {
                Console.WriteLine($"Error occured fetching Account with code: {accountCode}");
            }

            var activationLink = ApplicationUrl + "/account/deleted?code=" + mail.Code;
            var account = accountResponse.Values;

            var cc = mail.Recipient.CC?.Split(',').ToList();
            var bcc = mail.Recipient.BCC?.Split(',').ToList();

            var result = await _mailClient.SendAccountDeleteRequestedAsync(mail.Recipient.Email, cc, bcc, activationLink, account);

            return result;
        }
    }
}
