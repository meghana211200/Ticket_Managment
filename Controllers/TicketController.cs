using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using Ticket_Management.Models;
using bcrypt = BCrypt.Net.BCrypt;

namespace Ticket_Management.Controllers
{

    public class TicketController : Controller
    {
        private readonly TicketManagementDbContext _context;

        public TicketController(TicketManagementDbContext context)
        {
            _context = context;
        }


        public string payloadData()
        {
            var payloadData = HttpContext.User;
            var ID = "";
            if (payloadData?.Claims != null)
            {
                foreach (var claim in payloadData.Claims)
                {
                    ID = claim.Value;
                    break;
                }
            }
            return ID;
        }



        [HttpPost]
        [Route("createTicket/{id}"), Authorize(Roles = "usr")]
        public async Task<IActionResult> createTicket([FromBody] Ticket ticket, int? id)
        {
            var payloadId = Convert.ToInt32(payloadData());

            if (id == payloadId)
            {
                var user = _context.User.Where(x => x.user_id == payloadId).SingleOrDefault();

                var complaint = new Ticket
                {
                    ticket_user_id = user.user_id,
                    complaint = ticket.complaint,
                    ticket_status="New"
                   
                };

                _context.Ticket.Add(complaint);
                await _context.SaveChangesAsync();
                return Ok("Hey " + complaint.ticket_user_id + ", Your complaint Is Raised Successfully");
            }

            else
            {
                return BadRequest("Incorrect EmpID !");
            }
        }


    }
}