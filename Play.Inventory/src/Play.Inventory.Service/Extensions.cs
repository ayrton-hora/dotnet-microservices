using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
{
    public static class Extensions 
    {
        public static InventoryItemDTO AsDTO(this InventoryItem item, string name, string description)
        {
            return new InventoryItemDTO(item.CatalogItemId, name, description, item.Quantity, item.AcquireDate);
        }
    }
}