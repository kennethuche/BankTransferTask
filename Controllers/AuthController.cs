using BankTransferTask.Core.Data;
using BankTransferTask.Core.Entities;
using BankTransferTask.Core.Models.Payloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankTransferTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        public IConfiguration configuration;
      
        public AuthController( IConfiguration configuration)
        {
           
            this.configuration = configuration;
        }


        /// <summary>
        /// Endpoint for Authorization
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Post(AuthPayload payload)
        {

            if (payload is not null)
            {

                var jwt = configuration.GetSection("Jwt").Get<Jwt>();

                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id",Guid.NewGuid().ToString()),
                        new Claim("UserName", payload.Username),
                        new Claim("Password", payload.Password)

                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                await Task.Delay(100);
                var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));

            }
            else
            {
                return BadRequest("Invalid Request");

            }

        }
    }
}
