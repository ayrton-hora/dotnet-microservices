using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDTO AsDTO(this Item item)
        {
            return new ItemDTO(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}