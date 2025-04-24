using BOJ0043_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOJ0043_App.Services
{
    public class CoworkingSpaceService : ApiService
    {
        private const string _endpoint = "api/coworkingspace";

        public CoworkingSpaceService(string baseUrl = "http://localhost:5263/") : base(baseUrl)
        {
        }

        public async Task<List<CoworkingSpace>?> GetAllCoworkingSpacesAsync()
        {
            try
            {
                // Zkusíme nejprve standardní API endpoint
                var result = await GetAsync<List<CoworkingSpace>>(_endpoint);
                if (result != null)
                    return result;

                // Pokud standardní endpoint nefunguje, zkusíme MVC controller
                return await GetAsync<List<CoworkingSpace>>("CoworkingSpace/GetAll");
            }
            catch
            {
                // Pokud první pokus selže, zkusíme alternativní endpoint
                return await GetAsync<List<CoworkingSpace>>("CoworkingSpace/GetAll");
            }
        }

        public async Task<CoworkingSpace?> GetCoworkingSpaceByIdAsync(int id)
        {
            return await GetAsync<CoworkingSpace>($"{_endpoint}/{id}");
        }

        public async Task<CoworkingSpace?> CreateCoworkingSpaceAsync(CoworkingSpace coworkingSpace)
        {
            // Use the correct endpoint for creating a coworking space
            return await PostAsync<CoworkingSpace>("CoworkingSpace/CreateApi", coworkingSpace);
        }

        public async Task<CoworkingSpace?> UpdateCoworkingSpaceAsync(int id, CoworkingSpace coworkingSpace)
        {
            // Use the new JSON API endpoint for WPF
            return await PutAsync<CoworkingSpace>($"CoworkingSpace/Update/{id}", coworkingSpace);
        }

        public async Task<bool> DeleteCoworkingSpaceAsync(int id)
        {
            return await DeleteAsync($"CoworkingSpace/DeleteApi/{id}");
        }

        public async Task<CoworkingSpace?> GetWithWorkspacesAsync(int id)
        {
            System.Diagnostics.Debug.WriteLine($"DEBUG: Fetching CoworkingSpace with workspaces for id={id}");
            var result = await GetAsync<CoworkingSpace>($"CoworkingSpace/GetWithWorkspacesJson/{id}");
            System.Diagnostics.Debug.WriteLine($"DEBUG: Result is null? {result == null}, Workspaces count: {result?.Workspaces?.Count ?? -1}");
            return result;
        }
    }
}
