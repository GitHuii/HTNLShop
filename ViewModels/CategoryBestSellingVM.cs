namespace HTNLShop.ViewModels
{
    public class CategoryBestSellingVM
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<BestSellingProductVM>? TopProducts { get; set; }
    }
}
