namespace InformationSystem.Domain.Models;
public class RawMaterialWarehouse
{
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; }
    public int Quantity { get; set; }
    public decimal PurchaseCost { get; set; }
    public DateTime LastUpdated { get; set; }
    public Guid ExpenseId { get; set; }

    public Expense Expense { get; set; }
}