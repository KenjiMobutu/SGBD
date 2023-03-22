using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;

namespace MyPoll.Model;

public class MyPollContext : DbContextBase {
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins  => Set<Admin>();
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<Choice> Choices { get; set; }
    public DbSet<Comment> Comments { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder
            //.UseSqlite("Data Source=prbd-2223-a14.db")
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=prbd-2223-a14")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseLazyLoadingProxies(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Name = "ben" },
            new User { UserId = 2, Name = "bruno" }
            );

        modelBuilder.Entity<Admin>().HasData(
            new Admin { UserId = 3, Name = "Mando" }
            );

    }
}
