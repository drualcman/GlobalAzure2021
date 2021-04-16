using CodeFlow.Models;
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

namespace CodeFlow.Controllers
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


    }
}
