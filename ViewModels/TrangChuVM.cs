namespace HTNLShop.ViewModels
{
    public class TrangChuVM
    {
        public List<ProductVM> Laptops { get; set; } = new List<ProductVM>();
        public List<ProductVM> PCs { get; set; } = new List<ProductVM>();
        public List<ProductVM> Mices { get; set; } = new List<ProductVM>();
        public List<ProductVM> Keyboards { get; set; } = new List<ProductVM>();
        public List<ProductVM> Monitors { get; set; } = new List<ProductVM>();
        public List<CategoryBestSellingVM>? Categories { get; set; } = new List<CategoryBestSellingVM>();
    }
}
