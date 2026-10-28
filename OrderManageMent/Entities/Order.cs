class Orders
{
    public int OrderId {get; set;}
    public int CustomerId {get; set;}
    public List<OrderItem> Items = new();
    public decimal TotalPrice {get; set;}
}
