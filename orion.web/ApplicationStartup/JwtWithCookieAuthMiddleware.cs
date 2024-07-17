using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Orion.Web.ApplicationStartup
{
    public class JwtWithCookieAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtWithCookieAuthMiddleware> _logger;
        private static readonly string MehKey = Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + DateTimeOffset.UtcNow.Ticks.ToString() + DateTime.Now.ToLongTimeString() + DateTime.Now.ToLongDateString() + Guid.NewGuid().ToString();
        public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(MehKey));
        public static readonly SigningCredentials Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        public const string Issuer = "orion-web";
        private static HashSet<string> excludeFromJwtAuthPaths = new HashSet<string>()
        {
            "/api/token",
            "/api/Notifications",
            "/api/notifications"
        };

        public JwtWithCookieAuthMiddleware(RequestDelegate next, ILogger<JwtWithCookieAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api") && !excludeFromJwtAuthPaths.Contains(context.Request.Path.ToString()))
            {
                if (await IsAuthorized(context))
                {
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Headers.Add("x-login-ep", "/api/token");
                    _logger.LogWarning("api call made without valid token!");
                }
            }
            else
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
        }

        private async Task<bool> IsAuthorized(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var auth = context.Request.Headers["Authorization"];
                if (auth.First().StartsWith("Bearer "))
                {
                    if (await SignInFromToken(context, auth.First()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static readonly string Key = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

        private Task<bool> SignInFromToken(HttpContext context, string token)
        {
            try
            {
                var principal = new JwtSecurityTokenHandler().ValidateToken(
                    token.Replace("Bearer ", ""),
                    new TokenValidationParameters()
                    {
                        IssuerSigningKey = SecurityKey,
                        ValidateAudience = false,
                        ValidIssuer = Issuer
                    },
                    out var toke);

                // var fullToken = toke as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
                // var signInManager = context.RequestServices.GetService<SignInManager<IdentityUser>>();
                // var user = await signInManager.UserManager.FindByNameAsync(fullToken.Subject);
                // await signInManager.SignInAsync(user, true);
                context.User = principal;
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                // eat it
                _logger.LogError(e, "Error trying to validate token");
            }

            return Task.FromResult(false);
        }
    }
}
