using Microsoft.AspNetCore.Identity;

namespace WeatherApp.Models
{
    public class Users : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public bool ReceiveNotifications { get; set; }

    }
}
