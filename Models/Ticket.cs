using System;
using System.ComponentModel.DataAnnotations;

namespace Ticket_Management.Models
{
    public class Ticket
    {

        [Key]
        public int ticket_id { get; set; }
        public int ticket_user_id { get; set; }
        public string complaint { get; set; }
        public string? ticket_status { get; set; } = "New";
    }
}

