using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PhoneShopApp.FE.Web.Models.Auth;
using PhoneShopApp.FE.Web.Models.Phones;
using PhoneShopApp.FE.Web.Models.Users;

namespace PhoneShopApp.FE.Web.Services;

public class BackendApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public BackendApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LoginResponseViewModel?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        var payload = JsonSerializer.Serialize(new { username, password });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

// 
        var response = await client.PostAsync("/api/auth/login", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<LoginResponseViewModel>(body, JsonOptions);
    }

    public async Task<List<PhoneViewModel>> SearchPhonesAsync(string? keyword, string? searchBy, string token, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query.Add($"keyword={Uri.EscapeDataString(keyword)}");
        }

        if (!string.IsNullOrWhiteSpace(searchBy))
        {
            query.Add($"searchBy={Uri.EscapeDataString(searchBy)}");
        }

        var path = "/api/phones";
        if (query.Count > 0)
        {
            path += $"?{string.Join("&", query)}";
        }

        var response = await client.GetAsync(path, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new List<PhoneViewModel>();
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<PhoneViewModel>>(body, JsonOptions) ?? new List<PhoneViewModel>();
    }

    public async Task<bool> CreatePhoneAsync(CreatePhoneViewModel model, string token, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = JsonSerializer.Serialize(model);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/phones", content, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePhoneAsync(int id, CreatePhoneViewModel model, string token, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = JsonSerializer.Serialize(model);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"/api/phones/{id}", content, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePhoneStatusAsync(int id, string action, string token, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = JsonSerializer.Serialize(new { action });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/phones/{id}/status")
        {
            Content = content
        };

        var response = await client.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePhoneAsync(int id, string token, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync($"/api/phones/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<UserViewModel>> GetUsersAsync(string token, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/users", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new List<UserViewModel>();
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<UserViewModel>>(body, JsonOptions) ?? new List<UserViewModel>();
    }

    public async Task<bool> CreateUserAsync(CreateUserViewModel model, string token, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = JsonSerializer.Serialize(model);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/users", content, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
