using Microsoft.EntityFrameworkCore;

namespace HoF_API.Models
{
    public class PersonContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Skill> Skills { get; set; }

        public PersonContext(DbContextOptions<PersonContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Skill>()
                .HasOne(s => s.Person)
                .WithMany(p => p.Skills)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
