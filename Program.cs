namespace MetadataOne;

class Program
{
    static void Main(string[] args)
    {
        var metadataPath = string.Empty;
        var ordersPath = string.Empty;
        var orderItemsPath = string.Empty;

        var argsDictionary = args.Select(arg => arg.Split('='))
                                 .ToDictionary(arg => arg[0], arg => arg[1]);

        if (argsDictionary.ContainsKey("--metadata"))
        {
            metadataPath = argsDictionary["--metadata"];
        }
        else
        {
            Console.WriteLine("Metadata path is required. Use --metadata=<path>");
            return;
        }

        if (argsDictionary.ContainsKey("--orders"))
        {
            ordersPath = argsDictionary["--orders"];
        }
        else
        {
            Console.WriteLine("Orders path is required. Use --orders=<path>");
            return;
        }

        if (argsDictionary.ContainsKey("--orderItems"))
        {
            orderItemsPath = argsDictionary["--orderItems"];
        }
        else
        {
            Console.WriteLine("Order items path is required. Use --orderItems=<path>");
            return;
        }
        var processor = new CsvProcessor(argsDictionary["--metadata"]);
        var transformedOrders = processor.LoadOrders(
            argsDictionary["--orders"],
            argsDictionary["--orderItems"]);

        // Save or further process transformedOrders
        foreach (var order in transformedOrders)
        {
            Console.WriteLine($"Order {order.OrderId} - Customer: {order.CustomerName}, Email: {order.CustomerEmail}");
            foreach (var item in order.OrderItems)
            {
                Console.WriteLine($"  Item {item.OrderItemId}: {item.ProductName} - ${item.Price}");
            }
        }

    }
}
