class OrderManageMentSystem
{
    public static void Main()
    {
        OrderService service = new OrderService();

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
                    Customer customer = service.AddCustomer();
                    service.CreateOrder(customer);
                    break;
                case 2:
                    List<Orders> orders = service.GetAllOrders();
                    if (orders.Count == 0)
                    {
                        Console.WriteLine("No orders yet.");
                    }
                    foreach (Orders o in orders)
                    {
                        Console.WriteLine($"Order {o.OrderId} (Customer {o.CustomerId}) - Total: {o.TotalPrice}");
                        foreach (OrderItem i in o.Items)
                        {
                            Console.WriteLine($"   Product {i.ProductId} x {i.Quantity} @ {i.UnitPrice}");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
