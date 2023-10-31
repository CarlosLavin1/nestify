using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Models;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;

namespace mvcNestify.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Agent>? Agents { get; set; }
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<Listing>? Listings { get; set; }
        public DbSet<Contract>? Contracts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Showing>()
                .HasKey(lc => new { lc.ListingID, lc.CustomerID });

            builder.Entity<Listing>()
                .HasMany(s => s.Contract)
                .WithOne(e => e.Listing)
                .HasForeignKey(e => e.ListingID)
                .HasConstraintName("FK_Contract_Listing_ListingID");

            builder.Entity<Agent>()
            .HasMany(s => s.Contract)
            .WithOne(e => e.ListingAgent)
            .HasForeignKey(e => e.AgentID)
            .HasConstraintName("FK_Contract_Agent_AgentID");

            builder.Entity<Contract>()
                .HasKey(en => new { en.ListingID, en.AgentID });
        }

        public DbSet<Showing>? Showings { get; set; }

        public DbSet<mvcNestify.Models.Image>? Image { get; set; }
    }
}