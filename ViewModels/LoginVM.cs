using System.ComponentModel.DataAnnotations;

namespace HTNLShop.ViewModels
{
    public class LoginVM
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        [Display(Name = "User name")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
