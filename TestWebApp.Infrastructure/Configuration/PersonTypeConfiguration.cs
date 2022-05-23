using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestWebApp.Domain.PersonManagement;

namespace TestWebApp.Infrastructure.Configuration
{
    public class PersonTypeConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);

            builder.Property(x => x.Sex).IsRequired();

            builder.Property(x => x.PersonalId).IsRequired().HasMaxLength(11);

            builder.Property(x => x.ImageUrl).HasMaxLength(255);

            builder.HasOne(x => x.City).WithMany().HasForeignKey(x => x.CityId);
        }
    }
}
