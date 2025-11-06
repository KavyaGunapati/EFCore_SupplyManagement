using System;

namespace SupplyManagement.Models;
public class Transaction
{
    public int TransactionId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public string TransactionType { get; set; }=null!;
    public DateTime Date {  get; set; }
}