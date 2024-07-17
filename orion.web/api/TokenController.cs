using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Orion.Web.ApplicationStartup;
using Orion.Web.Employees;
using Orion.Web.Jobs;

namespace Orion.Web.Api
{
    public class TokenLogin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    [Route("api/token")]
    [Route("orion-api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TokenController(IEmployeeRepository employeeRepository, SignInManager<IdentityUser> signInManager)
        {
            _employeeRepository = employeeRepository;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TokenLogin login)
        {
            if (!string.IsNullOrWhiteSpace(login?.Email))
            {
                var user = await _signInManager.UserManager.FindByNameAsync(login.Email);
                if (user != null)
                {
                    if (await _signInManager.UserManager.CheckPasswordAsync(user, login.Password))
                    {
                        var toke = await GenerateJSONWebToken(user);
                        if (toke != null)
                        {
                            return new ObjectResult(toke)
                            {
                                StatusCode = StatusCodes.Status201Created
                            };
                        }
                    }
                }
            }

            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public class GenerateTokenResult
        {
            public string AccessToken { get; set; }
            public DateTimeOffset ValidFrom { get; set; }
            public DateTimeOffset ValidTo { get; set; }
        }

        private async Task<GenerateTokenResult> GenerateJSONWebToken(IdentityUser user)
        {
            var emp = await _employeeRepository.GetSingleEmployeeAsync(user.UserName);
            var claims = new[]
            {
                        new Claim("sub", emp.UserName),
                        new Claim("orion:first", emp.First),
                        new Claim("orion:last", emp.Last),
                        new Claim(ClaimTypes.Role, emp.Role),
                        new Claim(ClaimTypes.Name, emp.UserName),
                        new Claim("orion:employee-id", emp.EmployeeId.ToString())
            };

            var token = new JwtSecurityToken(
                JwtWithCookieAuthMiddleware.Issuer,
                JwtWithCookieAuthMiddleware.Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(120),
                notBefore: DateTime.Now.AddMinutes(-5),
                signingCredentials: JwtWithCookieAuthMiddleware.Credentials);

            var tokey = new JwtSecurityTokenHandler().WriteToken(token);
            return new GenerateTokenResult()
            {
                AccessToken = tokey,
                ValidFrom = new DateTimeOffset(token.ValidFrom).ToOffset(TimeZoneInfo.Local.GetUtcOffset(DateTime.Now.AddMinutes(-5))),
                ValidTo = new DateTimeOffset(token.ValidTo).ToOffset(TimeZoneInfo.Local.GetUtcOffset(DateTime.Now.AddMinutes(120))),
            };
        }
    }
}
