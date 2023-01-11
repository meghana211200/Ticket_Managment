using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ticket_Management.Models
{
    public partial class TicketManagementDbContext : DbContext
    {
        public TicketManagementDbContext(DbContextOptions<TicketManagementDbContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<SupportEngineer> SupportEngineer { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<TicketTracker> TicketTracker { get; set; }
    }

}

