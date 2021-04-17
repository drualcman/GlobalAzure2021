using Microsoft.AspNetCore.Authorization;
using System;

namespace WeatherApiServer.Polices
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; set; }
        public string Scope { get; set; }
        public ScopeRequirement(string scope, string issuer)
        {
            Issuer = issuer ??
                throw new ArgumentNullException(nameof(issuer));
            Scope = scope ??
                throw new ArgumentNullException(nameof(scope));
        }
    }
}
