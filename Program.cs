using CommandLine;

namespace MetadataOne;

class Program
{
    public class Options
    {
        [Option("metadata", Required = true, HelpText = "Path to the metadata JSON file.")]
        public string? Metadata { get; set; }

        [Option("orders", Required = true, HelpText = "Path to the orders CSV file.")]
        public string? Orders { get; set; }

        [Option("orderItems", Required = true, HelpText = "Path to the order items CSV file.")]
        public string? OrderItems { get; set; }
    }

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }

    static void RunOptions(Options opts)
    {
        var processor = new CsvProcessor(opts.Metadata!);
        var transformedOrders = processor.LoadOrders(opts.Orders!, opts.OrderItems!);

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

    static void HandleParseError(IEnumerable<Error> errs)
    {
        // Handle errors
        foreach (var err in errs)
        {
            Console.WriteLine(err.ToString());
        }
    }
}