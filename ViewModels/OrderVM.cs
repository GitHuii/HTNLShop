using HTNLShop.Data;

namespace HTNLShop.ViewModels
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; }

        // Sản phẩm đầu tiên (hiển thị trong list)
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Tất cả sản phẩm (hiển thị trong detail)
        public List<OrderItemVM> Items { get; set; }
    }
}
