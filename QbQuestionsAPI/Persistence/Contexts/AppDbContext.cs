using Microsoft.EntityFrameworkCore;

using QbQuestionsAPI.Domain.Models;

namespace QbQuestionsAPI.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<QbQuestion> QbQuestions { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<QbQuestion>().ToTable("QbQuestions");
            builder.Entity<QbQuestion>().HasKey(q => q.Id);
            builder.Entity<QbQuestion>().Property(q => q.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<QbQuestion>().Property(q => q.Level).IsRequired();
            builder.Entity<QbQuestion>().Property(q => q.Tournament).IsRequired().HasMaxLength(50);
            builder.Entity<QbQuestion>().Property(q => q.Year).IsRequired();
            builder.Entity<QbQuestion>().Property(q => q.Power).IsRequired(false);
            builder.Entity<QbQuestion>().Property(q => q.Body).IsRequired();
            builder.Entity<QbQuestion>().Property(q => q.Answer).IsRequired().HasMaxLength(50);
            builder.Entity<QbQuestion>().Property(q => q.Notes).IsRequired(false).HasMaxLength(250);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<User>().HasKey(u => u.Id);
            builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<User>().Property(u => u.Username).IsRequired().HasMaxLength(30);
            builder.Entity<User>().Property(u => u.Password).IsRequired().HasMaxLength(64);
        }
    }
}