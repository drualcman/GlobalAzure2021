using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BlazorWeatherApiClient.Pages
{
    public partial class Authentication
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }

        string Code, State;
        protected override void OnInitialized()
        {
            Uri Uri =
                NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            NameValueCollection QueryString = HttpUtility.ParseQueryString(Uri.Query);

            Code = QueryString["code"];
            State = QueryString["State"];
        }
    }
}
