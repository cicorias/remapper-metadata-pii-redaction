namespace MetadataOne;
public class Orders
{
    public int OrderId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public List<OrderItems> OrderItems { get; set; } = new();
}

public class OrderItems
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
}
