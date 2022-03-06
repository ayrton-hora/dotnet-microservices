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
        private readonly IRepository<InventoryItem> _inventoryItemsRepository;
        private readonly IRepository<CatalogItem> _catalogItemsRespository;

        public ItemsController(IRepository<InventoryItem> inventoryItemRepo, IRepository<CatalogItem> catalogItemRepo)
        {
            _inventoryItemsRepository = inventoryItemRepo;
            _catalogItemsRespository = catalogItemRepo;
        }

        [HttpGet]
        public String HealthCheck() => "I'm alive";
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDTO>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();
            
            var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(i => i.UserId == userId);

            var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);

            var catalogItemsEntities = await _catalogItemsRespository.GetAllAsync(i => itemIds.Contains(i.Id));

            var inventoryItemDTOs = inventoryItemEntities.Select(inventoryItem => 
            {
                var catalogItem = catalogItemsEntities.Single(item => item.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDTO(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDTOs);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDTO grantItemsDTO)
        {
            var inventoryItem = await _inventoryItemsRepository.GetAsync(
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

                await _inventoryItemsRepository.CreateAsync(inventoryItem);
            }

            else
            {
                inventoryItem.Quantity += grantItemsDTO.Quantity;
                
                await _inventoryItemsRepository.UpdateAsync(inventoryItem); 
            }

            return Ok();
        }
    }
}