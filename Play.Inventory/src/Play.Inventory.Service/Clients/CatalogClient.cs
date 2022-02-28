using System.Collections.ObjectModel;

using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient _httpClient;

        public CatalogClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<IReadOnlyCollection<CatalogItemDTO>> GetCatalogItemsAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDTO>>("/Items/GetAsync");
            if (items == null) return new ReadOnlyCollection<CatalogItemDTO>(new List<CatalogItemDTO>());
            return items;
        }
    }
}