using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NhanVanAPi.Models;


namespace NhanVanAPi.Data
{
    //This class is used to communicate to sql server 

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TestBank> TestBank {  get; set; }// create test bank  table 

        // fluent API to configure how the Etity is stored in DB - We can also use the DataAnnotation 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // this is required. Identity table is defined here 

            builder.Entity<TestBank>(entity =>
            {
                entity.Property(t=>t.Title).IsRequired().HasMaxLength(200); // 200 characters

                entity.Property(t => t.Category).HasConversion<string>().HasMaxLength(20).IsRequired();

                // category is an enum type . so the value should be stored as a number. HasConversion<string>() turns it back to string
                entity.HasIndex(t => t.Category); // Adding index on the category . searching faster . wont seach on every row

            });

        }

    }
}
