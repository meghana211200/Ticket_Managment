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

    public class TicketController : Controller
    {
        private readonly TicketManagementDbContext _context;

        public TicketController(TicketManagementDbContext context)
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
                    ticket_status = "New"

                };

                _context.Ticket.Add(complaint);
                await _context.SaveChangesAsync();
                return Ok("Hey " + complaint.ticket_user_id + ", Your complaint Is Raised Successfully");
            }

            else
            {
                return BadRequest("Incorrect UserID !");
            }
        }




        [HttpGet]
        [Route("getTickets/{id}"), Authorize(Roles = "usr")]
        public async Task<IActionResult> getTicket(int? id)
        {
            var payloadId = Convert.ToInt32(payloadData());

            if (id == payloadId)
            {
                var ticket = _context.Ticket.Where(x => x.ticket_user_id == id);
                return Ok(ticket);
            }

            else
            {
                return BadRequest("Incorrect UserID !");
            }
        }

        [HttpPut]
        [Route("updateTicket/{TicketNo}"), Authorize(Roles = "usr")]
        public async Task<IActionResult> updateTicket([FromBody] Ticket ticket, int? TicketNo)
        {
            var payloadId = Convert.ToInt32(payloadData());

            var updateTicket = _context.Ticket.Where(x => x.ticket_id == TicketNo).SingleOrDefault();

            if (updateTicket.ticket_user_id == payloadId)
            {
                updateTicket.complaint = ticket.complaint;
                _context.SaveChanges();
                return Ok("Issue Ticket " + updateTicket.ticket_id + " Is Being Updated");
            }

            else
            {
                return BadRequest("Incorrect EmpID !");
            }
        }


        [HttpDelete]
        [Route("deleteTicket/{TicketNo}"), Authorize(Roles = "usr")]
        public async Task<IActionResult> deleteTicket(int? TicketNo)
        {
            var payloadId = Convert.ToInt32(payloadData());

            var ticket = _context.Ticket.Where(e => e.ticket_id == TicketNo).SingleOrDefault();
            if (ticket == null)
            {
                return BadRequest("Ticket not present");
            }
            else
            {


                if (ticket.ticket_user_id == payloadId)
                {
                    _context.Ticket.Remove(ticket);
                    _context.SaveChanges();

                    return Ok("Your Ticket " + ticket.ticket_id + " is Deleted Successfully");
                }

                else
                {
                    return BadRequest("");
                }
            }
        }


    }
}