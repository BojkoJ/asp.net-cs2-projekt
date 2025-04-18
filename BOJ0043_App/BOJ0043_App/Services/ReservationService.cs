using BOJ0043_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOJ0043_App.Services
{
    public class ReservationService : ApiService
    {
        private const string _endpoint = "api/reservation";

        public ReservationService(string baseUrl = "http://localhost:5263/") : base(baseUrl)
        {
        }

        public async Task<List<Reservation>?> GetAllReservationsAsync()
        {
            try
            {
                // Zkusíme nejprve standardní API endpoint
                var result = await GetAsync<List<Reservation>>(_endpoint);
                if (result != null)
                    return result;
                
                // Pokud standardní endpoint nefunguje, zkusíme MVC controller
                return await GetAsync<List<Reservation>>("Reservation/GetAll");
            }
            catch
            {
                // Pokud první pokus selže, zkusíme alternativní endpoint
                return await GetAsync<List<Reservation>>("Reservation/GetAll");
            }
        }

        public async Task<List<Reservation>?> GetReservationsByWorkspaceIdAsync(int workspaceId)
        {
            try
            {
                // Přímé volání na MVC controller s přesným endpointem
                System.Diagnostics.Debug.WriteLine($"Volám endpoint: Reservation/GetByWorkspaceId?workspaceId={workspaceId}");
                var result = await GetAsync<List<Reservation>>($"Reservation/GetByWorkspaceId?workspaceId={workspaceId}");
                
                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Úspěšně načteno {result.Count} rezervací");
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
                System.Diagnostics.Debug.WriteLine($"Chyba při načítání rezervací: {ex.Message}");
                return null;
            }
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            

            // Zkusíme nejprve standardní API endpoint
            var result = await GetAsync<Reservation>($"{_endpoint}/{id}");
            if (result != null)
                return result;
            
            // Pokud standardní endpoint nefunguje, zkusíme MVC controller
            return await GetAsync<Reservation>($"Reservation/GetById/{id}");

        }

        public async Task<Reservation?> CreateReservationAsync(Reservation reservation)
        {
            return await PostAsync<Reservation>(_endpoint, reservation);
        }

        public async Task<Reservation?> UpdateReservationAsync(int id, Reservation reservation)
        {
            return await PutAsync<Reservation>($"{_endpoint}/{id}", reservation);
        }

        public async Task<bool> ChangeReservationStatusAsync(int id, string status)
        {
            return await PutAsync<bool>($"{_endpoint}/{id}/changestatus", new { status });
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            return await DeleteAsync($"{_endpoint}/{id}");
        }

        public async Task<List<Reservation>?> GetActiveReservationsByWorkspaceIdAsync(int workspaceId)
        {
            // Use query string parameter to match backend controller
            return await GetAsync<List<Reservation>>($"Reservation/GetActiveByWorkspaceId?workspaceId={workspaceId}");
        }
    }
}
