using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using orion.web.ApplicationStartup;
using orion.web.Jobs;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace orion.web.UI.api
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
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ISitesRepository _sitesRepository;
        private readonly IMapper _mapper;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TokenController(ISitesRepository sitesRepository, IMapper mapper, SignInManager<IdentityUser> signInManager)
        {
            _sitesRepository = sitesRepository;
            _mapper = mapper;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TokenLogin login)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(login.Email);
            if(user != null)
            {
                if(await _signInManager.UserManager.CheckPasswordAsync(user, login.Password))
                {
                    var toke = GenerateJSONWebToken();
                    if(toke != null)
                    {
                        return new ObjectResult(toke)
                        {
                            StatusCode = StatusCodes.Status201Created
                        };

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

        private GenerateTokenResult GenerateJSONWebToken()
        {
            var claims = new[] {
                        new Claim("sub","admin@company.com"),
                        new Claim(ClaimTypes.Role,"Admin"),
                        new Claim(ClaimTypes.Name, "admin@company.com"),
                        new Claim("orion:employee-id",12.ToString())
                        };

            var token = new JwtSecurityToken(JwtWithCookieAuthMiddleware.Issuer,
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
