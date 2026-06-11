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
    public class RentalPaymentMicroClient
    {

        private readonly HttpClient _httpClient;

        public RentalPaymentMicroClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get all rental orders from RentalPayment Service
        /// </summary>
        public async Task<List<RentalOrderDataDTO>> GetAllRentalOrdersAsync(DateTime? fromDate = null, DateTime? toDate = null, string? status = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (fromDate.HasValue)
                    queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
                if (toDate.HasValue)
                    queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={status}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"/api/rentals{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<RentalOrderDataDTO>>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result?.Data ?? new List<RentalOrderDataDTO>();
                }
                return new List<RentalOrderDataDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching rental orders from RentalPayment Service: {ex.Message}");
                return new List<RentalOrderDataDTO>();
            }
        }

        /// <summary>
        /// Get rental order by ID from RentalPayment Service
        /// </summary>
        public async Task<RentalOrderDataDTO?> GetRentalOrderByIdAsync(string rentalId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/rentals/{rentalId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<RentalOrderDataDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result?.Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching rental order by ID from RentalPayment Service: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all payments from RentalPayment Service
        /// </summary>
        public async Task<List<PaymentDataDTO>> GetAllPaymentsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (fromDate.HasValue)
                    queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
                if (toDate.HasValue)
                    queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"/api/payments{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<PaymentDataDTO>>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result?.Data ?? new List<PaymentDataDTO>();
                }
                return new List<PaymentDataDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching payments from RentalPayment Service: {ex.Message}");
                return new List<PaymentDataDTO>();
            }
        }

        /// <summary>
        /// Get payments by rental ID from RentalPayment Service
        /// </summary>
        public async Task<List<PaymentDataDTO>> GetPaymentsByRentalIdAsync(string rentalId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/payments/rental/{rentalId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<PaymentDataDTO>>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result?.Data ?? new List<PaymentDataDTO>();
                }
                return new List<PaymentDataDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching payments by rental ID from RentalPayment Service: {ex.Message}");
                return new List<PaymentDataDTO>();
            }
        }

        /// <summary>
        /// Get renter analytics from RentalPayment Service
        /// </summary>
        public async Task<RenterAnalyticsResponseDTO?> GetRenterAnalyticsAsync(string renterId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (fromDate.HasValue)
                    queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
                if (toDate.HasValue)
                    queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"/api/analytics/renter/{renterId}{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<RenterAnalyticsResponseDTO>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return result?.Data;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching renter analytics from RentalPayment Service: {ex.Message}");
                return null;
            }
        }

        // Helper class for API response
        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }
        }

        // Helper DTO for renter analytics
        public class RenterAnalyticsResponseDTO
        {
            public int TotalRentals { get; set; }
            public int CompletedRentals { get; set; }
            public int ActiveRentals { get; set; }
            public decimal TotalSpent { get; set; }
            public double AverageRating { get; set; }
            public int TotalRentalHours { get; set; }
        }
    }
}
