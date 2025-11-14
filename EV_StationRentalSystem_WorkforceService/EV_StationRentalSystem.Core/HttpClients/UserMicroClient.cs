using EV_StationRentalSystem.Core.DTO;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserMicroClient>? _logger;

        public UserMicroClient(HttpClient httpClient, ILogger<UserMicroClient>? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Lấy thông tin profile của một user theo userId
        /// Hỗ trợ cả JWT token và Gateway headers
        /// </summary>
        public async Task<UserProfileResponse?> GetUserProfileAsync(
            string userId,
            string? authToken = null,
            Dictionary<string, string>? gatewayHeaders = null)
        {
            try
            {
                // Clear existing headers
                _httpClient.DefaultRequestHeaders.Clear();

                // Thêm JWT token nếu có
                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                // Thêm Gateway headers nếu có (để forward từ Gateway)
                if (gatewayHeaders != null)
                {
                    foreach (var header in gatewayHeaders)
                    {
                        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    _logger?.LogInformation("Forwarding Gateway headers to UserService");
                }

                _logger?.LogInformation("Calling UserService for userId: {UserId}", userId);

                var response = await _httpClient.GetAsync($"/api/User/profile/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("UserService returned {StatusCode} for userId: {UserId}",
                        response.StatusCode, userId);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserProfileResponse>>();

                _logger?.LogInformation("Successfully retrieved user profile for userId: {UserId}", userId);
                return result?.Data;
            }
            catch (TaskCanceledException ex)
            {
                _logger?.LogError(ex, "Timeout calling UserService for userId: {UserId}", userId);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "HTTP error calling UserService for userId: {UserId}", userId);
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error calling UserService for userId: {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Lấy thông tin nhiều users theo danh sách userId
        /// </summary>
        public async Task<Dictionary<string, UserProfileResponse>> GetMultipleUserProfilesAsync(
            List<string> userIds,
            string? authToken = null,
            Dictionary<string, string>? gatewayHeaders = null)
        {
            var result = new Dictionary<string, UserProfileResponse>();

            _logger?.LogInformation("Fetching {Count} user profiles", userIds.Count);

            // Xử lý song song để tăng tốc độ
            var tasks = userIds.Select(async userId =>
            {
                var profile = await GetUserProfileAsync(userId, authToken, gatewayHeaders);
                return new { UserId = userId, Profile = profile };
            });

            var results = await Task.WhenAll(tasks);

            foreach (var item in results)
            {
                if (item.Profile != null)
                {
                    result[item.UserId] = item.Profile;
                }
            }

            _logger?.LogInformation("Successfully fetched {Count}/{Total} user profiles",
                result.Count, userIds.Count);

            return result;
        }

        /// <summary>
        /// Cập nhật thông tin user (chỉ manager)
        /// </summary>
        public async Task<UserProfileResponse?> AdminUpdateUserAsync(
            string userId,
            AdminUpdateUserRequest request,
            string? authToken = null,
            Dictionary<string, string>? gatewayHeaders = null)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                if (gatewayHeaders != null)
                {
                    foreach (var header in gatewayHeaders)
                    {
                        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                _logger?.LogInformation("Admin updating user: {UserId}", userId);

                var response = await _httpClient.PutAsJsonAsync($"/api/User/admin/{userId}", request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("Failed to update user {UserId}: {StatusCode}", userId, response.StatusCode);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserProfileResponse>>();
                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating user {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Thay đổi role của user (chỉ manager)
        /// </summary>
        public async Task<bool> ChangeUserRoleAsync(
            string userId,
            string newRole,
            string? authToken = null,
            Dictionary<string, string>? gatewayHeaders = null)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                if (gatewayHeaders != null)
                {
                    foreach (var header in gatewayHeaders)
                    {
                        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                _logger?.LogInformation("Changing role for user {UserId} to {NewRole}", userId, newRole);

                var response = await _httpClient.PutAsJsonAsync($"/api/User/admin/{userId}/role", new { role = newRole });

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error changing role for user {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Khóa/mở khóa user (chỉ manager)
        /// </summary>
        public async Task<bool> LockUserAsync(
            string userId,
            bool isLocked,
            string? reason = null,
            string? authToken = null,
            Dictionary<string, string>? gatewayHeaders = null)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                if (gatewayHeaders != null)
                {
                    foreach (var header in gatewayHeaders)
                    {
                        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                var action = isLocked ? "lock" : "unlock";
                _logger?.LogInformation("{Action} user {UserId}", action, userId);

                var response = await _httpClient.PutAsJsonAsync($"/api/User/admin/{userId}/{action}", new { reason });

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error locking/unlocking user {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Xóa user (chỉ manager)
        /// </summary>
        public async Task<bool> DeleteUserAsync(
            string userId,
            string? reason = null,
            string? authToken = null,
            Dictionary<string, string>? gatewayHeaders = null)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(authToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }

                if (gatewayHeaders != null)
                {
                    foreach (var header in gatewayHeaders)
                    {
                        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                _logger?.LogInformation("Deleting user {UserId}", userId);

                var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/User/admin/{userId}")
                {
                    Content = JsonContent.Create(new { reason })
                };

                var response = await _httpClient.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting user {UserId}", userId);
                return false;
            }
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
