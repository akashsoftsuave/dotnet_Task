namespace OrderManageMent.Interfaces;

internal interface ICustomerService
{
    Customer? AddCustomer(int customerId, string customerName);

    Customer? FindById(int customerId);

    List<Customer> GetAllCustomers();
}
