using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
  public record ItemDTO (Guid Id, 
  String Name, 
  String Description, 
  Decimal Price, 
  DateTimeOffset CreatedDate);

  public record CreatedItemDTO ([Required] String Name, 
    String Description, 
    [Range(0, 1000)] decimal Price);

  public record UpdateItemDTO ([Required] String Name, 
  String Description, 
  [Range(0, 1000)] decimal Price);
}