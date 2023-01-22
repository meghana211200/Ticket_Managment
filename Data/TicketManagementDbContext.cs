using System;
using Microsoft.EntityFrameworkCore;
using Ticket_Management.Models;

namespace Ticket_Management.Data
{
    public partial class TicketManagementDbContext : DbContext
    {
        public TicketManagementDbContext(DbContextOptions<TicketManagementDbContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<SupportEngineer> SupportEngineer { get; set; }
        public DbSet<TicketTracker> TicketTracker { get; set; }
    }
}

