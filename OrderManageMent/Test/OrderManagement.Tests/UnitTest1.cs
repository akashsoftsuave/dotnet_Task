using System.Reflection;
using Xunit;

namespace OrderManagement.Tests;

// The app's service/entity classes are internal, so the tests reach them through reflection.
internal static class Reflect
{
    // OrderItem and Product are public, so they give a safe handle on the app assembly.
    private static readonly Assembly App = typeof(OrderItem).Assembly;

    public static Type TypeOf(string name) => App.GetTypes().Single(t => t.Name == name);

    public static object New(string typeName, params object[] args) =>
        Activator.CreateInstance(TypeOf(typeName), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, args, null)!;

    public static object? Call(object target, string method, params object[] args) =>
        target.GetType().GetMethod(method)!.Invoke(target, args);

    public static T Get<T>(object target, string member)
    {
        PropertyInfo? property = target.GetType().GetProperty(member);
        object? value = property != null ? property.GetValue(target) : target.GetType().GetField(member)!.GetValue(target);
        return (T)value!;
    }

    public static void Set(object target, string property, object value) =>
        target.GetType().GetProperty(property)!.SetValue(target, value);

    public static object NewCustomer(int id, string name)
    {
        object customer = New("Customer");
        Set(customer, "CustomerId", id);
        Set(customer, "CustomerName", name);
        return customer;
    }

    public static OrderItem Item(int productId, int quantity, decimal unitPrice) =>
        new() { CustomerId = 1, ProductId = productId, Quantity = quantity, UnitPrice = unitPrice };

    public static int StockOf(object productService, int productId)
    {
        object product = Call(productService, "FindById", productId)!;
        return Get<int>(product, "StockQuantity");
    }
}

public class ProductServiceTests
{
    [Fact]
    public void GetAvailableProducts_ReturnsThreeProducts()
    {
        object service = Reflect.New("ProductService");

        var result = (List<Product>)Reflect.Call(service, "GetAvailableProducts")!;

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetAvailableProducts_ExcludesProductsOutOfStock()
    {
        object service = Reflect.New("ProductService");
        Reflect.Call(service, "TryReduceStock", 1, 10); // Laptop stock 10 -> 0

        var result = (List<Product>)Reflect.Call(service, "GetAvailableProducts")!;

        Assert.DoesNotContain(result, p => p.ProductId == 1);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void FindById_ExistingProduct_ReturnsProduct()
    {
        object service = Reflect.New("ProductService");

        var product = (Product?)Reflect.Call(service, "FindById", 2);

        Assert.NotNull(product);
        Assert.Equal("Mouse", product!.Name);
        Assert.Equal(500m, product.Price);
    }

    [Fact]
    public void FindById_UnknownProduct_ReturnsNull()
    {
        object service = Reflect.New("ProductService");

        Assert.Null(Reflect.Call(service, "FindById", 99));
    }

    [Fact]
    public void TryReduceStock_ValidQuantity_ReducesStock()
    {
        object service = Reflect.New("ProductService");

        var result = (bool)Reflect.Call(service, "TryReduceStock", 2, 5)!;

        Assert.True(result);
        Assert.Equal(45, Reflect.StockOf(service, 2));
    }

    [Fact]
    public void TryReduceStock_ExactStock_ReducesToZero()
    {
        object service = Reflect.New("ProductService");

        var result = (bool)Reflect.Call(service, "TryReduceStock", 1, 10)!;

        Assert.True(result);
        Assert.Equal(0, Reflect.StockOf(service, 1));
    }

    [Theory]
    [InlineData(1, 11)]  // more than stock
    [InlineData(1, 0)]   // zero quantity
    [InlineData(1, -3)]  // negative quantity
    [InlineData(99, 1)]  // unknown product
    public void TryReduceStock_InvalidRequest_ReturnsFalseAndKeepsStock(int productId, int quantity)
    {
        object service = Reflect.New("ProductService");

        var result = (bool)Reflect.Call(service, "TryReduceStock", productId, quantity)!;

        Assert.False(result);
        Assert.Equal(10, Reflect.StockOf(service, 1));
    }
}

public class CustomerServiceTests
{
    [Fact]
    public void AddCustomer_ValidDetails_ReturnsCreatedCustomer()
    {
        object service = Reflect.New("CustomerService");

        object? result = Reflect.Call(service, "AddCustomer", 1, "Akash");

        Assert.NotNull(result);
        Assert.Equal(1, Reflect.Get<int>(result!, "CustomerId"));
        Assert.Equal("Akash", Reflect.Get<string>(result!, "CustomerName"));
    }

    [Fact]
    public void AddCustomer_NameWithSpaces_TrimsName()
    {
        object service = Reflect.New("CustomerService");

        object? result = Reflect.Call(service, "AddCustomer", 1, "  Asha ");

        Assert.Equal("Asha", Reflect.Get<string>(result!, "CustomerName"));
    }

    [Fact]
    public void AddCustomer_ValidDetails_StoresCustomer()
    {
        object service = Reflect.New("CustomerService");

        Reflect.Call(service, "AddCustomer", 1, "Akash");

        var all = (System.Collections.IList)Reflect.Call(service, "GetAllCustomers")!;
        Assert.Single(all);
    }

    [Fact]
    public void AddCustomer_DuplicateId_ReturnsNullAndDoesNotAdd()
    {
        object service = Reflect.New("CustomerService");
        Reflect.Call(service, "AddCustomer", 1, "Akash");

        object? duplicate = Reflect.Call(service, "AddCustomer", 1, "Ravi");

        Assert.Null(duplicate);
        var all = (System.Collections.IList)Reflect.Call(service, "GetAllCustomers")!;
        Assert.Single(all);
    }

    [Theory]
    [InlineData(0, "Akash")]
    [InlineData(-1, "Akash")]
    [InlineData(1, "")]
    [InlineData(1, "   ")]
    public void AddCustomer_InvalidDetails_ReturnsNullAndDoesNotAdd(int id, string name)
    {
        object service = Reflect.New("CustomerService");

        object? result = Reflect.Call(service, "AddCustomer", id, name);

        Assert.Null(result);
        var all = (System.Collections.IList)Reflect.Call(service, "GetAllCustomers")!;
        Assert.Empty(all);
    }

    [Fact]
    public void FindById_ExistingCustomer_ReturnsCustomer()
    {
        object service = Reflect.New("CustomerService");
        Reflect.Call(service, "AddCustomer", 5, "Meena");

        object? found = Reflect.Call(service, "FindById", 5);

        Assert.NotNull(found);
        Assert.Equal("Meena", Reflect.Get<string>(found!, "CustomerName"));
    }

    [Fact]
    public void FindById_UnknownCustomer_ReturnsNull()
    {
        object service = Reflect.New("CustomerService");

        Assert.Null(Reflect.Call(service, "FindById", 6));
    }

    [Fact]
    public void GetAllCustomers_NewService_IsEmpty()
    {
        object service = Reflect.New("CustomerService");

        var all = (System.Collections.IList)Reflect.Call(service, "GetAllCustomers")!;

        Assert.Empty(all);
    }
}

public class OrderServiceTests
{
    private static (object products, object orders) CreateServices()
    {
        object products = Reflect.New("ProductService");
        object orders = Reflect.New("OrderService", products);
        return (products, orders);
    }

    private static object? CreateOrder(object orders, params OrderItem[] items) =>
        Reflect.Call(orders, "CreateOrder", Reflect.NewCustomer(1, "Akash"), items.ToList());

    [Fact]
    public void CreateOrder_ValidDetails_ReturnsCreatedOrder()
    {
        var (products, orders) = CreateServices();

        object? result = CreateOrder(orders, Reflect.Item(1, 2, 50000m));

        Assert.NotNull(result);
        Assert.Equal(1, Reflect.Get<int>(result!, "OrderId"));
        Assert.Equal(1, Reflect.Get<int>(result!, "CustomerId"));
        Assert.Equal(100000m, Reflect.Get<decimal>(result!, "TotalPrice"));
        Assert.Equal(8, Reflect.StockOf(products, 1));
    }

    [Fact]
    public void CreateOrder_MultipleItems_CalculatesTotalAcrossItems()
    {
        var (_, orders) = CreateServices();

        object? result = CreateOrder(orders, Reflect.Item(1, 2, 50000m), Reflect.Item(2, 3, 500m));

        Assert.Equal(101500m, Reflect.Get<decimal>(result!, "TotalPrice"));
        Assert.Equal(2, Reflect.Get<List<OrderItem>>(result!, "Items").Count);
    }

    [Fact]
    public void CreateOrder_ValidDetails_StoresOrder()
    {
        var (_, orders) = CreateServices();

        CreateOrder(orders, Reflect.Item(1, 1, 50000m));

        var all = (System.Collections.IList)Reflect.Call(orders, "GetAllOrders")!;
        Assert.Single(all);
    }

    [Fact]
    public void CreateOrder_MultipleOrders_IncrementsOrderId()
    {
        var (_, orders) = CreateServices();

        object? first = CreateOrder(orders, Reflect.Item(1, 1, 50000m));
        object? second = CreateOrder(orders, Reflect.Item(2, 1, 500m));

        Assert.Equal(1, Reflect.Get<int>(first!, "OrderId"));
        Assert.Equal(2, Reflect.Get<int>(second!, "OrderId"));
    }

    [Fact]
    public void CreateOrder_NoItems_ReturnsNull()
    {
        var (_, orders) = CreateServices();

        Assert.Null(CreateOrder(orders));
        var all = (System.Collections.IList)Reflect.Call(orders, "GetAllOrders")!;
        Assert.Empty(all);
    }

    [Fact]
    public void CreateOrder_QuantityExceedsStock_ReturnsNullAndKeepsStock()
    {
        var (products, orders) = CreateServices();

        object? result = CreateOrder(orders, Reflect.Item(1, 11, 50000m));

        Assert.Null(result);
        Assert.Equal(10, Reflect.StockOf(products, 1));
        var all = (System.Collections.IList)Reflect.Call(orders, "GetAllOrders")!;
        Assert.Empty(all);
    }

    [Fact]
    public void CreateOrder_OneInvalidItem_RejectsWholeOrderWithoutReducingStock()
    {
        var (products, orders) = CreateServices();

        // First item is valid, second exceeds stock: nothing should be deducted.
        object? result = CreateOrder(orders, Reflect.Item(1, 2, 50000m), Reflect.Item(2, 51, 500m));

        Assert.Null(result);
        Assert.Equal(10, Reflect.StockOf(products, 1));
        Assert.Equal(50, Reflect.StockOf(products, 2));
    }

    [Fact]
    public void CreateOrder_UnknownProduct_ReturnsNull()
    {
        var (_, orders) = CreateServices();

        Assert.Null(CreateOrder(orders, Reflect.Item(99, 1, 10m)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateOrder_NonPositiveQuantity_ReturnsNull(int quantity)
    {
        var (_, orders) = CreateServices();

        Assert.Null(CreateOrder(orders, Reflect.Item(1, quantity, 50000m)));
    }

    [Fact]
    public void CreateOrder_SecondOrderExceedsRemainingStock_ReturnsNull()
    {
        var (products, orders) = CreateServices();
        CreateOrder(orders, Reflect.Item(1, 8, 50000m));

        object? second = CreateOrder(orders, Reflect.Item(1, 3, 50000m));

        Assert.Null(second);
        Assert.Equal(2, Reflect.StockOf(products, 1));
    }

    [Fact]
    public void GetAllOrders_NewService_IsEmpty()
    {
        var (_, orders) = CreateServices();

        var all = (System.Collections.IList)Reflect.Call(orders, "GetAllOrders")!;

        Assert.Empty(all);
    }
}
