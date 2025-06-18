using System.ComponentModel.DataAnnotations;

namespace WeatherApp.ViewModels
{
    public class ProfileViewModel
    {
        [Required, EmailAddress]
        [Display(Name = "Электронная почта")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Отримувати повідомлення")]
        public bool ReceiveNotifications { get; set; }

        [Display(Name = "Країна")]
        public string? Country { get; set; }

        [Display(Name = "Місто")]
        public string? City { get; set; }
    }
}
