using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorWeatherApiClient.Pages
{
    public partial class CallTheApi
    {
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        string APIResponse;

        protected async override Task OnInitializedAsync()
        {
            string Api_Endpoint =
                Configuration["OAuth:Api_Endpoint"];

            // Obtenemos el token de acceso que guardamos previamente
            string Content =
                await JSRuntime.InvokeAsync<string>(
                    "sessionStorage.getItem", "content");
            JsonElement JsonElement =
                JsonSerializer.Deserialize<JsonElement>(Content);
            string Token = JsonElement.GetProperty("access_token").ToString();

            HttpClient HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", Token);

            HttpResponseMessage Response = await HttpClient.GetAsync(Api_Endpoint);

            if (Response.IsSuccessStatusCode)
            {
                JsonElement =
                    await Response.Content.ReadFromJsonAsync<JsonElement>();
                APIResponse = JsonSerializer.Serialize(JsonElement,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
            }
            else
            {
                APIResponse =
                    $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
            }
        }
    }
}
