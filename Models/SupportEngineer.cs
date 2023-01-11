using System;
using System.ComponentModel.DataAnnotations;

namespace Ticket_Management.Models
{
	public class SupportEngineer
	{
        [Key]
        public int? se_id { get; set; }
        public int? se_user_id { get; set; }
        public bool? isAvailable { get; set; }

    }
}



