using Microsoft.AspNetCore.Mvc;

using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
	[ApiController]
	[Route("items")]
	public class ItemsController : ControllerBase 
	{
		// private static readonly List<ItemDTO> items = new() 
		// {
		// 	new ItemDTO(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
		// 	new ItemDTO(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
		// 	new ItemDTO(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
		// };

		private readonly ItemsRepository itemsRepository = new();

		// [HttpGet]
		// public String HealthCheck()  
		// {
		//   return "I'm alive";
		// }

		[HttpGet]
		public async Task<IEnumerable<ItemDTO>> GetAsync() 
		{
			var items = (await itemsRepository.GetAllAsync()).Select(i => i.AsDTO());
			return items;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ItemDTO>> GetByIdAsync(Guid id) 
		{
			var item = await itemsRepository.GetAsync(id);
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

			await itemsRepository.CreateAsync(item);

			return CreatedAtAction(nameof(GetByIdAsync), new {id = item.Id}, item);
		} 

		[HttpPut("{id}")]
		public async Task<IActionResult> PutAsync(Guid id, UpdateItemDTO updateItemDTO)
		{
			var existingItem = await itemsRepository.GetAsync(id);

			if (existingItem == null) return NotFound();

			existingItem.Name = updateItemDTO.Name;
			existingItem.Description = updateItemDTO.Description;
			existingItem.Price = updateItemDTO.Price;

			await itemsRepository.UpdateAsync(existingItem);

			return NoContent();
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteAsync(Guid id) 
		{
			var item = await itemsRepository.GetAsync(id);

			if (item == null) return NotFound();
			
			await itemsRepository.RemoveAsync(item.Id);

			return NoContent();
		}
	}
}