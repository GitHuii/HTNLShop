using HTNLShop.Data;
using System.ComponentModel.DataAnnotations;

namespace HTNLShop.ViewModels
{
    public class RegisterVM
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "*")]
        [MaxLength(50,ErrorMessage ="Tối đa 50 kí tự")]
        [Display(Name ="Họ và tên")]
        public string? FullName { get; set; }
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "*")]
        [MaxLength(13, ErrorMessage = "Tối đa 13 kí tự")]
        [RegularExpression(@"^(0|\+84)(3|5|7|8|9)[0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = null!;
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = null!;
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        [Display(Name = "User name")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public int RoleId = 1; // id for customer

    }
}
