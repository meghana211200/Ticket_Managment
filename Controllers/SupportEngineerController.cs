using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using Ticket_Management.Models;
using Ticket_Management.Data;
using bcrypt = BCrypt.Net.BCrypt;

namespace Ticket_Management.Controllers
{

    public class SupportEngineerController : Controller
    {
        private readonly TicketManagementDbContext _context;

        public SupportEngineerController(TicketManagementDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Ticket> ticket = _context.Ticket;
            return View(ticket);
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


        [HttpPut]
        [Route("closeTicket/{id}"), Authorize(Roles = "se")]
        public async Task<IActionResult> closeTicket([FromBody] TicketTrackerDTO ticketTracker, int? id)
        {
            var payloadId = Convert.ToInt32(payloadData());

            if (id == payloadId)
            {
                var updateTicket = _context.Ticket.Where(x => x.ticket_id == ticketTracker.ticket_id).SingleOrDefault();
                updateTicket.ticket_status = "Resolved";
                _context.SaveChanges();

                var supportEngineer = _context.SupportEngineer.Where(x => x.se_id == ticketTracker.se_id).SingleOrDefault();
                supportEngineer.isAvailable = true;
                _context.SaveChanges();


                var ticket = _context.TicketTracker.Where(e => e.ticket_id == ticketTracker.ticket_id).SingleOrDefault();
                if (ticket == null)
                {
                    return BadRequest("Ticket not present");
                }
                else
                {

                    _context.TicketTracker.Remove(ticket);
                    _context.SaveChanges();

                    return Ok("Your Ticket " + ticket.ticket_id + " is Deleted Successfully");

                }
            }

            else
            {
                return BadRequest("Incorrect UserID !");
            }
        }



    }
}