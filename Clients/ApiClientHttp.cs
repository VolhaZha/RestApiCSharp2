using Newtonsoft.Json;
using RestApiCSharp.Authentication;
using RestApiCSharp.ConstantsTestingGeneral;
using RestApiCSharp.Models;
using System.Net.Http.Headers;
using System.Text;

namespace RestApiCSharp.Clients
{
    public class ApiClientHttp
    {
        private static ApiClientHttp? _instance;

        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly HttpClient _client;
        private string _accessToken = string.Empty;

        private ApiClientHttp (string baseUrl, string clientId, string clientSecret)
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _client = new HttpClient();
        }

        public static void Initialize(string baseUrl, string clientId, string clientSecret)
        {
            if (_instance == null)
                _instance = new ApiClientHttp(baseUrl, clientId, clientSecret);
        }

        public static ApiClientHttp GetInstance ()
        {
            if (_instance == null)
                throw new InvalidOperationException("ApiClientHttp not initialized.");
            return _instance;
        }

        public async Task SetClientScopeAsync(string scope)
        {
            var authenticator = new Authenticator(_baseUrl, _clientId, _clientSecret, scope);
            _accessToken = await authenticator.GetAuthenticationHeaderAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _accessToken.Split(' ')[1]);
        }

        private StringContent GetJsonContent<T>(T obj) =>
            new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        public async Task<HttpResponseMessage> ExpandZipCodes(string scope, List<string> zipCodes)
        {
            await SetClientScopeAsync(scope);

            var json = JsonConvert.SerializeObject(zipCodes);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"{ConstantsTesting.BaseUrl}/zip-codes/expand";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = data
            };

            var response = await _client.SendAsync(requestMessage);

            return response;
        }

        public async Task<HttpResponseMessage> GetZipCodes(string scope)
        {
            await SetClientScopeAsync(scope);

            var url = $"{_baseUrl}/zip-codes";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _client.SendAsync(requestMessage);

            return response;
        }

        public async Task<HttpResponseMessage> CreateUsers (string scope, User user)
        {
            await SetClientScopeAsync(scope);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null.");
            }

            var url = $"{_baseUrl}/users";
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            try
            {
                var response = await _client.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Request failed with status code {response.StatusCode}. Response content: {responseContent}");
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed with exception: {ex.Message}");

                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public async Task CreateUsersList(string scope, List<User> users)
        {
            foreach (var user in users)
            {
                await CreateUsers(scope, user);
            }
        }

        public async Task<HttpResponseMessage> GetUsers(string scope, List<(string name, string value)>? parameters = null)
        {
            await SetClientScopeAsync(scope);

            var url = $"{_baseUrl}/users";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            AddParameters(requestMessage, parameters);

            var response = await _client.SendAsync(requestMessage);

            return response;
        }

        public void AddParameters(HttpRequestMessage request, List<(string name, string value)>? parameters = null)
        {
            if (parameters == null || parameters.Count == 0) return;

            var queryString = string.Join("&", parameters.Select(p =>
                $"{Uri.EscapeDataString(p.name)}={Uri.EscapeDataString(p.value)}"));

            if (request.RequestUri?.Query.Length > 0)
            {
                request.RequestUri = new Uri($"{request.RequestUri}&{queryString}");
            }
            else
            {
                request.RequestUri = new Uri($"{request.RequestUri}?{queryString}");
            }
        }

        public async Task<HttpResponseMessage> UpdateUsersPatch(string scope, UserUpdate user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User object cannot be null.");

            await SetClientScopeAsync(scope);
            var url = $"{_baseUrl}/users";
            var content = GetJsonContent(user);

            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = content
            };

            try
            {
                var response = await _client.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}. Response content: {response.Content}");

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed with exception: {ex.Message}");

                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public async Task<HttpResponseMessage> UpdateUsersPut(string scope, UserUpdate user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null.");
            }

            await SetClientScopeAsync(scope);
            var url = $"{_baseUrl}/users";
            var content = GetJsonContent(user);

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = content
            };

            try
            {
                var response = await _client.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Request failed with status code {response.StatusCode}. Response content: {response.Content}");
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed with exception: {ex.Message}");

                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        public async Task<HttpResponseMessage> DeleteUser(string scope, User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null.");
            }

            await SetClientScopeAsync(scope);

            var url = $"{_baseUrl}/users";
            var content = GetJsonContent(user);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = content
            };

            try
            {
                var response = await _client.SendAsync(requestMessage); ;

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Request failed with status code {response.StatusCode}. Response content: {response.Content}");
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed with exception: {ex.Message}");

                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }
    }
}
