using System.Net.Http.Json;
using MonteCarlo.Core.Models;

namespace MonteCarlo.Web.Services
{
    public class SimulationApiService
    {
        private readonly HttpClient _httpClient;

        public SimulationApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SimulationResult?> RunSimulation(SimulationParameters parameters)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7264/api/simulation/run", parameters);

            return await response.Content.ReadFromJsonAsync<SimulationResult>();
        }
    }
}
