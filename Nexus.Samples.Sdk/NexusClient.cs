using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;
using Nexus.Samples.Sdk.Models.Shared;

namespace Nexus.Samples.Sdk
{
    public class NexusClient
    {
        private readonly HttpClient _client;
        private readonly NexusConnectionOptions _nexusConnectionConfig;
        private DateTime refreshTime = DateTime.MinValue;
        private string token;

        public NexusClient(IOptions<NexusConnectionOptions> nexusConnectionConfiguration)
        {
            _nexusConnectionConfig = nexusConnectionConfiguration.Value;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_nexusConnectionConfig.ApiUrl);
            _client.DefaultRequestHeaders.Add("api_version", "1.2");
        }

        private async Task PotentialTokenRefresh()
        {
            if (refreshTime < DateTime.UtcNow.AddMinutes(-45)
                || string.IsNullOrWhiteSpace(token))
            {
                token = await GetNewToken();

                if (token != null)
                {
                    Trace.WriteLine("Updated Token");

                    _client.SetBearerToken(token);
                    refreshTime = DateTime.UtcNow;
                }
            }
        }

        private async Task<string> GetNewToken()
        {
            // discover endpoints from metadata
            var disco = await _client.GetDiscoveryDocumentAsync(_nexusConnectionConfig.IdentityUrl);

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return null;
            }

            // request token
            var tokenResponse = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _nexusConnectionConfig.Username,
                ClientSecret = _nexusConnectionConfig.Password,
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return null;
            }

            return tokenResponse.AccessToken;
        }

        public async Task<DefaultResponseTemplate<CreateCustomerResponse>> CreateCustomer(CreateCustomerRequest request)
        {
            var response = await PostRequestAsync("customer", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<CreateCustomerResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetAccountResponse>> CreateAccount(string customerCode, CreateAccountRequest request)
        {
            var response = await PostRequestAsync($"customer/{customerCode}/accounts", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetAccountResponse>>();
        }

        public async Task<DefaultResponseTemplate<SimulateBuyBrokerResponse>> SimulateBuyBroker(SimulateBuyBrokerRequest request)
        {
            var response = await PostRequestAsync("buy/broker/simulate", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<SimulateBuyBrokerResponse>>();
        }

        public async Task<DefaultResponseTemplate<SimulateSellBrokerResponse>> SimulateSellBroker(SimulateSellBrokerRequest request)
        {
            var response = await PostRequestAsync("sell/broker/simulate", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<SimulateSellBrokerResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetAccountResponse>> GetAccount(string accountCode)
        {
            var response = await GetRequestAsync($"accounts/{accountCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetAccountResponse>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetAccountResponse>>> GetAccountsForCustomer(string customerCode)
        {
            var response = await GetRequestAsync($"/customer/{customerCode}/accounts");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetAccountResponse>>>();
        }

        public async Task<DefaultResponseTemplate<EmptyResponse>> ActivateAccount(string accountCode)
        {
            var response = await GetRequestAsync($"accounts/activate/{accountCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<EmptyResponse>>();
        }

        public async Task<DefaultResponseTemplate<EmptyResponse>> DeleteAccount(string accountCode)
        {
            var response = await DeleteRequestAsync($"accounts/{accountCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<EmptyResponse>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetBrokerTransactionResponse>>> GetTransactions(string customerCode)
        {
            return await GetTransactions(customerCode, null);
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetBrokerTransactionResponse>>> GetTransactions(string customerCode, string transactionStatus)
        {
            var queryString = $"transaction?customer={customerCode}";
            if (transactionStatus != null)
            {
                queryString += $"&status={transactionStatus}";
            }
            var response = await GetRequestAsync(queryString);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetBrokerTransactionResponse>>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetBrokerTransactionResponse>>> GetBuyTransactions(string customerCode)
        {
            var response = await GetRequestAsync($"transaction?customer={customerCode}&type=Buy");
            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetBrokerTransactionResponse>>>();
        }

        public async Task<DefaultResponseTemplate<GetBrokerTransactionResponse>> GetTransaction(string code)
        {
            var response = await GetRequestAsync($"transaction/{code}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetBrokerTransactionResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetCustomerResponse>> GetCustomer(string customerCode)
        {
            var response = await GetRequestAsync($"customer/{customerCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetCustomerResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetPricesResponse>> GetPrices(string currencyCode)
        {
            var response = await GetRequestAsync($"prices/{currencyCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetPricesResponse>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetMailResponse>>> GetReadyToSendMails(int page, string typeList = "")
        {
            var response = await GetRequestAsync($"mail?status=ReadyToSend&page={page}&type={typeList}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetMailResponse>>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetCustomerResponse>>> GetCustomersByEmail(string email)
        {
            var response = await GetRequestAsync($"customer?email={email}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetCustomerResponse>>>();
        }

        public async Task<DefaultResponseTemplate<GetMailResponse>> GetMailByCode(string code)
        {
            var response = await GetRequestAsync($"mail/{code}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }


        public async Task<DefaultResponseTemplate<GetMailResponse>> CreateMail(CreateMailRequest request)
        {
            var response = await PostRequestAsync("mail", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetMailResponse>> UpdateMailContent(string code, UpdateMailContentRequest request)
        {
            var response = await PutRequest($"mail/{code}", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetMailResponse>> MailSent(string code)
        {
            var response = await PutRequest($"mail/{code}/sent");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetPaymentMethodResponse>>> GetPaymentMethodInformation(string currency, string crypto, string transactionType)
        {
            var response = await GetRequestAsync($"paymentmethod?currency={currency}&crypto={crypto}&transactionType={transactionType}&limit=100");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetPaymentMethodResponse>>>();
        }

        public async Task<DefaultResponseTemplate<InitiateBrokerBuyResponse>> InitiateBrokerBuy(InitiateBrokerBuyRequest request)
        {
            var response = await PostRequestAsync("/buy/broker", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<InitiateBrokerBuyResponse>>();
        }

        public async Task<DefaultResponseTemplate<InitiateBrokerSellResponse>> InitiateBrokerSell(InitiateBrokerSellRequest request)
        {
            var response = await PostRequestAsync("/sell/broker", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<InitiateBrokerSellResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetBrokerLimitResponse>> GetBrokerBuyLimits(string customerCode, string paymentMethodCode)
        {
            var response = await GetRequestAsync($"customer/{customerCode}/limits/broker/buy?paymentMethodCode={paymentMethodCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetBrokerLimitResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetBrokerLimitResponse>> GetBrokerSellLimits(string customerCode, string cryptoCode)
        {
            var response = await GetRequestAsync($"customer/{customerCode}/limits/broker/sell?cryptoCode={cryptoCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetBrokerLimitResponse>>();
        }

        public async Task<HttpResponseMessage> GetRequestAsync(string url)
        {
            await PotentialTokenRefresh();

            return await _client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> DeleteRequestAsync(string url)
        {
            await PotentialTokenRefresh();

            return await _client.DeleteAsync(url);
        }

        public async Task<T> GetRequest<T>(string url)
        {
            await PotentialTokenRefresh();

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            string responseresult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseresult);
        }

        public async Task<HttpResponseMessage> PostRequestAsync<T>(string url, T postData)
        {
            await PotentialTokenRefresh();

            return await _client.PostAsJsonAsync(url, postData);
        }

        public async Task<HttpResponseMessage> PutRequest<T>(string url, T msg)
        {
            await PotentialTokenRefresh();

            var response = await _client.PutAsJsonAsync(url, msg);

            return response;
        }

        public async Task<HttpResponseMessage> PutRequest(string url)
        {
            await PotentialTokenRefresh();

            var response = await _client.PutAsync(url, null);

            return response;
        }
    }
}
