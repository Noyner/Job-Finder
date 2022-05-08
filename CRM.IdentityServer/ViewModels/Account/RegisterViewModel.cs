using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.IdentityServer.ViewModels.Account
{
    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "Имя пользователя")] 
        public string Username { get; set; }
        
        [EmailAddress]
        [Required]
        [Display(Name="Адрес электронной почты")]
        public string Email { get; set; }
        
        
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string PasswordConfirm { get; set; }
        
        [Required]
        [Display(Name = "Имя")] 
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Фамилия")] 
        public string LastName { get; set; }
        
        [Display(Name = "Отчество")] 
        public string FathersName { get; set; }
        
        [Display(Name = "Дата рождения")] 
        public DateTime DateOfBirth { get; set; }
    }
}