using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.HttpClients
{
    public class RentalPaymentMicroClient
    {

        private readonly HttpClient _httpClient;

        public RentalPaymentMicroClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        
    }
}
