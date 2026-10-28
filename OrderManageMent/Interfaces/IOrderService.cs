namespace OrderManageMent.Interfaces;

internal interface IOrderService
{
    Customer AddCustomer();

    Orders? CreateOrder(Customer customer);

    List<Orders> GetAllOrders();
}
