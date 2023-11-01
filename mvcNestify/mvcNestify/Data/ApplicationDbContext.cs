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
                .HasMany(c => c.Contract)
                .WithOne(l => l.Listing)
                .HasForeignKey(l => l.ListingID)
                .HasConstraintName("FK_Contract_Listing_ListingID");

            builder.Entity<Agent>()
                .HasMany(c => c.Contract)
                .WithOne(cust => cust.ListingAgent)
                .HasForeignKey(cust => cust.AgentID)
                .HasConstraintName("FK_Contract_Agent_AgentID");
           
            builder.Entity<Customer>()
                .HasMany(c => c.Contract)
                .WithOne(cust => cust.Customer)
                .HasForeignKey(cust => cust.CustomerID)
                .HasConstraintName("FK_Contract_Customer_CustomerID");
        }

        public DbSet<Showing>? Showings { get; set; }

        public DbSet<mvcNestify.Models.Image>? Image { get; set; }
    }
}