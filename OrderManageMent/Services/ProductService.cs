using OrderManageMent.Interfaces;

class ProductService : IProductService
{
    private readonly List<Product> products = new()
    {
        new Product { ProductId = 1, Name = "Laptop", Price = 50000m, StockQuantity = 10 },
        new Product { ProductId = 2, Name = "Mouse", Price = 500m, StockQuantity = 50 },
        new Product { ProductId = 3, Name = "Keyboard", Price = 1500m, StockQuantity = 20 }
    };

    public List<Product> GetAvailableProducts() =>
        products.Where(p => p.StockQuantity > 0).ToList();

    public Product? FindById(int productId) =>
        products.FirstOrDefault(p => p.ProductId == productId);

    public bool TryReduceStock(int productId, int quantity)
    {
        Product? product = FindById(productId);
        if (product == null || quantity <= 0 || product.StockQuantity < quantity)
        {
            return false;
        }

        product.StockQuantity -= quantity;
        return true;
    }
}
