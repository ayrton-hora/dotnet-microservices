using System.IO;


using Microsoft.AspNetCore.Mvc;

using Play.Common;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
	[Route("[controller]/[action]")]
	public class ItemsController : ControllerBase 
	{
		private static int _requestCounter = 0;

		private readonly IRepository<Item> _itemsRepository;
		
		public ItemsController(IRepository<Item> itemsRepository)
		{
			_itemsRepository = itemsRepository;
		}

		[HttpGet]
		public String HealthCheck() => "I'm alive";


		[HttpGet]
		public async Task<ActionResult<IEnumerable<ItemDTO>>> GetAsync()
		{
			_requestCounter++;
			Console.WriteLine($"Request {_requestCounter}: Starting...");

			if (_requestCounter <= 2) 
			{
				Console.WriteLine($"Request {_requestCounter}: Delaying...");
				await Task.Delay(TimeSpan.FromSeconds(10));
			}

			else if (_requestCounter <= 4) 
			{
				Console.WriteLine($"Request {_requestCounter}: 500 (Interal Server Error).");
				return StatusCode(500);
			}

			var items = (await _itemsRepository.GetAllAsync()).Select(i => i.AsDTO());
			Console.WriteLine($"Request {_requestCounter}: 200 (Ok).");
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

			return NoContent();
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteAsync(Guid id) 
		{
			var item = await _itemsRepository.GetAsync(id);

			if (item == null) return NotFound();
			
			await _itemsRepository.RemoveAsync(item.Id);

			return NoContent();
		}
	}
}