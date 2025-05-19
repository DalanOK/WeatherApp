using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using WeatherApp.Models.AIModels;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/ai/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ChatClient _chat;

        public ChatController(ChatClient chatClient)
        {
            _chat = chatClient;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatRequest request)
        {
            ChatCompletion completion = await _chat.CompleteChatAsync(request.Prompt);

            string reply = completion.Content[0].Text;

            return Ok(new ChatResponse { Reply = reply });
        }
    }
}
