﻿using Newtonsoft.Json;
using RSAllies.Client.Contracts;
using RSAllies.Client.HelperTypes;

namespace RSAllies.Client
{
    public class ApiClient(HttpClient httpClient)
    {
        public async Task<Result<List<VenueDto>?>?> GetVenuesAsync()
        {
            var response = await httpClient.GetAsync("/api/venues");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Result<List<VenueDto>?>? result = JsonConvert.DeserializeObject<Result<List<VenueDto>>>(content)!;
                return result;
            }
            else
            {
                // Handle error
                return null;
            }
        }

        public async Task<Result<List<SessionDto>?>?> GetVenueSessionAsync(Guid Id)
        {
            var response = await httpClient.GetAsync($"/api/venue/{Id}/sessions");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Result<List<SessionDto>?>? result = JsonConvert.DeserializeObject<Result<List<SessionDto>>>(content)!;
                return result;
            }
            else
            {
                // Handle error
                return null;
            }
        }

    }
}
