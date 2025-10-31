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

        public async Task<string> GetUserProfileAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/user/profile");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<UserProfileDto?> GetUserInfoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/user/info");
                response.EnsureSuccessStatusCode();

                // Using System.Net.Http.Json for deserialization
                UserProfileDto? jsonContent = await response.Content.ReadFromJsonAsync<UserProfileDto>();
                return jsonContent;


                //  Using System.Text.Json for deserialization
                //var jsonContent = await response.Content.ReadAsStringAsync();
                //var options = new JsonSerializerOptions
                //{
                //    PropertyNameCaseInsensitive = true
                //};
                //return JsonSerializer.Deserialize<UserProfileDto>(jsonContent, options);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
