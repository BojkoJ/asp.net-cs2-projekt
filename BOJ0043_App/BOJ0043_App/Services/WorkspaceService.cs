using BOJ0043_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOJ0043_App.Services
{
    public class WorkspaceService : ApiService
    {
        private const string _endpoint = "api/workspace";

        public WorkspaceService(string baseUrl = "http://localhost:5263/") : base(baseUrl)
        {
        }

        public async Task<List<Workspace>?> GetAllWorkspacesAsync()
        {
            try
            {
                // Zkusíme nejprve standardní API endpoint
                var result = await GetAsync<List<Workspace>>(_endpoint);
                if (result != null)
                    return result;

                // Pokud standardní endpoint nefunguje, zkusíme MVC controller
                return await GetAsync<List<Workspace>>("Workspace/GetAll");
            }
            catch
            {
                // Pokud první pokus selže, zkusíme alternativní endpoint
                return await GetAsync<List<Workspace>>("Workspace/GetAll");
            }
        }
        public async Task<List<Workspace>?> GetWorkspacesByCoworkingSpaceIdAsync(int coworkingSpaceId)
        {
            try
            {
                // Přímé volání na MVC controller s přesným endpointem
                System.Diagnostics.Debug.WriteLine($"Volám endpoint: Workspace/GetByCoworkingSpaceId?coworkingSpaceId={coworkingSpaceId}");
                var result = await GetAsync<List<Workspace>>($"Workspace/GetByCoworkingSpaceId?coworkingSpaceId={coworkingSpaceId}");

                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Úspěšně načteno {result.Count} pracovních míst");
                    return result;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Endpoint nevrátil žádná data");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Zalogujeme chybu pro debugging
                System.Diagnostics.Debug.WriteLine($"Chyba při načítání pracovních míst: {ex.Message}");
                return null;
            }
        }

        public async Task<Workspace?> GetWorkspaceByIdAsync(int id)
        {
            try
            {
                // Zkusíme nejprve standardní API endpoint
                var result = await GetAsync<Workspace>($"{_endpoint}/{id}");
                if (result != null)
                    return result;

                // Pokud standardní endpoint nefunguje, zkusíme MVC controller
                return await GetAsync<Workspace>($"Workspace/GetById/{id}");
            }
            catch
            {
                // Pokud první pokus selže, zkusíme alternativní endpoint
                return await GetAsync<Workspace>($"Workspace/GetById/{id}");
            }
        }
        public async Task<Workspace?> CreateWorkspaceAsync(Workspace workspace)
        {
            try
            {
                // Zkusíme nejprve standardní API endpoint
                var result = await PostAsync<Workspace>("Workspace/CreateApi", workspace);
                return result;
            }
            catch
            {
                // Pokud první pokus selže, zkusíme znova
                return await PostAsync<Workspace>("Workspace/CreateApi", workspace);
            }
        }


        public async Task<Workspace?> UpdateWorkspaceAsync(int id, Workspace workspace)
        {
            return await PutAsync<Workspace>($"{_endpoint}/{id}", workspace);
        }

        public async Task<bool> ChangeWorkspaceStatusAsync(int id, string status)
        {
            return await PutAsync<bool>($"{_endpoint}/{id}/changestatus", new { status });
        }

        public async Task<bool> DeleteWorkspaceAsync(int id)
        {
            return await DeleteAsync($"WorkSpace/DeleteApi/{id}");
        }

        public async Task<List<WorkspaceStatusHistory>?> GetStatusHistoryAsync(int workspaceId)
        {
            // Assumes API endpoint: Workspace/GetStatusHistory?workspaceId={workspaceId}
            return await GetAsync<List<WorkspaceStatusHistory>>($"Workspace/GetStatusHistory?workspaceId={workspaceId}");
        }

        // Helper to map Czech status to enum string
        private string MapCzechStatusToEnum(string status)
        {
            switch (status?.Trim())
            {
                case "Dostupné":
                    return "Available";
                case "V údržbě":
                    return "Maintenance";
                case "Obsazené":
                    return "Occupied";
                default:
                    return status; // fallback, may cause API error if not valid
            }
        }

        public async Task<(bool Success, string? Error)> ChangeWorkspaceStatusWithErrorAsync(int id, string status, string comment)
        {
            // Map Czech status to enum value
            var enumStatus = MapCzechStatusToEnum(status);
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("newStatus", enumStatus),
                new KeyValuePair<string, string>("comment", comment ?? string.Empty)
            };
            var content = new System.Net.Http.FormUrlEncodedContent(formData);
            var response = await PostFormAsync<BOJ0043_App.Models.ApiSuccessResponse>("Workspace/ChangeStatusApi", content);
            if (response != null && response.Success)
                return (true, null);
            return (false, response?.Error);
        }

        public async Task<bool> ChangeWorkspaceStatusAsync(int id, string status, string comment)
        {
            var (success, _) = await ChangeWorkspaceStatusWithErrorAsync(id, status, comment);
            return success;
        }
        public async Task<bool> UpdateWorkspaceApiAsync(int id, Workspace workspace)
        {
            var result = await PutAsync<Workspace>($"Workspace/Update/{id}", workspace);
            return result != null;
        }
    }
}
