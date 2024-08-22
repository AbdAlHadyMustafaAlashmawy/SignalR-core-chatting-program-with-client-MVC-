using Chat_SignalR_Demo.Models;
using Chat_SignalR_Demo.SignalR_Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Chat_SignalR_Demo
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Conversation> conversation { get; set; }
        public virtual DbSet<UserConnection> UserConnections { get; set; }
        public virtual DbSet<UserGroups> UserGroups { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
         : base(options)
        {
        }

    }
}
