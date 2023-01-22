using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ticket_Management.Data;
using Ticket_Management.Models;
using bcrypt = BCrypt.Net.BCrypt;

namespace Ticket_Management.Controllers
{

    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TicketManagementDbContext _context;

        public LoginController(IConfiguration configuration, TicketManagementDbContext context)
        {
            _context = context;
            _configuration = configuration;
        }



        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDTO login)
        {
            var checkEmail = await _context.User.FirstOrDefaultAsync(x => x.user_email == login.email);
            if (checkEmail != null)
            {
                if (bcrypt.Verify(login.password, checkEmail.user_password))
                {
                    var token = CreateToken(checkEmail);
                    return Ok(token);
                }
                else
                {
                    return BadRequest("Wrong Password");
                }
            }
            else
            {
                return BadRequest("Invalid User");
            }
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("ID",user.user_id.ToString()),
                new Claim(ClaimTypes.Email, user.user_email),
                new Claim(ClaimTypes.Role, user.user_role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("SecretKey:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}