using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Models;
using System.Security.Cryptography.X509Certificates;

namespace mvcNestify.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Agent>? Agents { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
    }
}