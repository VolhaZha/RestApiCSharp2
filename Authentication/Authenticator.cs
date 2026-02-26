using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace RestApiCSharp.Authentication
{
    public class Authenticator
    {
        readonly string _baseUrl;
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _scope;
        private string _token = string.Empty;

        public Authenticator(string baseUrl, string clientId, string clientSecret, string scope)
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scope = scope;
        }

        public async Task<string> GetAuthenticationHeaderAsync()
        {
            _token = string.IsNullOrEmpty(_token) ? await GetTokenAsync() : _token;
            return _token;
        }

        private async Task<string> GetTokenAsync()
        {
            using var client = new HttpClient { BaseAddress = new System.Uri(_baseUrl) };

            var byteArray = Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var content = new FormUrlEncodedContent
                (
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", _scope)
            ]);

            var response = await client.PostAsync("oauth/token", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            if (tokenResponse == null)
                throw new System.InvalidOperationException("Failed to retrieve token");

            return $"{tokenResponse.TokenType} {tokenResponse.AccessToken}";
        }

        private class TokenResponse
        {
            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
        }
    }
}
