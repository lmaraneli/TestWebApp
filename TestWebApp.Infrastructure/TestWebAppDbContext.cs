using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TestWebApp.Domain.CityManagement;
using TestWebApp.Domain.PersonManagement;
using TestWebApp.Infrastructure.Configuration;

namespace TestWebApp.Infrastructure
{
    public class TestWebAppDbContext : DbContext
    {
        public TestWebAppDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        protected TestWebAppDbContext()
        {
        }

        public DbSet<Person> Persons { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<PersonPhone> PersonPhones { get; set; }

        public DbSet<PersonRelation> PersonRelations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration<City>(new CityTypeConfiguration());
            modelBuilder.ApplyConfiguration<Person>(new PersonTypeConfiguration());
            modelBuilder.ApplyConfiguration<PersonRelation>(new PersonRelationTypeConfiguration());

            modelBuilder.Entity<Person>().OwnsMany<PersonPhone>(x => x.PhoneNumbers);
        }
    }
}
