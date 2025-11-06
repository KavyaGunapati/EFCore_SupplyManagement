using System.Collections.Generic;

namespace SupplyManagement.Models;
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public int WarehouseId {  get; set; }
    public Warehouse Warehouse { get; set; }=null!;
    public int SupplierId {  get; set; }
    public Supplier Supplier { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}