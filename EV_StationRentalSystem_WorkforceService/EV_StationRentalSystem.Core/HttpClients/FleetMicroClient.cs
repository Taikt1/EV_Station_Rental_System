using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EV_StationRentalSystem.Core.DTO;

namespace EV_StationRentalSystem.Core.HttpClients
{
    public class FleetMicroClient
    {

        private readonly HttpClient _httpClient;

        public FleetMicroClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get all vehicles from Fleet Service
        /// </summary>
        public async Task<List<VehicleDataDTO>> GetAllVehiclesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Vehicle");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<VehicleDataDTO>>();
                    return result ?? new List<VehicleDataDTO>();
                }
                return new List<VehicleDataDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vehicles from Fleet Service: {ex.Message}");
                return new List<VehicleDataDTO>();
            }
        }

        /// <summary>
        /// Get vehicles by status from Fleet Service
        /// </summary>
        public async Task<List<VehicleDataDTO>> GetVehiclesByStatusAsync(string status)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Vehicle/status/{status}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<VehicleDataDTO>>();
                    return result ?? new List<VehicleDataDTO>();
                }
                return new List<VehicleDataDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vehicles by status from Fleet Service: {ex.Message}");
                return new List<VehicleDataDTO>();
            }
        }

        /// <summary>
        /// Get vehicle by ID from Fleet Service
        /// </summary>
        public async Task<VehicleDataDTO?> GetVehicleByIdAsync(string vehicleId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Vehicle/{vehicleId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<VehicleDataDTO>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vehicle by ID from Fleet Service: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all vehicle types from Fleet Service
        /// </summary>
        public async Task<List<VehicleTypeDataDTO>> GetAllVehicleTypesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/TypeVehicle");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<VehicleTypeDataDTO>>();
                    return result ?? new List<VehicleTypeDataDTO>();
                }
                return new List<VehicleTypeDataDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vehicle types from Fleet Service: {ex.Message}");
                return new List<VehicleTypeDataDTO>();
            }
        }

        /// <summary>
        /// Get vehicle status summary from Fleet Service
        /// </summary>
        public async Task<Dictionary<string, int>> GetVehicleStatusSummaryAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Vehicle/summary");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
                    return result ?? new Dictionary<string, int>();
                }
                return new Dictionary<string, int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vehicle status summary from Fleet Service: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }
    }
}
