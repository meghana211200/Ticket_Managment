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

    public class AdminController : Controller
    {
        private readonly TicketManagementDbContext _context;

        public AdminController(TicketManagementDbContext context)
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


        [HttpGet]
        [Route("getAllTickets/{id}"), Authorize(Roles = "adm")]
        public async Task<IActionResult> getAllTicket(int? id)
        {
            var payloadId = Convert.ToInt32(payloadData());

            if (id == payloadId)
            {
                var ticket = _context.Ticket;
                return Ok(ticket);
            }

            else
            {
                return BadRequest("Incorrect UserID !");
            }
        }

        [HttpGet]
        [Route("getAllAvailableSupportEng/{id}"), Authorize(Roles = "adm")]
        public async Task<IActionResult> getAllAvailableSupportEng(int? id)
        {
            var payloadId = Convert.ToInt32(payloadData());

            if (id == payloadId)
            {
                var supportEngineers = _context.SupportEngineer.Where(x => x.isAvailable == true);
                return Ok(supportEngineers);
            }

            else
            {
                return BadRequest("Incorrect UserID !");
            }
        }

        [HttpGet]
        [Route("getTicketsFilter/{id}"), Authorize(Roles = "adm")]
        public async Task<IActionResult> getTicketsFilter([FromBody] TicketFilterDTO ticketFilter, int? id)
        {
            var payloadId = Convert.ToInt32(payloadData());

            if (id == payloadId)
            {
                var ticket = _context.Ticket.Where(x => x.ticket_status == ticketFilter.ticketStatus);
                return Ok(ticket);
            }

            else
            {
                return BadRequest("Incorrect UserID !");
            }
        }


        [HttpPost]
        [Route("assignTicket/{id}"), Authorize(Roles = "adm")]
        public async Task<IActionResult> assignTicket([FromBody] TicketTrackerDTO ticketTracker, int? id)
        {
            var payloadId = Convert.ToInt32(payloadData());
            var checkTicket = await _context.TicketTracker.FirstOrDefaultAsync(x => x.ticket_id == ticketTracker.ticket_id);
            if (checkTicket == null)
            {
                if (id == payloadId)
                {

                    var ticketTrackers = new TicketTracker
                    {

                        ticketTracker_se_id = ticketTracker.se_id,
                        ticket_id = ticketTracker.ticket_id,

                    };

                    _context.TicketTracker.Add(ticketTrackers);
                    await _context.SaveChangesAsync();

                    var updateTicket = _context.Ticket.Where(x => x.ticket_id == ticketTracker.ticket_id).SingleOrDefault();
                    updateTicket.ticket_status = "Assigned";
                    _context.SaveChanges();

                    var supportEngineer = _context.SupportEngineer.Where(x => x.se_id == ticketTracker.se_id).SingleOrDefault();
                    supportEngineer.isAvailable = false;
                    _context.SaveChangesAsync();

                    return Ok("Hey " + ticketTrackers + ", Your complaint Is Raised Successfully");
                }

                else
                {
                    return BadRequest("Incorrect UserID !");
                }
            }
            else
            {
                return BadRequest("Ticket Already Assigned !");
            }
        }



    }
}