using CodeFlowWithOpenIdConnect.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

namespace CodeFlowWithOpenIdConnect.Controllers
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

        [Authorize]
        [HttpGet("/view/authentication/data")]
        public IActionResult ViewAuthnticationData()
        {
            return View();
        }

        [Authorize]
        [HttpGet("/call/the/api")]
        public async Task<IActionResult> CallTheApi()
        {
            string AccessToken =
                await HttpContext.GetTokenAsync("access_token");

            string Api_Endpoint =
                Configuration["OAuth:Api_Endpoint"];

            HttpClient HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AccessToken);

            HttpResponseMessage Response = await HttpClient.GetAsync(Api_Endpoint);

            (string Status, string Content) Model;
            Model.Status =
                $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
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
                        Encoder = System.Text.Encodings.Web
                        .JavaScriptEncoder.UnsafeRelaxedJsonEscaping
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
