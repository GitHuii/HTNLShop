namespace HTNLShop.ViewModels
{
    public class ReviewVM
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Content { get; set; }
        public int Rate { get; set; }

        public string CreatedAtFormat => CreatedDate.ToString("dd/MM/yyyy");
        public DateTime CreatedDate { get; set; }

        public string? ProductName { get; set; }
    }
}
