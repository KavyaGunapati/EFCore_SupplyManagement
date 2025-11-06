using System.Collections.Generic;

namespace SupplyManagement.Models;
public class Supplier
{
    public int SupplierId { get; set; }
    public string Name { get; set; } = null!;
    public string Contact { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}