namespace OrderManageMent.Interfaces;

internal interface IProductService
{
    List<Product> GetAvailableProducts();

    Product? FindById(int productId);

    bool TryReduceStock(int productId, int quantity);
}
