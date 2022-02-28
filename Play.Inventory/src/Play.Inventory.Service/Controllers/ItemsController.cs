using Microsoft.AspNetCore.Mvc;

using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _itemsRepository;
        private readonly CatalogClient _catalogClient;

        public ItemsController(IRepository<InventoryItem> itemRepo, CatalogClient ctlgClient)
        {
            _itemsRepository = itemRepo;
            _catalogClient = ctlgClient;
        }

        [HttpGet]
        public String HealthCheck() => "I'm alive";
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDTO>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            var catalogItems = await _catalogClient.GetCatalogItemsAsync();
            
            var inventoryItemEntities = await _itemsRepository.GetAllAsync(i => i.UserId == userId);

            var inventoryItemDTOs = inventoryItemEntities.Select(inventoryItem => 
            {
                var catalogItem = catalogItems.Single(item => item.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDTO(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDTOs);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDTO grantItemsDTO)
        {
            var inventoryItem = await _itemsRepository.GetAsync(
               i => i.UserId == grantItemsDTO.UserId && i.CatalogItemId == grantItemsDTO.CatalogItemId);

            if (inventoryItem == null) 
            {
                inventoryItem = new InventoryItem
                {
                    UserId = grantItemsDTO.UserId,
                    CatalogItemId = grantItemsDTO.CatalogItemId,
                    Quantity = grantItemsDTO.Quantity,
                    AcquireDate = DateTimeOffset.UtcNow
                };

                await _itemsRepository.CreateAsync(inventoryItem);
            }

            else
            {
                inventoryItem.Quantity += grantItemsDTO.Quantity;
                
                await _itemsRepository.UpdateAsync(inventoryItem); 
            }

            return Ok();
        }
    }
}