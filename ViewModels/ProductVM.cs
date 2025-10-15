namespace HTNLShop.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public double Price { get; set; }
        public string ProductDetail { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        //public int StockQuantity { get; set; }
    }
}
