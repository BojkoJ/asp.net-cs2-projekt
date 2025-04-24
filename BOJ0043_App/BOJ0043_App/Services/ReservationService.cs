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
                var result = await GetAsync<List<Reservation>>(_endpoint);
                if (result != null)
                    return result;

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
            return await PostAsync<Reservation>($"Reservation/CreateApi", reservation);
        }


        public async Task<List<Reservation>?> GetActiveReservationsByWorkspaceIdAsync(int workspaceId)
        {
            // Use query string parameter to match backend controller
            return await GetAsync<List<Reservation>>($"Reservation/GetActiveByWorkspaceId?workspaceId={workspaceId}");
        }

        public async Task<bool> CompleteReservationAsync(int id)
        {
            // Calls the new API endpoint to complete (close) a reservation
            var response = await PostAsync<Models.ApiSuccessResponse>("Reservation/CompleteApi/" + id, null);
            return response != null && response.Success;
        }

        public async Task<List<Views.StatisticItem>?> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            // Calls the public API endpoint for statistics

            // Endpoint expects StartDate and EndDate as query string parameters
            var result = await PostAsync<Dictionary<string, int>>($"Reservation/StatisticsApi?StartDate={startDate:yyyy-MM-dd}&EndDate={endDate:yyyy-MM-dd}", null);
            if (result == null) return null;
            var list = new List<Views.StatisticItem>();
            foreach (var kv in result)
                list.Add(new Views.StatisticItem { Name = kv.Key, Count = kv.Value });
            return list;
        }
    }
}
