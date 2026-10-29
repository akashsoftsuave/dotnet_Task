using OrderManageMent.Interfaces;

class CustomerService : ICustomerService
{
    private readonly List<Customer> customers = new();

    public Customer? AddCustomer(int customerId, string customerName)
    {
        if (customerId <= 0 || string.IsNullOrWhiteSpace(customerName) || FindById(customerId) != null)
        {
            return null;
        }

        var customer = new Customer { CustomerId = customerId, CustomerName = customerName.Trim() };
        customers.Add(customer);
        return customer;
    }

    public Customer? FindById(int customerId) =>
        customers.FirstOrDefault(c => c.CustomerId == customerId);

    public List<Customer> GetAllCustomers() => customers;
}
