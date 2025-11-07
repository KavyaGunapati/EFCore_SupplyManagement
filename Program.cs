using SupplyManagement.Models;
using SupplyManagement.Data;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.IO;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        SupplyDbContext context = new SupplyDbContext();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Supply Management System ===");
            Console.WriteLine("1. Add Warehouse");
            Console.WriteLine("2. Add Supplier");
            Console.WriteLine("3. Add Product");
            Console.WriteLine("4. Record Inventory Transaction");
            Console.WriteLine("5. View Products by Warehouse");
            Console.WriteLine("6. View Transactions by Product");
            Console.WriteLine("7. Reports");
            Console.WriteLine("8. Filter Transactions");
            Console.WriteLine("9. Group Transactions");
            Console.WriteLine("10. Join Product & Supplier Info");
            Console.WriteLine("11. Dynamic Product Filter");
            Console.WriteLine("12. Update Product");
            Console.WriteLine("13. Delete Product");
            Console.WriteLine("14. Order Product");
            Console.WriteLine("15. Export Products to CSV");
            Console.WriteLine("16. Export Transactions to CSV");
            Console.WriteLine("17. Export All Data to csv");
            Console.WriteLine("18. Exit");
            Console.Write("\nSelect an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddWarehouse(context); break;
                case "2":
                    await AddSupplier(context); break;
                case "3":
                    await AddProduct(context); break;
                case "4":
                    await RecordTransaction(context); break;
                case "5":
                    ViewProductsByWarehouse(context); break;
                case "6":
                    ViewTransactionsByProduct(context); break;
                case "7":
                    ShowReports(context); break;
                case "8":
                    FilterTransactions(context); break;
                case "9":
                    GroupTransactions(context); break;
                case "10":
                    JoinProductSupplierInfo(context); break;
                case "11":
                    DynamicProductFilter(context); break;
                case "12":
                    await UpdateProduct(context); break;
                case "13":
                    await DeleteProduct(context); break;
                case "14":
                    await OrderProduct(context); break;
                case "15":
                    await ExportProductsToCSV(context); break;
                case "16":
                    await ExportTransactionsToCSV(context); break;
                case "17":
                    await ExportAllDataAsync(context); break;
                case "18": return;
                default:
                    Console.WriteLine("Invalid choice. Press Enter to try again.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue...");
        Console.ReadLine();
    }

    static async Task AddWarehouse(SupplyDbContext context)
    {
        Console.Write("Enter Warehouse Name: ");
        string? name = Console.ReadLine();
        Console.Write("Enter Location: ");
        string? location = Console.ReadLine();

        var warehouse = new Warehouse { Name = name!, Location = location! };
        await context.Warehouses.AddAsync(warehouse);
        await context.SaveChangesAsync();

        Console.WriteLine("\n Warehouse added successfully.");
        Pause();
    }

    static async Task AddSupplier(SupplyDbContext context)
    {
        Console.Write("Enter Supplier Name: ");
        string? name = Console.ReadLine();
        Console.Write("Enter Contact Info: ");
        string? contact = Console.ReadLine();

        var supplier = new Supplier { Name = name!, Contact = contact! };
        await context.Suppliers.AddAsync(supplier);
        await context.SaveChangesAsync();

        Console.WriteLine("\n Supplier added successfully.");
        Pause();
    }

    static async Task AddProduct(SupplyDbContext context)
    {
        Console.Write("Enter Product Name: ");
        string? name = Console.ReadLine();
        Console.Write("Enter Price: ");
        decimal price = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Enter Stock Quantity: ");
        int stock = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Warehouse ID: ");
        int warehouseId = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Supplier ID: ");
        int supplierId = Convert.ToInt32(Console.ReadLine());

        var product = new Product
        {
            Name = name!,
            Price = price,
            StockQuantity = stock,
            WarehouseId = warehouseId,
            SupplierId = supplierId
        };

        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        Console.WriteLine("\n Product added successfully.");
        Pause();
    }

    static async Task RecordTransaction(SupplyDbContext context)
    {
        Console.Write("Enter Product ID: ");
        int productId = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Quantity: ");
        int quantity = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Transaction Type (IN/OUT): ");
        string? type = Console.ReadLine().ToUpper().Trim();
        Console.Write("Enter Date (yyyy-MM-dd): ");
        DateTime date = DateTime.Parse(Console.ReadLine() ?? "");

        var product = await context.Products.FindAsync(productId);

        if (product == null)
        {
            Console.WriteLine(" Product not found.");
            Pause();
            return;
        }

        if (type == "OUT" && product.StockQuantity < quantity)
        {
            Console.WriteLine(" Not enough stock.");
            Pause();
            return;
        }

        var transaction = new Transaction
        {
            ProductId = productId,
            Quantity = quantity,
            TransactionType = type!,
            Date = date
        };

        await context.Transactions.AddAsync(transaction);
        product.StockQuantity += (type == "IN" ? quantity : -quantity);
        await context.SaveChangesAsync();

        Console.WriteLine("\n Transaction recorded and stock updated.");
        Pause();
    }

    static void ViewProductsByWarehouse(SupplyDbContext context)
    {
        Console.Write("Enter Warehouse ID: ");
        int warehouseId = Convert.ToInt32(Console.ReadLine());

        var products = context.Products
            .Where(p => p.WarehouseId == warehouseId)
            .ToList();
        if (products.Count <= 0)
        {
            Console.WriteLine("There are nor transactions recorded for this product");
            Pause();
            return;
        }
        Console.WriteLine($"\nProducts in Warehouse {warehouseId}:");
        foreach (var product in products)
        {
            Console.WriteLine($"- {product.Name} | Price: {product.Price} | Stock: {product.StockQuantity}");
        }

        Pause();
    }

    static void ViewTransactionsByProduct(SupplyDbContext context)
    {
        Console.Write("Enter Product ID: ");
        int productId = Convert.ToInt32(Console.ReadLine());

        var transactions = context.Transactions
            .Where(t => t.ProductId == productId)
            .ToList();
        if (transactions.Count <= 0)
        {
            Console.WriteLine("There are nor transactions recorded for this product");
            Pause();
            return;
        }
        Console.WriteLine($"\nTransactions for Product {productId}:");
        foreach (var t in transactions)
        {
            Console.WriteLine($"{t.TransactionType} | Qty: {t.Quantity} | Date: {t.Date.ToShortDateString()}");
        }

        Pause();
    }

    static async Task UpdateProduct(SupplyDbContext context)
    {
        Console.Write("Enter Product ID to Update: ");
        int productId = Convert.ToInt32(Console.ReadLine());

        var product = await context.Products.FindAsync(productId);
        if (product == null)
        {
            Console.WriteLine(" Product not found.");
            Pause();
            return;
        }

        Console.Write("Enter New Name (leave blank to keep current): ");
        string? name = Console.ReadLine();
        Console.Write("Enter New Price (leave blank to keep current): ");
        string? priceInput = Console.ReadLine();
        Console.Write("Enter New Stock Quantity (leave blank to keep current): ");
        string? stockInput = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(name)) product.Name = name;
        if (decimal.TryParse(priceInput, out decimal newPrice)) product.Price = newPrice;
        if (int.TryParse(stockInput, out int newStock)) product.StockQuantity = newStock;

        await context.SaveChangesAsync();
        Console.WriteLine("\n Product updated successfully.");
        Pause();
    }

    static async Task DeleteProduct(SupplyDbContext context)
    {
        Console.Write("Enter Product ID to Delete: ");
        int productId = Convert.ToInt32(Console.ReadLine());

        var product = await context.Products.FindAsync(productId);
        if (product == null)
        {
            Console.WriteLine(" Product not found.");
            Pause();
            return;
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        Console.WriteLine("\n Product deleted successfully.");
        Pause();
    }

    static void ShowReports(SupplyDbContext context)
    {
        Console.WriteLine("1. Total Stock per Warehouse");
        Console.WriteLine("2. Top Supplied Products");
        Console.Write("Choose report: ");
        string? choice = Console.ReadLine();

        if (choice == "1")
        {
            var report = context.Products
                .GroupBy(p => p.WarehouseId)
                .Select(g => new
                {
                    WarehouseId = g.Key,
                    TotalStock = g.Sum(p => p.StockQuantity)
                });

            Console.WriteLine("\n Total Stock per Warehouse:");
            foreach (var item in report)
            {
                Console.WriteLine($"Warehouse {item.WarehouseId}: {item.TotalStock} units");
            }
        }
        else if (choice == "2")
        {
            var report = context.Products
                .GroupBy(p => p.SupplierId)
                .Select(g => new
                {
                    SupplierId = g.Key,
                    ProductCount = g.Count()
                })
                .OrderByDescending(r => r.ProductCount);

            Console.WriteLine("\n Top Supplied Products:");
            foreach (var item in report)
            {
                Console.WriteLine($"Supplier {item.SupplierId}: {item.ProductCount} products");
            }
        }

        Pause();
    }

    static void FilterTransactions(SupplyDbContext context)
    {
        Console.Write("Enter Transaction Type (IN/OUT or leave blank): ");
        string? type = Console.ReadLine();
        Console.Write("Enter Minimum Quantity (or leave blank): ");
        string? qtyInput = Console.ReadLine();

        var query = context.Transactions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(t => t.TransactionType == type);

        if (int.TryParse(qtyInput, out int minQty))
            query = query.Where(t => t.Quantity >= minQty);

        var results = query.ToList();
        if (results.Count <= 0)
        {
            Console.WriteLine("No transactions under this");
            Pause();
            return;

        }
        Console.WriteLine("\nFiltered Transactions:");
        foreach (var t in results)
        {
            Console.WriteLine($"{t.TransactionType} | Qty: {t.Quantity} | Date: {t.Date.ToShortDateString()}");
        }

        Pause();
    }

    static void GroupTransactions(SupplyDbContext context)
    {
        Console.WriteLine("Group Transactions by:");
        Console.WriteLine("1. Product");
        Console.WriteLine("2. Date (Month-Year)");
        Console.Write("Choose option: ");
        string? choice = Console.ReadLine();

        if (choice == "1")
        {
            var grouped = context.Transactions
                .GroupBy(t => t.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(t => t.Quantity),
                    Count = g.Count()
                });

            Console.WriteLine("\nTransactions Grouped by Product:");
            foreach (var item in grouped)
            {
                Console.WriteLine($"Product {item.ProductId}: {item.TotalQuantity} units in {item.Count} transactions");
            }
        }
        else if (choice == "2")
        {
            var grouped = context.Transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Month}-{g.Key.Year}",
                    TotalQuantity = g.Sum(t => t.Quantity),
                    Count = g.Count()
                });

            Console.WriteLine("\nTransactions Grouped by Month-Year:");
            foreach (var item in grouped)
            {
                Console.WriteLine($"{item.Period}: {item.TotalQuantity} units in {item.Count} transactions");
            }
        }

        Pause();
    }

    static void JoinProductSupplierInfo(SupplyDbContext context)
    {
        var joined = context.Products
            .Join(context.Suppliers,
                  p => p.SupplierId,
                  s => s.SupplierId,
                  (p, s) => new
                  {
                      p.Name,
                      p.Price,
                      SupplierName = s.Name,
                      s.Contact
                  });

        Console.WriteLine("\nProduct & Supplier Info:");
        foreach (var item in joined)
        {
            Console.WriteLine($"Product: {item.Name} | Price: {item.Price} | Supplier: {item.SupplierName} | Contact: {item.Contact}");
        }

        Pause();
    }

    static void DynamicProductFilter(SupplyDbContext context)
    {
        Console.Write("Enter minimum price (or leave blank): ");
        string? minPriceInput = Console.ReadLine();
        Console.Write("Enter maximum stock (or leave blank): ");
        string? maxStockInput = Console.ReadLine();

        var query = context.Products.AsQueryable();

        if (decimal.TryParse(minPriceInput, out decimal minPrice))
            query = query.Where(p => p.Price >= minPrice);

        if (int.TryParse(maxStockInput, out int maxStock))
            query = query.Where(p => p.StockQuantity <= maxStock);

        var results = query.ToList();

        Console.WriteLine("\nFiltered Products:");
        foreach (var p in results)
        {
            Console.WriteLine($"Product: {p.Name} | Price: {p.Price} | Stock: {p.StockQuantity}");
        }

        Pause();
    }

    static async Task OrderProduct(SupplyDbContext context)
    {
        Console.Write("Enter Product ID to Order: ");
        int productId = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter Quantity: ");
        int quantity = Convert.ToInt32(Console.ReadLine());

        var product = await context.Products.FindAsync(productId);
        if (product == null)
        {
            Console.WriteLine(" Product not found.");
            Pause();
            return;
        }

        if (product.StockQuantity < quantity)
        {
            Console.WriteLine(" Not enough stock to fulfill the order.");
            Pause();
            return;
        }

        var transaction = new Transaction
        {
            ProductId = productId,
            Quantity = quantity,
            TransactionType = "OUT",
            Date = DateTime.Now
        };

        product.StockQuantity -= quantity;
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        Console.WriteLine("\n Order placed and transaction recorded.");
        Pause();
    }

    static async Task ExportAllDataAsync(SupplyDbContext context)
    {
        var exportProductsTask = Task.Run(() => ExportProductsToCSV(context));
        var exportTransactionsTask = Task.Run(() => ExportTransactionsToCSV(context));

        await Task.WhenAll(exportProductsTask, exportTransactionsTask);

        Console.WriteLine("\nAll data exported successfully.");
        Pause();
    }
    static async Task ExportTransactionsToCSV(SupplyDbContext context)
    {
        string filePath = "transactions.csv";
        var transactions = context.Transactions.ToList();
        var csv = new StringBuilder();

        csv.AppendLine("Transaction ID,Product ID,Quantity,Type,Date");
        foreach (var t in transactions)
        {
            csv.AppendLine($"{t.TransactionId},{t.ProductId},{t.Quantity},{t.TransactionType},{t.Date:yyyy-MM-dd}");
        }
        File.WriteAllText(filePath, csv.ToString());
  Console.WriteLine($"\nTransaction data exported to {filePath}");
        Pause();

    }
    static async Task ExportProductsToCSV(SupplyDbContext context)
    {
        string filePath = "products.csv";
        var products = context.Products.ToList();

        var csv = new StringBuilder();
        csv.AppendLine("Product ID,Name,Price,Stock Quantity,Warehouse ID,Supplier ID");

        foreach (var p in products)
        {
            csv.AppendLine($"{p.Id},{p.Name},{p.Price},{p.StockQuantity},{p.WarehouseId},{p.SupplierId}");
        }

        await File.WriteAllTextAsync(filePath, csv.ToString());
        Console.WriteLine($"\nProduct data exported to {filePath}");
        Pause();
    }
}

