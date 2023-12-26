using Microsoft.AspNetCore.Mvc;

using OpenAI_API;
using OpenAI_API.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {



            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //public async Task<string> SummarizeAsync(string text)
        //{

        //    var kernel = Kernel.CreateBuilder();

        //    var _modelName = "text-davinci-003";
        //    var _apiKey = "fdsgfsdgsdgdfgfdsggsdgsdgf";

        //    kernel.AddOpenAIChatCompletion(_modelName,_apiKey);
        //    //AddOpenAITextCompletionService(_modelName, _apiKey);
        //    kernel.Build();
        //    var skills = kernel.Build().ImportPluginFromPromptDirectory("Skills", "Summarization");

        //    var input = new ContextVariables
        //    {
        //        ["input"] = text
        //    };

        //    var output = await kernel.RunAsync(input, skills["Summary"]);

        //    return output.Result;
        //}
        [HttpPost(Name = "SummarizeAsync")]
        public async Task<string> SummarizeAsync(string text)
        {

            OpenAIAPI api = new OpenAIAPI("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"); // shorthand
            var chat = api.Chat.CreateConversation();
            chat.Model = Model.GPT4_Turbo;
            chat.RequestParameters.Temperature = 0;

            /// give instruction as System
            chat.AppendSystemMessage("You are a helpful assistant. Use you pretrained knowledge to best answer the users query.");

            // give a few examples as user and assistant
         //   chat.AppendUserInput("Is this an animal? Cat");
           // chat.AppendExampleChatbotOutput("Yes");
           // chat.AppendUserInput("Is this an animal? House");
           // chat.AppendExampleChatbotOutput("No");

            // now let's ask it a question
            chat.AppendUserInput(text);
            // and get the response
            string response = await chat.GetResponseFromChatbotAsync();

            return response;
        }
    }
    
}
