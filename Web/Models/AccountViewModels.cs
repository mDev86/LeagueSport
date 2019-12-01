using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class AccountViewModels
    {
        public class LoginViewModel
        {
            [Required]
            [Display(Name = "Адрес электронной почты")]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Display(Name = "Запомнить")]
            public bool RememberMe { get; set; }
        }
        
        public class RegisterViewModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Адрес электронной почты")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Значение {0} должно содержать не менее {1} символов.")]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
            public string ConfirmPassword { get; set; }
        }
    }
}