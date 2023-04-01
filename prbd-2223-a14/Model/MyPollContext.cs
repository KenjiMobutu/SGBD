using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using PRBD_Framework;

namespace MyPoll.Model;

public class MyPollContext : DbContextBase {
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins  => Set<Admin>();
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<Participation> Participations => Set<Participation>();
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


        modelBuilder.Entity<Participation>()
            .HasKey(p => new { p.PollId, p.UserId });

        modelBuilder.Entity<Vote>()
            .HasKey(v => new { v.UserId, v.ChoiceId });

        /*------------USER--------------*/

        modelBuilder.Entity<User>()
            .HasMany(u => u.Polls)
            .WithMany(p => p.Participants)
            .UsingEntity(e => e.ToTable("Participation"));

        modelBuilder.Entity<User>()
                    .HasDiscriminator(u => u.Role)
                    .HasValue<User>(Role.Member)
                    .HasValue<Admin>(Role.Admin);

        modelBuilder.Entity<Admin>()
            .HasBaseType<User>();

        // l'entité User participe à une relation one-to-many ... à vérifier avec BP
        /*modelBuilder.Entity<User>()
            // avec, du côté many, la propriété PollsCreator ...
            .HasMany(user => user.Polls)
            // avec, du côté one, la propriété CreatorId ...
            .WithOne(poll => poll.CreatorId)
            // et pour laquelle on active le delete en cascade du côté client (EF)
            .OnDelete(DeleteBehavior.ClientCascade);*/


        modelBuilder.Entity<User>()
            .HasMany(user => user.CommentsList)
            .WithOne(comment => comment.User)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasMany(user => user.VotesList)
            .WithOne(vote => vote.User)
            .OnDelete(DeleteBehavior.ClientCascade);

        /*------------POLL--------------*/
        modelBuilder.Entity<Poll>()
            .HasKey(p => p.PollId);

        modelBuilder.Entity<Poll>()
            .Property(p => p.Title)
            .IsRequired();

        modelBuilder.Entity<Poll>()
            .HasMany(poll => poll.Comments)
            .WithOne(comment => comment.Poll)
            .HasForeignKey(c => c.PollId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Poll>()
            .HasMany(poll => poll.Choices)
            .WithOne(choice => choice.Poll)
            .HasForeignKey(c => c.PollId)
            .OnDelete(DeleteBehavior.ClientCascade);

     

        /*------------CHOICE--------------*/

        modelBuilder.Entity<Choice>()
            .HasMany(choice => choice.VotesList)
            .WithOne(vote => vote.Choice)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Polls)
            .WithMany(p => p.Participants)
            .UsingEntity<Participation>(
                p => p.HasOne(p => p.Poll)
                    .WithMany(p => p.Participations)
                    .HasForeignKey(p => p.PollId),
                p => p.HasOne(p => p.User)
                    .WithMany(u => u.PartcipantsList)
                    .HasForeignKey(p => p.UserId),
                p => { p.HasKey(p => new {p.PollId, p.UserId }); }
            );



        modelBuilder.Entity<Participation>()
            .HasKey(p => new {p.PollId, p.UserId});

        SeedData(modelBuilder);
    }
   /* protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        ...
        // l'entité Member participe à une relation many-to-many ...
        modelBuilder.Entity<Member>()
            // avec, d'un côté, la propriété Followees ...
            .HasMany(m => m.Followees)
            // avec, de l'autre côté, la propriété Followers ...
            .WithMany(m => m.Followers)
            // en utilisant l'entité Follow comme entité "association"
            .UsingEntity<Follow>(
                // celle-ci étant constituée d'une relation one-to-many à partir de Followee
                right => right.HasOne(f => f.Followee).WithMany().HasForeignKey(nameof(Follow.FolloweePseudo))
                    .OnDelete(DeleteBehavior.ClientCascade),
                // et d'une autre relation one-to-many à partir de Follower
                left => left.HasOne(f => f.Follower).WithMany().HasForeignKey(nameof(Follow.FollowerPseudo))
                    .OnDelete(DeleteBehavior.ClientCascade),
                joinEntity => {
                    // en n'oubliant pas de spécifier la clé primaire composée de la table association
                    joinEntity.HasKey(f => new { f.FollowerPseudo, f.FolloweePseudo });
                });
    }*/


    private static void SeedData(ModelBuilder modelBuilder) {
        /*modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Name = "ben" },
            new User { UserId = 2, Name = "bruno" }
            );

        modelBuilder.Entity<Admin>().HasData(
            new Admin { UserId = 3, Name = "Mando" }
            );*/
        
        modelBuilder.Entity<User>()
            .HasData(
                new User {
                    UserId = 1, Name = "Harry Covère", Mail = "harry@test.com", Password = SecretHasher.Hash("harry")
                },
                new User {
                    UserId = 2, Name = "Mélusine Enfayite", Mail = "melusine@test.com",
                    Password = SecretHasher.Hash("melusine")
                },
                new User {
                    UserId = 3, Name = "John Deuf", Mail = "john@test.com", Password = SecretHasher.Hash("john")
                },
                new User {
                    UserId = 4, Name = "Alain Terrieur", Mail = "alain@test.com", Password = SecretHasher.Hash("alain")
                },
                new User {
                    UserId = 5, Name = "Camille Honnête", Mail = "camille@test.com",
                    Password = SecretHasher.Hash("camille")
                },
                new User {
                    UserId = 6, Name = "Jim Nastik", Mail = "jim@test.com", Password = SecretHasher.Hash("jim")
                },
                new User {
                    UserId = 7, Name = "Mehdi Cament", Mail = "mehdi@test.com", Password = SecretHasher.Hash("mehdi")
                },
                new User { UserId = 8,Name = "Ali Gator", Mail = "ali@test.com", Password = SecretHasher.Hash("ali") }
            );

        modelBuilder.Entity<Admin>()
            .HasData(
                new Admin() { UserId = 9, Name = "Admin", Mail = "admin@test.com", Password = SecretHasher.Hash("admin") }
            );

        modelBuilder.Entity<Poll>()
            .HasData(
                new Poll { PollId = 1, Title = "Meilleure citation ?", CreatorId = 1 },
                new Poll { PollId = 2, Title = "Meilleur film de série B ?", CreatorId = 3 },
                new Poll { PollId = 3, Title = "Plus belle ville du monde ?", CreatorId = 1, Type = PollType.Single },
                new Poll { PollId = 4, Title = "Meilleur animé japonais ?", CreatorId = 5 },
                new Poll { PollId = 5, Title = "Sport pratiqué", CreatorId = 3, IsClosed = true },
                new Poll { PollId = 6, Title = "Langage informatique préféré", CreatorId = 7 }
            );

        modelBuilder.Entity<Comment>()
            .HasData(
                new Comment {
                    CommentId = 1, UserId = 1, PollId = 1, Text = "M'en fout",
                    CreationDate = DateTime.Parse("2022-12-10 16:40")
                },
                new Comment {
                    CommentId = 2, UserId = 1, PollId = 2, Text = "Bonne question!",
                    CreationDate = DateTime.Parse("2022-12-01 12:33")
                },
                new Comment {
                    CommentId = 3, UserId = 2, PollId = 1, Text = "Moi aussi",
                    CreationDate = DateTime.Parse("2022-12-11 22:07")
                },
                new Comment {
                    CommentId = 4, UserId = 3, PollId = 1, Text = "Bla bla bla",
                    CreationDate = DateTime.Parse("2022-12-27 08:15")
                },
                new Comment {
                    CommentId = 5, UserId = 1, PollId = 6, Text = "I love C#",
                    CreationDate = DateTime.Parse("2022-12-31 23:59")
                },
                new Comment {
                    CommentId = 6, UserId = 3, PollId = 6, Text = "I hate WPF",
                    CreationDate = DateTime.Parse("2023-01-01 00:01")
                },
                new Comment {
                    CommentId = 7, UserId = 2, PollId = 1,
                    Text =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi pulvinar, dolor non commodo commodo, " +
                        "felis libero sagittis tellus, at tristique orci risus hendrerit lorem. Maecenas varius hendrerit lacinia. " +
                        "Vestibulum dapibus, libero nec accumsan pulvinar, felis velit imperdiet libero, sed venenatis massa risus " +
                        "gravida dolor. In et lobortis massa.",
                    CreationDate = DateTime.Parse("2023-01-02 08:45")
                }
            );

        modelBuilder.Entity<Choice>()
            .HasData(
                new Choice {
                    ChoiceId = 1, PollId = 1,
                    Label =
                        "La science est ce que nous comprenons suffisamment bien pour l'expliquer à un ordinateur. L'art, c'est tout ce que nous faisons d'autre. (Knuth)"
                },
                new Choice {
                    ChoiceId = 2, PollId = 1,
                    Label =
                        "La question de savoir si les machines peuvent penser... est à peu près aussi pertinente que celle de savoir si les sous-marins peuvent nager. (Dijkstra)"
                },
                new Choice {
                    ChoiceId = 3, PollId = 1,
                    Label =
                        "Nous ne savons pas où nous allons, mais du moins il nous reste bien des choses à faire. (Turing)"
                },
                new Choice {
                    ChoiceId = 4, PollId = 1, Label = "La constante d’une personne est la variable d’une autre. (Perlis)"
                },
                new Choice {
                    ChoiceId = 5, PollId = 1,
                    Label =
                        "There are only two kinds of [programming] languages: the ones people complain about and the ones nobody uses. (Stroustrup)"
                },
                new Choice { ChoiceId = 6, PollId = 2, Label = "Massacre à la tronçonneuse" },
                new Choice { ChoiceId = 7, PollId = 2, Label = "Braindead" },
                new Choice { ChoiceId = 8, PollId = 2, Label = "La Nuit des morts-vivants" },
                new Choice { ChoiceId = 9, PollId = 2, Label = "Psychose" },
                new Choice { ChoiceId = 10, PollId = 2, Label = "Evil Dead" },
                new Choice { ChoiceId = 11, PollId = 3, Label = "Charleroi" },
                new Choice { ChoiceId = 12, PollId = 3, Label = "Charleville-Mézières" },
                new Choice { ChoiceId = 13, PollId = 3, Label = "Pyongyang" },
                new Choice { ChoiceId = 14, PollId = 3, Label = "Détroit" },
                new Choice { ChoiceId = 15, PollId = 4, Label = "One piece" },
                new Choice { ChoiceId = 16, PollId = 4, Label = "Hunter x Hunter" },
                new Choice { ChoiceId = 17, PollId = 4, Label = "Fullmetal Alchemist" },
                new Choice { ChoiceId = 18, PollId = 4, Label = "Death Note" },
                new Choice { ChoiceId = 19, PollId = 4, Label = "Naruto Shippûden" },
                new Choice { ChoiceId = 20, PollId = 4, Label = "Dragon Ball Z" },
                new Choice { ChoiceId = 21, PollId = 5, Label = "Curling" },
                new Choice { ChoiceId = 22, PollId = 5, Label = "Swamp Football" },
                new Choice { ChoiceId = 23, PollId = 5, Label = "Fléchettes" },
                new Choice { ChoiceId = 24, PollId = 5, Label = "Kin-ball" },
                new Choice { ChoiceId = 25, PollId = 5, Label = "Pétanque" },
                new Choice { ChoiceId = 26, PollId = 5, Label = "Lancer de tronc" },
                new Choice { ChoiceId = 27, PollId = 6, Label = "Brainfuck" },
                new Choice { ChoiceId = 28, PollId = 6, Label = "Java" },
                new Choice { ChoiceId = 29, PollId = 6, Label = "C#" },
                new Choice { ChoiceId = 30, PollId = 6, Label = "PHP" },
                new Choice { ChoiceId = 31, PollId = 6, Label = "Whitespace" },
                new Choice { ChoiceId = 32, PollId = 6, Label = "Aargh!" }
            );

        modelBuilder.Entity<Vote>()
            .HasData(
                new Vote { UserId = 1, ChoiceId = 1, Type = VoteType.Yes },
                new Vote { UserId = 1, ChoiceId = 2, Type = VoteType.Maybe },
                new Vote { UserId = 1, ChoiceId = 5, Type = VoteType.No },
                new Vote { UserId = 1, ChoiceId = 11, Type = VoteType.Yes },
                new Vote { UserId = 1, ChoiceId = 16, Type = VoteType.Yes },
                new Vote { UserId = 1, ChoiceId = 17, Type = VoteType.No },
                new Vote { UserId = 1, ChoiceId = 27, Type = VoteType.Yes },
                new Vote { UserId = 2, ChoiceId = 3, Type = VoteType.Yes },
                new Vote { UserId = 2, ChoiceId = 9, Type = VoteType.Maybe },
                new Vote { UserId = 2, ChoiceId = 10, Type = VoteType.Yes },
                new Vote { UserId = 2, ChoiceId = 16, Type = VoteType.Yes },
                new Vote { UserId = 2, ChoiceId = 29, Type = VoteType.Yes },
                new Vote { UserId = 3, ChoiceId = 2, Type = VoteType.Yes },
                new Vote { UserId = 3, ChoiceId = 4, Type = VoteType.Maybe },
                new Vote { UserId = 3, ChoiceId = 16, Type = VoteType.Maybe },
                new Vote { UserId = 3, ChoiceId = 20, Type = VoteType.Yes },
                new Vote { UserId = 3, ChoiceId = 32, Type = VoteType.No },
                new Vote { UserId = 4, ChoiceId = 29, Type = VoteType.Yes },
                new Vote { UserId = 5, ChoiceId = 27, Type = VoteType.Yes },
                new Vote { UserId = 5, ChoiceId = 28, Type = VoteType.No },
                new Vote { UserId = 6, ChoiceId = 27, Type = VoteType.Maybe },
                new Vote { UserId = 6, ChoiceId = 28, Type = VoteType.Yes },
                new Vote { UserId = 6, ChoiceId = 29, Type = VoteType.Maybe },
                new Vote { UserId = 7, ChoiceId = 27, Type = VoteType.Maybe },
                new Vote { UserId = 7, ChoiceId = 29, Type = VoteType.Yes },
                new Vote { UserId = 7, ChoiceId = 30, Type = VoteType.Maybe },
                new Vote { UserId = 8, ChoiceId = 27, Type = VoteType.Maybe },
                new Vote { UserId = 8, ChoiceId = 30, Type = VoteType.Yes },
                new Vote { UserId = 8, ChoiceId = 32, Type = VoteType.No }
            );

        modelBuilder.Entity<Participation>()
            .HasData(
                new Participation { PollId = 1, UserId = 1 },
                new Participation { PollId = 1, UserId = 2 },
                new Participation { PollId = 1, UserId = 3 },
                new Participation { PollId = 2, UserId = 2 },
                new Participation { PollId = 3, UserId = 1 },
                new Participation { PollId = 4, UserId = 1 },
                new Participation { PollId = 4, UserId = 2 },
                new Participation { PollId = 4, UserId = 3 },
                new Participation { PollId = 5, UserId = 1 },
                new Participation { PollId = 5, UserId = 2 },
                new Participation { PollId = 5, UserId = 3 },
                new Participation { PollId = 6, UserId = 1 },
                new Participation { PollId = 6, UserId = 2 },
                new Participation { PollId = 6, UserId = 3 },
                new Participation { PollId = 6, UserId = 4 },
                new Participation { PollId = 6, UserId = 5 },
                new Participation { PollId = 6, UserId = 6 },
                new Participation { PollId = 6, UserId = 7 },
                new Participation { PollId = 6, UserId = 8 }
            );

    }
}
