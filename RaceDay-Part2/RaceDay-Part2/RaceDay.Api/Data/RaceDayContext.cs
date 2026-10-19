using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayContext(DbContextOptions<RaceDayContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<EventCategory>().HasIndex(x => new { x.EventId, x.CategoryId }).IsUnique();
        modelBuilder.Entity<Enrolment>().HasIndex(x => new { x.ParticipantId, x.EventCategoryId }).IsUnique();
        modelBuilder.Entity<Result>().HasIndex(x => x.EnrolmentId).IsUnique();

        modelBuilder.Entity<User>().HasMany(x => x.OrganisedEvents).WithOne(x => x.Organiser)
            .HasForeignKey(x => x.OrganiserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EventCategory>().HasOne(x => x.Event).WithMany(x => x.EventCategories)
            .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EventCategory>().HasOne(x => x.Category).WithMany(x => x.EventCategories)
            .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasOne(x => x.Participant).WithMany(x => x.Enrolments)
            .HasForeignKey(x => x.ParticipantId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasOne(x => x.EventCategory).WithMany(x => x.Enrolments)
            .HasForeignKey(x => x.EventCategoryId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Result>().HasOne(x => x.Enrolment).WithOne(x => x.Result)
            .HasForeignKey<Result>(x => x.EnrolmentId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>().Property(x => x.DistanceKm).HasPrecision(8, 2);
        modelBuilder.Entity<EventCategory>().Property(x => x.EntryFee).HasPrecision(10, 2);
    }
}
