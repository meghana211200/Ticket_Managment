using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using Ticket_Management.Models;
using Ticket_Management.Data;
using bcrypt = BCrypt.Net.BCrypt;

namespace Ticket_Management.Controllers
{
    [ApiController]
    public class RegisterController : Controller
    {
        private readonly TicketManagementDbContext _context;

        public RegisterController(TicketManagementDbContext context)
        {
            _context = context;
        }



        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register([FromBody] User userInfo)
        {
            var checkEmail = await _context.User.FirstOrDefaultAsync(x => x.user_email == userInfo.user_email);
            if (checkEmail == null)
            {
                if (userInfo.user_email.Contains("@gmail.com") && userInfo.user_password.Length > 8)
                {
                    userInfo.user_password = bcrypt.HashPassword(userInfo.user_password, 12);
                    _context.User.Add(userInfo);
                    await _context.SaveChangesAsync();
                    if (userInfo.user_role == "se")
                    {
                        var supportEngineer = new SupportEngineer
                        {
                            se_user_id = userInfo.user_id,
                            isAvailable = true

                        };
                        _context.SupportEngineer.Add(supportEngineer);
                        await _context.SaveChangesAsync();
                    }
                    return Ok("User Created Successfully");
                }
                else
                {
                    return BadRequest("Inputs does not meet requirements !");
                }

            }
            else
            {
                return BadRequest("User already exists !");
            }
        }

    }
}