using OrderManageMent.Interfaces;

class OrderService : IOrderService
{
    private readonly List<Orders> orders = new();
    private readonly IProductService productService;
    private int nextOrderId = 1;

    public OrderService(IProductService productService)
    {
        this.productService = productService;
    }

    public Orders? CreateOrder(Customer customer, List<OrderItem> items)
    {
        if (items.Count == 0)
        {
            return null;
        }

        foreach (OrderItem item in items)
        {
            Product? product = productService.FindById(item.ProductId);
            if (product == null || item.Quantity <= 0 || product.StockQuantity < item.Quantity)
            {
                return null;
            }
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
        return order;
    }

    public List<Orders> GetAllOrders() => orders;
}
