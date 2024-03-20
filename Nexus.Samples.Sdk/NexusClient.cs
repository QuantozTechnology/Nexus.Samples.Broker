using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nexus.Samples.Sdk.Models.Request;
using Nexus.Samples.Sdk.Models.Response;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nexus.Samples.Sdk
{
    public class ConnectionConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
        public string IdentityUrl { get; set; }
        public static ConnectionConfiguration GetAPIConnectionSettings(IConfiguration configuration)
        {
            var username = configuration["dcpartnerapi_username"];
            var password = configuration["dcpartnerapi_password"];
            var url = configuration["dcpartnerapi_url"];
            var identity = configuration["dcpartnerapi_identity_url"];

            return new ConnectionConfiguration()
            {
                Username = username,
                Password = password,
                URL = url,
                IdentityUrl = identity
            };
        }
    }

    public class NexusClient
    {
        private readonly HttpClient client11;
        private readonly HttpClient client12;
        private readonly ConnectionConfiguration config;
        private DateTime refreshTime = DateTime.MinValue;
        private string token;

        public NexusClient(IConfiguration configuration)
        {
            config = ConnectionConfiguration.GetAPIConnectionSettings(configuration);

            client11 = new HttpClient();
            client11.BaseAddress = new Uri(config.URL);
            client11.DefaultRequestHeaders.Add("api_version", "1.1");

            client12 = new HttpClient();
            client12.BaseAddress = new Uri(config.URL);
            client12.DefaultRequestHeaders.Add("api_version", "1.2");
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

                    client11.SetBearerToken(token);
                    client12.SetBearerToken(token);
                    refreshTime = DateTime.UtcNow;
                }
            }
        }

        private async Task<string> GetNewToken()
        {
            // discover endpoints from metadata
            var disco = await client11.GetDiscoveryDocumentAsync(config.IdentityUrl);

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return null;
            }

            // request token
            var tokenResponse = await client11.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = config.Username,
                ClientSecret = config.Password,
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return null;
            }

            return tokenResponse.AccessToken;
        }

        //[Obsolete("Use other methods to lower the chance of exceptions")]
        public async Task<T> GetRequestAsync<T>(string url, string argument = "")
        {
            await PotentialTokenRefresh();

            HttpResponseMessage response = await client11.GetAsync(url + argument);

            //try
            //{
            //    response.EnsureSuccessStatusCode();
            //}
            //catch (HttpRequestException ex)
            //{
            //    Trace.WriteLine(ex.ToString());
            //}

            string responseresult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseresult);
        }

        public async Task<DefaultResponseTemplate<CreateCustomerResponse>> CreateCustomer(CreateCustomerRequest request)
        {
            var response = await PostRequest12Async("customer", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<CreateCustomerResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetAccountResponse>> CreateAccount(string customerCode, CreateAccountRequest request)
        {
            var response = await PostRequest12Async($"customer/{customerCode}/accounts", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetAccountResponse>>();
        }

        public async Task<DefaultResponseTemplate<SimulateBuyBrokerResponse>> SimulateBuyBroker(SimulateBuyBrokerRequest request)
        {
            var response = await PostRequest12Async("buy/broker/simulate", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<SimulateBuyBrokerResponse>>();
        }

        public async Task<DefaultResponseTemplate<SimulateSellBrokerResponse>> SimulateSellBroker(SimulateSellBrokerRequest request)
        {
            var response = await PostRequest12Async("sell/broker/simulate", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<SimulateSellBrokerResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetAccountResponse>> GetAccount(string accountCode)
        {
            var response = await GetRequest12Async($"accounts/{accountCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetAccountResponse>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetAccountResponse>>> GetAccountsForCustomer(string customerCode)
        {
            var response = await GetRequest12Async($"/customer/{customerCode}/accounts");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetAccountResponse>>>();
        }

        public async Task<DefaultResponseTemplate<EmptyResponse>> ActivateAccount(string accountCode)
        {
            var response = await GetRequest12Async($"accounts/activate/{accountCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<EmptyResponse>>();
        }

        public async Task<DefaultResponseTemplate<EmptyResponse>> DeleteAccount(string accountCode)
        {
            var response = await DeleteRequest12Async($"accounts/{accountCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<EmptyResponse>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetBrokerTransactionResponse>>> GetTransactions(string customerCode)
        {
            var response = await GetRequest12Async($"transaction?customer={customerCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetBrokerTransactionResponse>>>();
        }

        public async Task<DefaultResponseTemplate<GetBrokerTransactionResponse>> GetTransaction(string code)
        {
            var response = await GetRequest12Async($"transaction/{code}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetBrokerTransactionResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetCustomerResponse>> GetCustomer(string customerCode)
        {
            var response = await GetRequest12Async($"customer/{customerCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetCustomerResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetPricesResponse>> GetPrices(string currencyCode)
        {
            var response = await GetRequest12Async($"prices/{currencyCode}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetPricesResponse>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetMailResponse>>> GetReadyToSendMails(int page, string typeList = "")
        {
            var response = await GetRequest12Async($"mail?status=ReadyToSend&page={page}&type={typeList}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetMailResponse>>>();
        }

        public async Task<DefaultResponseTemplate<PagedResult<GetCustomerResponse>>> GetCustomersByEmail(string email)
        {
            var response = await GetRequest12Async($"customer?email={email}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<PagedResult<GetCustomerResponse>>>();
        }

        public async Task<DefaultResponseTemplate<GetMailResponse>> GetMailByCode(string code)
        {
            var response = await GetRequest12Async($"mail/{code}");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }


        public async Task<DefaultResponseTemplate<GetMailResponse>> CreateMail(CreateMailRequest request)
        {
            var response = await PostRequest12Async("mail", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetMailResponse>> UpdateMailContent(string code, UpdateMailContentRequest request)
        {
            var response = await PutRequest12($"mail/{code}", request);

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }

        public async Task<DefaultResponseTemplate<GetMailResponse>> MailSent(string code)
        {
            var response = await PutRequest12($"mail/{code}/sent");

            return await response.Content.ReadAsAsync<DefaultResponseTemplate<GetMailResponse>>();
        }

        public async Task<HttpResponseMessage> GetRequest12Async(string url)
        {
            await PotentialTokenRefresh();

            return await client12.GetAsync(url);
        }

        public async Task<HttpResponseMessage> DeleteRequest12Async(string url)
        {
            await PotentialTokenRefresh();

            return await client12.DeleteAsync(url);
        }

        public async Task<T> GetRequest12<T>(string url)
        {
            await PotentialTokenRefresh();

            var response = await client12.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            string responseresult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseresult);
        }

        public async Task<HttpResponseMessage> PostRequest12Async<T>(string url, T postData)
        {
            await PotentialTokenRefresh();

            return await client12.PostAsJsonAsync(url, postData);
        }

        public async Task<HttpResponseMessage> PutRequest12<T>(string url, T msg)
        {
            await PotentialTokenRefresh();

            var response = await client12.PutAsJsonAsync(url, msg);

            return response;
        }

        public async Task<HttpResponseMessage> PutRequest12(string url)
        {
            await PotentialTokenRefresh();

            var response = await client12.PutAsync(url, null);

            return response;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, string argument = "")
        {
            await PotentialTokenRefresh();

            HttpResponseMessage response = await client11.GetAsync(url + argument);

            return response;
        }

        public async Task<HttpResponseMessage> PostRequestAsync<T>(string url, T postData, string argument = "")
        {
            await PotentialTokenRefresh();

            return await client11.PostAsJsonAsync(url + argument, postData);
        }

        public async Task<T> PostAndGetRequestAsync<T, T2>(string url, T2 postData, string id = "")
        {
            await PotentialTokenRefresh();

            HttpResponseMessage response = await client11.PostAsJsonAsync(url + id, postData);
            var res = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            return await response.Content.ReadAsAsync<T>();
        }
    }
}
