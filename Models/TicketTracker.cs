using System;
using System.ComponentModel.DataAnnotations;

namespace Ticket_Management.Models
{
	public class TicketTracker
	{
        [Key]
        public int? ticketTracker_id { get; set; }
        public int? ticket_id { get; set; }
        public int? ticketTracker_user_id { get; set; }
        public int? ticketTracker_se_id { get; set; }
        public string? ticketTracker_ticket_status { get; set; }
        
    }
}

