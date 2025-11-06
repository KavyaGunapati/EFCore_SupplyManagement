using System.Collections.Generic;

namespace SupplyManagement.Models;
public class Warehouse
{
    public int WarehouseId { get; set; }
    public string Name { get; set; } = null!;
    public string Location { get; set; }=null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();

}