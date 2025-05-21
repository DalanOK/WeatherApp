using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.ViewModels
{
    public class ChatFormViewModel
    {
        public List<SelectListItem> Questions { get; set; }
        public string SelectedQuestionKey { get; set; }

        public string Role { get; set; }     // необязательное
        [Required]
        public string City { get; set; }     // обязательно

        // Результат от ChatGPT
        public string ChatResponse { get; set; }
    }
}
