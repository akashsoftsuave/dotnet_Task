using OrderManageMent.Interfaces;

class OrderManageMentSystem
{
    public static void Main()
    {
        IProductService productService = new ProductService();
        ICustomerService customerService = new CustomerService();
        IOrderService orderService = new OrderService(productService);

        new ConsoleUI(customerService, productService, orderService).Run();
    }
}
