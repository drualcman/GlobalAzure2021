using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WheatherAPI.Polices
{
    public class SopeHandler : AuthorizationHandler<ScopeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, ScopeRequirement requirement)
        {
            // Verificamos si existe el Claim "scope" y que el emisor 
            // del token sea el requerido.
            if (context.User.HasClaim(c => c.Type ==
             "http://schemas.microsoft.com/identity/claims/scope" &&
             c.Issuer == requirement.Issuer))
            {
                // Obtenemos los valores de los scopes que vienen separados 
                // por espacio dentro del claim "scope".
                string[] Scopes = context.User.FindFirst(c => c.Type ==
                "http://schemas.microsoft.com/identity/claims/scope" &&
                c.Issuer == requirement.Issuer).Value.Split(' ');

                // Verificamos que se encuentre el scope requerido
                if (Scopes.Any(s => s == requirement.Scope))
                    // Indicamos que se cumple el requerimiento
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
