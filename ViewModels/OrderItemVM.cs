namespace HTNLShop.ViewModels
{
    public class OrderItemVM
    {
        public string ProductName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
