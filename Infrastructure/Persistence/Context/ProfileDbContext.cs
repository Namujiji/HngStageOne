using HngStageOne.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HngStageOne.Infrastructure.Persistence.Context;

public class ProfileDbContext(DbContextOptions<ProfileDbContext> options) : DbContext(options)
{
    public DbSet<Profile> Profiles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

            entity.Property(e => e.Gender)
            .HasMaxLength(15)
            .IsRequired();

            entity.Property(e => e.AgeGroup)
            .HasMaxLength(15)
            .IsRequired();

            entity.Property(e => e.CountryId)
            .HasMaxLength(15)
            .IsRequired();
        });
    }
}