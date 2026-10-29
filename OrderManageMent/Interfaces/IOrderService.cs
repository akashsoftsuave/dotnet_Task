namespace OrderManageMent.Interfaces;

internal interface IOrderService
{
    Orders? CreateOrder(Customer customer, List<OrderItem> items);

    List<Orders> GetAllOrders();
}
