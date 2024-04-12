using Microsoft.FluentUI.AspNetCore.Components;
using Newtonsoft.Json;
using RSAllies.Client.Contracts;
using RSAllies.Client.HelperTypes;
using System.Net.Http.Json;

namespace RSAllies.Client
{
    public class ApiClient(HttpClient httpClient, IDialogService dialogService)
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

        public async Task<Result<Guid>?> CreateVenueAsync(CreateVenueDto venue)
        {
            var response = await httpClient.PostAsJsonAsync<CreateVenueDto>("/api/venues", venue);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Result<Guid>>(content)!;
                return result;
            }
            else
            {
                // Handle error
                dialogService.ShowError("Failed to create venue", "Api_Error");
                return null;
            }
        }

    }
}
