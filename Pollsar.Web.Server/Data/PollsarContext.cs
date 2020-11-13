using Microsoft.AspNetCore.Identity;
using Pollsar.Web.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Security.Cryptography.X509Certificates;
using Bogus;
using System.Linq;

namespace Pollsar.Web.Data
{
    public partial class PollsarContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public PollsarContext (DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating (ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Category
            builder.Entity<Category>(b =>
            {
                b.HasKey(c => c.Id);
                b.ToTable("categories");

                b.Property(c => c.CategoryName).IsRequired();

                b.Property(c => c.DateCreated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();

                b.Property(c => c.LastUpdated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAddOrUpdate();

                b.HasMany(c => c.Polls)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.ClientSetNull);
            });
            #endregion

            #region Poll
            builder.Entity<Poll>(b =>
            {
                b.HasKey(prop => prop.Id);
                b.ToTable("polls");
                b.Property(p => p.Title).IsRequired();
                b.Property(p => p.Description).HasMaxLength(255);

                b.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey(s => s.RefererId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(p => p.Creator)
                .WithMany(u => u.PollsCreated)
                .HasForeignKey(prop => prop.CreatorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

                b.Property(p => p.DateCreated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();


                b.Property(p => p.LastUpdated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAddOrUpdate();

                b.HasMany(p => p.Choices)
                .WithOne(c => c.Poll)
                .HasForeignKey(c => c.PollId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(p => p.Tags)
                .WithOne(t => t.Poll)
                .HasForeignKey(t => t.PollId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

                b.HasMany(p => p.Categories)
                .WithOne(c => c.Poll)
                .HasForeignKey(c => c.PollId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region PollCategory
            builder.Entity<PollCategory>(b =>
            {
                b.HasKey(prop => prop.Id);
                b.ToTable("polls_categories");

                b.HasOne(p => p.Poll)
                .WithMany(p => p.Categories)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasForeignKey(prop => prop.PollId);

                b.HasOne(p => p.Category)
                .WithMany(collection => collection.Polls)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasForeignKey(prop => prop.CategoryId);

                b.Property(p => p.DateAdded)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();

                b.Property(p => p.LastUpdated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAddOrUpdate();
            });
            #endregion

            #region PollChoice
            builder.Entity<PollChoice>(b =>
            {
                b.HasKey(prop => prop.Id);
                b.ToTable("polls_choices");

                b.Property(p => p.Name).IsRequired();

                b.HasOne(p => p.Poll)
                .WithMany(prop => prop.Choices)
                .HasForeignKey(prop => prop.PollId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

                b.HasMany(p => p.Votes)
                .WithOne()
                .HasForeignKey(p => p.PollChoiceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

                b.Property(p => p.LastUpdated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAddOrUpdate();
            });
            #endregion

            #region PollTag
            builder.Entity<PollTag>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasOne(t => t.Poll)
                .WithMany(prop => prop.Tags)
                .HasForeignKey(testc => testc.PollId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

                b.HasOne(t => t.Tag)
                .WithMany(testc => testc.Polls)
                .HasForeignKey(testc => testc.TagId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

                b.Property(t => t.DateAdded)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();
            });
            #endregion

            #region PollVote
            builder.Entity<PollVote>(b =>
            {
                b.HasKey(prop => prop.Id);
                b.ToTable("polls_votes");

                b.HasOne(p => p.Voter)
                .WithMany()
                .HasForeignKey(u => u.VoterId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

                b.HasOne(p => p.Choice)
                .WithMany(collection => collection.Votes)
                .HasForeignKey(prop => prop.PollChoiceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

                b.Property(p => p.VoteDate)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();
            });
            #endregion

            #region Tag
            builder.Entity<Tag>(b =>
            {
                b.HasKey(testc => testc.Id);
                b.ToTable("tags");

                b.Property(t => t.DateCreated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();

                b.Property(t => t.LastUpdated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAddOrUpdate();

                b.HasMany(t => t.Polls)
                .WithOne(prop => prop.Tag)
                .HasForeignKey(prop => prop.TagId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region User
            builder.Entity<User>(b =>
            {
                b.Property(u => u.Avatar).HasMaxLength(1000);
                b.Property(u => u.DateAdded)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();

                b.Property(u => u.LastUpdated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAddOrUpdate();

                b.HasMany(u => u.PollsCreated)
                .WithOne(prop => prop.Creator)
                .HasForeignKey(p => p.CreatorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region StaticResource
            builder.Entity<StaticResource>(b => {
                b.HasKey(s => s.Id);
                b.ToTable("static_resources");

                b.Property(s => s.Url).IsRequired();

                b.Property(s => s.DateCreated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAdd();

                b.Property(s => s.LastUpdated)
                .HasValueGenerator<DateTimeGenerator>()
                .ValueGeneratedOnAddOrUpdate();
            });
            #endregion


            GenerateSeedData(builder);
        }

        private void GenerateSeedData (ModelBuilder builder)
        {
            var adminId = 1L;
            var adminRoleId = 1L;
            var userRoleId = 2L;

            var admin = new User
            {
                PhoneNumber = null,
                DateAdded = DateTime.Now,
                Email = "philllittle302@gmail.com",
                Id = adminId,
                NormalizedEmail = "PHILLLITTLE302@GMAIL.COM",
                UserName = "philllittle302@gmail.com",
                NormalizedUserName = "PHILLLITTLE302@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumberConfirmed = true,
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            var passwordHasher = new PasswordHasher<User>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "deadpool672");

            var adminRole = new IdentityRole<long>() { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" };
            var adminUserRole = new IdentityUserRole<long> { RoleId = adminRoleId, UserId = adminId };

            builder.Entity<User>().HasData(admin);
            builder.Entity<IdentityRole<long>>().HasData(adminRole, new IdentityRole<long>() { Id = userRoleId, Name = "User", NormalizedName = "USER" });
            builder.Entity<IdentityUserRole<long>>().HasData(adminUserRole);



            var i = adminId;
            var usersFaker = new Faker<User>()
                .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                .RuleFor(u => u.Id, f => ++i)
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.UserName, (f, u) => u.Email)
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.ConcurrencyStamp, f => f.Random.Uuid().ToString())
                .RuleFor(u => u.SecurityStamp, f => f.Random.Uuid().ToString())
                .RuleFor(u => u.PasswordHash, (f, u) => passwordHasher.HashPassword(u, f.Internet.Password()));
            var random = new Random();
            var fakeUsers = usersFaker.Generate(300);

            var pollsFaker = new Faker<Poll>()
                .RuleFor(u => u.Title, f => f.Random.Words(30))
                .RuleFor(p => p.CreatorId, f => f.PickRandom(fakeUsers.Select(u => u.Id)))
                .RuleFor(p => p.Description, f => f.Lorem.Lines(5));

            void staticImagesFakerGenerator (Poll poll)
            {
                var imagesFaker = new Faker<StaticResource>()
                .RuleFor(s => s.RefererId, f => poll.Id)
                .RuleFor(s => s.Url, f => f.Image.PicsumUrl());

                builder.Entity<StaticResource>().HasData(imagesFaker.Generate(random.Next(1, 6)));
            }

            builder.Entity<User>().HasData(fakeUsers);
            var fakePolls = pollsFaker.Generate(1000);
            
            fakePolls.ForEach(staticImagesFakerGenerator);

            builder.Entity<Poll>().HasData(fakePolls);
            builder.Entity<IdentityUserRole<long>>().HasData(fakeUsers.Select(u => u.Id).Select(id => new IdentityUserRole<long>() { RoleId = userRoleId, UserId = id }));
        }
    }

    class DateTimeGenerator : ValueGenerator<DateTime>
    {
        public override bool GeneratesTemporaryValues => false;

        public override DateTime Next ([NotNull] EntityEntry entry) => DateTime.UtcNow;
    }
}