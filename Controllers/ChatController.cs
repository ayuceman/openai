using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Models;
using OpenAI_API;
using System.Text;
using System.Net;
using System.Net.Http.Headers;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ChatController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost(Name = "Chat")]
        public async Task<string> ChatAsync([FromBody]string text)
        {
            string apiKey = _configuration["GPT:ApiKey"];
            string prompt = _configuration["GPT:Prompt"];
            string model = _configuration["GPT:Model"];
            int maxToken = Convert.ToInt32(_configuration["GPT:MaxToken"]);
            double temperature = Convert.ToDouble(_configuration["GPT:Temperature"]);


            OpenAIAPI api = new OpenAIAPI(apiKey); // shorthand
            var chat = api.Chat.CreateConversation();

            if (model.Equals("GPT4"))
            {
                chat.Model = Model.GPT4_Turbo;
            }
            else
            {
                chat.Model = Model.ChatGPTTurbo;
            }
               
            chat.RequestParameters.MaxTokens = maxToken;
            chat.RequestParameters.Temperature = temperature;

            /// give instruction as System
            chat.AppendSystemMessage(prompt);

        
            chat.AppendUserInput(text);
            // and get the response
            string data = await chat.GetResponseFromChatbotAsync();


            byte[] byteArray = Encoding.UTF8.GetBytes(data);


            Stream textStream = new MemoryStream(byteArray);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(textStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");  // Set the correct content type
            response.Headers.TransferEncodingChunked = true; ;  // Enable chunking
            return await response.Content.ReadAsStringAsync();


        }
    }
}

