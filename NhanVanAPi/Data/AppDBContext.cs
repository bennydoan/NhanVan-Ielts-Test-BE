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
        public DbSet<TestSection> TestSections { get; set; } // Test Section Table 

        public DbSet<QuestionGroup> QuestionGroups { get; set; }

        //public DbSet<Question> Questions { get; set; }

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

            builder.Entity<TestSection>(entity =>
            {
                entity.HasOne<TestBank>()
                .WithMany(t=>t.Sections).HasForeignKey(s=>s.TestBankId)
                .OnDelete(DeleteBehavior.Cascade); // IF  you delete the test -> all sections belonging to that test will be deleted as well
                entity.HasIndex(s => new { s.TestBankId, s.SectionNumber }).IsUnique();
                entity.Property(s => s.AudioUrl).HasMaxLength(500);
            });


            builder.Entity<QuestionGroup>(entity =>
            {
                entity.HasOne<TestSection>()                    // each group belongs to ONE section
                      .WithMany(s => s.Groups)                  // each section has MANY groups
                      .HasForeignKey(g => g.TestSectionId)
                      .OnDelete(DeleteBehavior.Cascade);        // delete a section → its groups are deleted

                entity.HasIndex(g => new { g.TestSectionId, g.Order }).IsUnique();
                entity.Property(g => g.Type).HasConversion<string>().HasMaxLength(30);
                entity.Property(g => g.Label).HasMaxLength(100);
                entity.Property(g => g.ImageUrl).HasMaxLength(500);
                entity.OwnsMany(g => g.Passages, p => p.ToJson()); // one json collum
            });

            builder.Entity<Question>(entity =>
            {
                entity.HasOne<QuestionGroup>().
                WithMany(g => g.Questions).HasForeignKey(q => q.QuestionGroupId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(q => new { q.QuestionGroupId, q.QuestionNumber }).IsUnique(); // no duplicate "Question 3" in a group
                entity.OwnsMany(q => q.Passage, p => p.ToJson()); // List<PassagePart> → JSON column
            });

        }
    }
}
