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

      
    }
}
