using OrderManageMent.Interfaces;

class OrderService : IOrderService
{
    private readonly List<Orders> orders = new();
    private readonly ProductService productService = new();
    private int nextOrderId = 1;

    public Customer AddCustomer()
    {
        Console.WriteLine("Enter Customer ID");
        int customerId = int.TryParse(Console.ReadLine(), out int parsedId) ? parsedId : 0;
        Console.WriteLine("Enter Customer Name");
        string customerName = Console.ReadLine() ?? string.Empty;
        return new Customer
        {
            CustomerId = customerId,
            CustomerName = customerName
        };
    }

    public Orders? CreateOrder(Customer customer)
    {
        var items = new List<OrderItem>();

        while (true)
        {
            List<Product> available = productService.GetAvailableProducts();
            if (available.Count == 0)
            {
                Console.WriteLine("No products are in stock.");
                break;
            }

            Console.WriteLine("Available products:");
            foreach (Product p in available)
            {
                Console.WriteLine($"{p.ProductId}. {p.Name} - Price: {p.Price} - Stock: {p.StockQuantity}");
            }

            Console.WriteLine("Enter Product ID");
            int productId = int.TryParse(Console.ReadLine(), out int parsedProduct) ? parsedProduct : 0;
            Product? product = available.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Invalid or out-of-stock product.");
                continue;
            }

            OrderItem? existing = items.FirstOrDefault(i => i.ProductId == productId);
            int remaining = product.StockQuantity - (existing?.Quantity ?? 0);

            Console.WriteLine("Enter Quantity");
            int quantity = int.TryParse(Console.ReadLine(), out int parsedQty) ? parsedQty : 0;
            if (quantity <= 0)
            {
                Console.WriteLine("Quantity must be greater than zero.");
                continue;
            }
            if (quantity > remaining)
            {
                Console.WriteLine($"Only {remaining} of {product.Name} available.");
                continue;
            }

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                items.Add(new OrderItem
                {
                    CustomerId = customer.CustomerId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }
            Console.WriteLine($"Added {quantity} x {product.Name}.");

            Console.WriteLine("Add another product? (y/n)");
            if (!string.Equals(Console.ReadLine()?.Trim(), "y", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
        }

        if (items.Count == 0)
        {
            Console.WriteLine("No items added. Order cancelled.");
            return null;
        }

        foreach (OrderItem item in items)
        {
            productService.TryReduceStock(item.ProductId, item.Quantity);
        }

        var order = new Orders
        {
            OrderId = nextOrderId++,
            CustomerId = customer.CustomerId,
            Items = items,
            TotalPrice = items.Sum(i => i.UnitPrice * i.Quantity)
        };
        orders.Add(order);

        Console.WriteLine($"Order {order.OrderId} created for {customer.CustomerName}. Total: {order.TotalPrice}");
        return order;
    }

    public List<Orders> GetAllOrders() => orders;
}
