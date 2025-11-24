namespace HTNLShop.ViewModels
{
    public class BestSellingProductVM
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDetail { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int TotalQuantitySold { get; set; }
        public string? ImageUrl { get; set; }
    }
}
