using OrderManageMent.Interfaces;

class ConsoleUI
{
    private readonly ICustomerService customerService;
    private readonly IProductService productService;
    private readonly IOrderService orderService;

    public ConsoleUI(ICustomerService customerService, IProductService productService, IOrderService orderService)
    {
        this.customerService = customerService;
        this.productService = productService;
        this.orderService = orderService;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Please select an option:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Create Order");
            Console.WriteLine("2. View Orders");
            int choice = int.TryParse(Console.ReadLine(), out int parsedChoice) ? parsedChoice : -1;

            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    CreateOrder();
                    break;
                case 2:
                    ViewOrders();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private Customer? ReadCustomer()
    {
        Console.WriteLine("Enter Customer ID");
        int customerId = int.TryParse(Console.ReadLine(), out int parsedId) ? parsedId : 0;

        Customer? existing = customerService.FindById(customerId);
        if (existing != null)
        {
            Console.WriteLine($"Welcome back, {existing.CustomerName}.");
            return existing;
        }

        Console.WriteLine("Enter Customer Name");
        string customerName = Console.ReadLine() ?? string.Empty;
        Customer? customer = customerService.AddCustomer(customerId, customerName);
        if (customer == null)
        {
            Console.WriteLine("Invalid customer ID or name.");
        }
        return customer;
    }

    private void CreateOrder()
    {
        Customer? customer = ReadCustomer();
        if (customer == null)
        {
            return;
        }

        List<OrderItem> items = ReadItems(customer);
        if (items.Count == 0)
        {
            Console.WriteLine("No items added. Order cancelled.");
            return;
        }

        Orders? order = orderService.CreateOrder(customer, items);
        if (order == null)
        {
            Console.WriteLine("Order could not be created.");
            return;
        }
        Console.WriteLine($"Order {order.OrderId} created for {customer.CustomerName}. Total: {order.TotalPrice}");
    }

    private List<OrderItem> ReadItems(Customer customer)
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
            available.ForEach(p =>
                Console.WriteLine($"{p.ProductId}. {p.Name} - Price: {p.Price} - Stock: {p.StockQuantity}"));

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

        return items;
    }

    private void ViewOrders()
    {
        List<Orders> orders = orderService.GetAllOrders();
        if (orders.Count == 0)
        {
            Console.WriteLine("No orders yet.");
        }
        orders.ForEach(o =>
        {
            string name = customerService.FindById(o.CustomerId)?.CustomerName ?? "Unknown";
            Console.WriteLine($"Order {o.OrderId} (Customer {o.CustomerId} - {name}) - Total: {o.TotalPrice}");
            o.Items.ForEach(i =>
                Console.WriteLine($"   Product {i.ProductId} x {i.Quantity} @ {i.UnitPrice}"));
        });
    }
}
