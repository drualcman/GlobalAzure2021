using WeatherApiClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;

namespace WeatherApiClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet("/get/the/authorization/code")]
        public IActionResult GetTheCode()
        {
            string Authorization_Endpoint =
                Configuration["OAuth:Authorization_Endpoint"];
            string Response_Type = "code";
            string Client_Id =
                Configuration["OAuth:Client_Id"];
            string Redirect_Uri =
                Configuration["OAuth:Redirect_Uri"];
            string Scope =
                Configuration["OAuth:Scope"];
            const string State = "ThisIsMyStateValue";

            string URL = $"{Authorization_Endpoint}?" +
                $"response_type={Response_Type}&" +
                $"client_id={Client_Id}&" +
                $"redirect_uri={Redirect_Uri}&" +
                $"scope={Scope}&state={State}";
            return Redirect(URL);
        }

        [HttpGet("/authentication/login-callback")]
        public IActionResult LoginCallback([FromQuery] string code, [FromQuery] string state)
        {
            return View((code, state));
        }


        [HttpGet("/exchange/the/authorization/code/for/an/access/token")]
        public async Task<IActionResult>
            ExchangeTheAuthorizationCodeForAnAccessToken(
            string code, string state)
        {
            const string Grant_Type = "authorization_code";

            string Token_Endpoint =
                Configuration["OAuth:Token_Endpoint"];
            string Redirect_Uri =
                Configuration["OAuth:Redirect_Uri"];
            string Client_Id =
                Configuration["OAuth:Client_Id"];
            string Client_Secret =
                Configuration["OAuth:Client_Secret"];
            string Scope =
                Configuration["OAuth:Scope"];

            Dictionary<string, string> BodyData =
                new Dictionary<string, string>
            {
                {"grant_type", Grant_Type },
                {"code", code },
                {"redirect_uri", Redirect_Uri },
                {"client_id", Client_Id},
                {"client_secret", Client_Secret},
                {"scope", Scope}
            };

            HttpClient HttpClient = new HttpClient();
            FormUrlEncodedContent Body = new FormUrlEncodedContent(BodyData);

            HttpResponseMessage Response =
                await HttpClient.PostAsync(Token_Endpoint, Body);
            string Status =
                $"{(int)Response.StatusCode} {Response.ReasonPhrase}";

            JsonElement JsonContent =
                await Response.Content.ReadFromJsonAsync<JsonElement>();

            string PrettyPrintJsonContent = JsonSerializer.Serialize(JsonContent,
                new JsonSerializerOptions { WriteIndented = true });

            return View(
                (Status, PrettyPrintJsonContent, Response.IsSuccessStatusCode));
        }

        [HttpPost("/call/the/api")]
        public async Task<IActionResult> CallTheApi(string token)
        {
            string AccessToken =
                JsonDocument.Parse(token).RootElement
                .GetProperty("access_token").GetString();

            string Api_Endpoint =
                Configuration["OAuth:Api_Endpoint"];

            HttpClient HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage Response = await HttpClient.GetAsync(Api_Endpoint);

            (string Status, string Content) Model;
            Model.Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            if (Response.IsSuccessStatusCode)
            {
                JsonElement JsonElement =
                    JsonSerializer.Deserialize<JsonElement>(
                        await Response.Content.ReadAsStringAsync());

                Model.Content =
                    JsonSerializer.Serialize(JsonElement,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
            }
            else
            {
                Model.Content =
                    await Response.Content.ReadAsStringAsync();
            }
            return View(Model);
        }

    }
}
