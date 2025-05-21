namespace WeatherApp.Models.AIModels
{
    public class ChatPayload
    {
        public string QuestionKey { get; set; }
        public string Role { get; set; }
        public string City { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
