using EV_StationRentalSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.HttpClients
{
    public class UserMicroClient
    {
        private readonly HttpClient _httpClient;

        public UserMicroClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Lấy thông tin profile của một user theo userId
        /// </summary>
        public async Task<UserProfileResponse?> GetUserProfileAsync(string userId, string? authToken = null)
        {
            try
            {
                // Thêm token vào header nếu có
                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                var response = await _httpClient.GetAsync($"/api/User/profile/{userId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserProfileResponse>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling UserService: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy thông tin nhiều users theo danh sách userId
        /// </summary>
        public async Task<Dictionary<string, UserProfileResponse>> GetMultipleUserProfilesAsync(
            List<string> userIds, string? authToken = null)
        {
            var result = new Dictionary<string, UserProfileResponse>();

            foreach (var userId in userIds)
            {
                var profile = await GetUserProfileAsync(userId, authToken);
                if (profile != null)
                {
                    result[userId] = profile;
                }
            }

            return result;
        }

        // Helper class để parse response từ UserService
        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public T? Data { get; set; }
        }
    }
}
