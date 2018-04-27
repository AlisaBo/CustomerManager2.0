using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CustomerManager2.Models;
using CustomerManager2.Models.CustomerModels;

namespace CustomerManager2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerInformation> CustomerInformations { get; set; }
        public DbSet<ContactsDetail> ContactsDetails { get; set; }
        public DbSet<Departament> Departaments { get; set; }
        public DbSet<LoginUser> LoginUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Departament>().HasMany(t => t.UsersInDepartament).WithOne(p => p.Departament).HasForeignKey(t => t.Id);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
