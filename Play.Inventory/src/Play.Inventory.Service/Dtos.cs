using System;

namespace Play.Inventory.Service.Dtos
{
    public record GrantItemsDTO(
        Guid UserId,
        Guid CatalogItemId,
        int Quantity
    );

    public record InventoryItemDTO(
        Guid CatalogItemId,
        string Name,
        string Description,
        int Quantity,
        DateTimeOffset AcquiredDate
    );

    public record CatalogItemDTO(
        Guid Id, 
        String Name, 
        String Description
    );
}