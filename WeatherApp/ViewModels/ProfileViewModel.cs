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

        [Display(Name = "Получать уведомления")]
        public bool ReceiveNotifications { get; set; }
    }
}
