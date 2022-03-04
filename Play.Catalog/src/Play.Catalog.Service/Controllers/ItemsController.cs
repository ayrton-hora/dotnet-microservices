using System.IO;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

using Play.Common;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
	[Route("[controller]/[action]")]
	public class ItemsController : ControllerBase 
	{
		private readonly IRepository<Item> _itemsRepository;
		private readonly IPublishEndpoint _publishEndpoint;
		
		public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
		{
			_itemsRepository = itemsRepository;
			_publishEndpoint = publishEndpoint;
		}

		[HttpGet]
		public String HealthCheck() => "I'm alive";


		[HttpGet]
		public async Task<ActionResult<IEnumerable<ItemDTO>>> GetAsync()
		{
			var items = (await _itemsRepository.GetAllAsync()).Select(i => i.AsDTO());
			return Ok(items);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ItemDTO>> GetByIdAsync(Guid id) 
		{
			var item = await _itemsRepository.GetAsync(id);
			if (item == null) return NotFound();
			else return item.AsDTO();
		}

		[HttpPost]
		public async Task<ActionResult<ItemDTO>> PostAsync(CreatedItemDTO createdItemDTO) 
		{
			var item = new Item
			{
				Name = createdItemDTO.Name,
				Description = createdItemDTO.Description,
				Price = createdItemDTO.Price,
				CreatedDate = DateTimeOffset.Now
			};

			await _itemsRepository.CreateAsync(item);

			await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

			return CreatedAtAction(nameof(GetByIdAsync), new {id = item.Id}, item);
		} 

		[HttpPut("{id}")]
		public async Task<IActionResult> PutAsync(Guid id, UpdateItemDTO updateItemDTO)
		{
			var existingItem = await _itemsRepository.GetAsync(id);

			if (existingItem == null) return NotFound();

			existingItem.Name = updateItemDTO.Name;
			existingItem.Description = updateItemDTO.Description;
			existingItem.Price = updateItemDTO.Price;

			await _itemsRepository.UpdateAsync(existingItem);

			await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

			return NoContent();
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteAsync(Guid id) 
		{
			var item = await _itemsRepository.GetAsync(id);

			if (item == null) return NotFound();
			
			await _itemsRepository.RemoveAsync(item.Id);

			await _publishEndpoint.Publish(new CatalogItemDeleted(id));

			return NoContent();
		}
	}
}