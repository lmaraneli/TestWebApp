using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestWebApp.Domain.PersonManagement;

namespace TestWebApp.Infrastructure.Configuration
{
    public class PersonPhoneTypeConfiguration : IEntityTypeConfiguration<PersonPhone>
    {
        public void Configure(EntityTypeBuilder<PersonPhone> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type).IsRequired();

            builder.Property(x => x.Number).IsRequired().HasMaxLength(50);
        }
    }
}
