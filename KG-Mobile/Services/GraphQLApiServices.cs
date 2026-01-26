using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Models.GraphQLAPI_Response_Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

internal class GraphQLApiServices
{
    private readonly HttpClient _httpClient;

    public GraphQLApiServices()
    {
        _httpClient = new HttpClient();
        // Optional: set a default timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
    }

    /// <summary>
    /// Logs in to a GraphQL API using username and password.
    /// Returns the access token as string if successful, or null if failed.
    /// </summary>
    public async Task<string?> LoginAsync(string username, string password)
    {
        try
        {
            var endpoint = Settings.GraphQLApiSecurityUrl;

            // GraphQL login query (adjust to your schema)
            var query = new
            {
                query = @"
                    query Login($username: String!, $password: String!) {
                        login(data: { username: $username, password: $password }) {
                            message
                            token
                        }
                    }",
                variables = new
                {
                    username = username,
                    password = password
                }
            };

            // Serialize the query to JSON
            var json = JsonSerializer.Serialize(query);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send POST request
            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            // Read response
            var responseString = await response.Content.ReadAsStringAsync();

            // Parse JSON response
            using var doc = JsonDocument.Parse(responseString);
            if (doc.RootElement.TryGetProperty("data", out var dataElement) &&
                dataElement.TryGetProperty("login", out var loginElement) &&
                loginElement.TryGetProperty("token", out var tokenElement))
            {
                Settings.AccessToken = tokenElement.GetString();
                return Settings.AccessToken;
            }

            return null; // failed to get token
        }
        catch (HttpRequestException httpEx)
        {
            // Network or HTTP error
            Console.WriteLine($"HTTP Error: {httpEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            // Catch all other exceptions
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Checks if the current bearer token is valid.
    /// </summary>
    public async Task LoggedInCheckAsync()
    {
        //try
        //{
        //    // GraphQL query for user info
        //    var query = new
        //    {
        //        query = @"
        //            query GetUserInfo {
        //                userInfo {
        //                    id
        //                    username
        //                    email
        //                }
        //            }"
        //    };

        //    var json = JsonSerializer.Serialize(query);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var request = new HttpRequestMessage(HttpMethod.Post, Settings.GraphQLApiSecurityUrl);
        //    request.Content = content;
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Settings.AccessToken);

        //    var response = await _httpClient.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    var responseString = await response.Content.ReadAsStringAsync();

        //    // Parse response
        //    using var doc = JsonDocument.Parse(responseString);
        //    if (doc.RootElement.TryGetProperty("data", out var dataElement) &&
        //        dataElement.TryGetProperty("userInfo", out var userInfoElement))
        //    {
        //        // Optionally, you can deserialize into a strongly typed object
        //        var token = new AuthToken();
        //        MessagingCenter.Send(token, "AlreadyLoggedIn");
        //    }
        //}
        //catch (Exception)
        //{
        //    // Ignore errors; will continue to normal login
        //}
        //finally
        //{
        //    // Hide Busy
        //    msg.visible = false;
        //    MessagingCenter.Send(msg, "BusyPopup");
        //}
    }
    public async Task<object> ExecuteAsync<T>(
            string queryOrMutation,
            object variables = null)
    {
        try
        {

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Settings.AccessToken);

            var payload = new
            {
                query = queryOrMutation,
                variables
            };

            var json = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(Settings.GraphQLApiUrl, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new PopupMessage(
                    "GraphQL Failed",
                    "GraphQLService",
                    responseJson,
                    "Ok"
                );
            }

            var result = JsonSerializer.Deserialize<GraphQLResponse<T>>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
            {
                return new PopupMessage(
                    "GraphQL Error",
                    "GraphQLService",
                    "No response from server",
                    "Ok"
                );
            }

            if (result.HasErrors)
            {
                var msg = string.Join("\n", result.Errors.Select(e => e.Message));
                return new PopupMessage(
                    "GraphQL Error",
                    "GraphQLService",
                    msg,
                    "Ok"
                );
            }

            return result.Data;
        }
        catch (Exception ex)
        {
            return new PopupMessage(
                "GraphQL Exception",
                "GraphQLService",
                ex.Message,
                "Ok"
            );
        }
    }
}
