namespace HTNLShop.ViewModels
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int ProductCount { get; set; }
    }
}
