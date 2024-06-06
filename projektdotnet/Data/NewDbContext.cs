using Microsoft.EntityFrameworkCore;
using projektdotnet.Models;

namespace projektdotnet.Data
{
    public class NewDbContext : DbContext
    {
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Opinion> Opinions { get; set; }
        public virtual DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TicketComment> TicketComments { get; set; }
        public NewDbContext(DbContextOptions<NewDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = localhost; Database = Projektdotnet; User=Adminek; Password=Adminek123!;TrustServerCertificate=true;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Sender)
                .WithMany(s => s.SentTickets)
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Receiver)
                .WithMany(r => r.ReceivedTickets)
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
