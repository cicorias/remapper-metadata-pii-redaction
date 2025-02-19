namespace MetadataOne;
public class Order
{
    public int OrderId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
}
