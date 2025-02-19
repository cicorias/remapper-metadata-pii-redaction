using System.Text.Json;

using CsvHelper;
using CsvHelper.Configuration;


namespace MetadataOne;
public class MetadataConfig
{
    public Dictionary<string, Dictionary<string, string>>? Mappings { get; set; }
    public Dictionary<string, List<string>>? PiiFields { get; set; }
    public Dictionary<string, string>? RedactionRules { get; set; }
}

public class CsvProcessor
{
    private readonly MetadataConfig _config;

    public CsvProcessor(string metadataPath)
    {
        if (string.IsNullOrWhiteSpace(metadataPath) || !File.Exists(metadataPath))
        {
            throw new ArgumentException("Invalid metadata path.");
        }

        string metadataJson = File.ReadAllText(metadataPath);

        if (string.IsNullOrEmpty(metadataJson))
        {
            throw new ArgumentException("Metadata JSON is empty.");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Match JSON's camelCase naming
            // PropertyNameCaseInsensitive = true
        };

        var config = JsonSerializer.Deserialize<MetadataConfig>(metadataJson, options);
        _config = config ?? throw new ArgumentException("Invalid metadata JSON.");
    }

    public List<Orders> LoadOrders(string ordersFilePath, string orderItemsFilePath)
    {
        var orders = ReadCsv<Orders>(ordersFilePath);
        var orderItems = ReadCsv<OrderItems>(orderItemsFilePath);

        // Assign OrderItems to Orders
        var orderDict = orders.ToDictionary(o => o.OrderId);
        foreach (var item in orderItems)
        {
            if (orderDict.TryGetValue(item.OrderId, out var order))
            {
                order.OrderItems.Add(item);
            }
        }

        return TransformData(orders);
    }

    private List<T> ReadCsv<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture));
        return csv.GetRecords<T>().ToList();
    }

    private List<Orders> TransformData(List<Orders> orders)
    {
        foreach (var order in orders)
        {
            ApplyTransformations(order);
            foreach (var item in order.OrderItems)
            {
                ApplyTransformations(item);
            }
        }
        return orders;
    }

    private void ApplyTransformations<T>(T record)
    {
        var typeName = typeof(T).Name;
        if (_config.PiiFields != null && _config.PiiFields.TryGetValue(typeName, out var piiFields))
        {
            foreach (var field in piiFields)
            {
                var prop = typeof(T).GetProperty(field);
                if (prop != null && prop.GetValue(record) is string value)
                {
                    prop.SetValue(record, ApplyRedaction(field, value));
                }
            }
        }
    }

    private string ApplyRedaction(string fieldName, string value)
    {
        if (_config.RedactionRules != null && _config.RedactionRules.TryGetValue(fieldName, out var rule))
        {
            return rule switch
            {
                "MASK" => "*****",
                _ => value
            };
        }
        return value;
    }
}
