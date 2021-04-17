using BlazorWeatherApiClient.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWeatherApiClient.Pages
{
    public partial class Index 
    {
        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        protected async override Task OnInitializedAsync()
        {
            PKCEHelper.GenerateCodes();
            // Guardar el valor para utilizarlo posteriormente
            await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", "cv",
                PKCEHelper.Code_Verifier);
        }
        void GetTheCode()
        {
            const string Response_Type = "code";

            string Authorization_Endpoint =
                Configuration["OAuth:Authorization_Endpoint"];
            string Client_Id =
                Configuration["OAuth:Client_Id"];
            string Redirect_Uri =
                Configuration["OAuth:Redirect_Uri"];
            string Scope =
                Configuration["OAuth:Scope"];

            string URL = $"{Authorization_Endpoint}?" +
                $"response_type={Response_Type}&" +
                $"client_id={Client_Id}&" +
                $"redirect_uri={Redirect_Uri}&" +
                $"scope={Scope}&" +
                $"code_challenge={Helpers.PKCEHelper.Code_Challenge}&" +
                $"code_challenge_method={PKCEHelper.Code_Challenge_Method}&" +
                $"state=MyStateValue";

            NavigationManager.NavigateTo(URL);
        }
    }
}
