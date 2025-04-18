using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BOJ0043_App.Services
{
    /// <summary>
    /// Základní třída pro API služby
    /// </summary>
    public class ApiService
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;
        
        public ApiService(string baseUrl = "http://localhost:5263/")
        {
            _baseUrl = baseUrl;
            
            // Nastavení handleru pro ignorování problémů s SSL certifikáty
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;
            
            // Vytvoření HTTP klienta s tímto handlerem
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                // V reálné aplikaci bychom řešili chyby lépe (logování, uživatelské zprávy)
                System.Diagnostics.Debug.WriteLine($"Error in GET {endpoint}: {ex.Message}");
                return default;
            }
        }

        protected async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in POST {endpoint}: {ex.Message}");
                return default;
            }
        }

        protected async Task<T?> PostFormAsync<T>(string endpoint, HttpContent content)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in POST FORM {endpoint}: {ex.Message}");
                return default;
            }
        }

        protected async Task<T?> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseContent, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in PUT {endpoint}: {ex.Message}");
                return default;
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}{endpoint}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DELETE {endpoint}: {ex.Message}");
                return false;
            }
        }
    }
}
