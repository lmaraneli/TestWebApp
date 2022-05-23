using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestWebApp.Domain.PersonManagement;

namespace TestWebApp.Infrastructure.Configuration
{
    public class PersonRelationTypeConfiguration : IEntityTypeConfiguration<PersonRelation>
    {
        public void Configure(EntityTypeBuilder<PersonRelation> builder)
        {
            builder.HasKey(x => new { x.PersonId, x.RelativeId });

            builder.HasOne(x => x.Person).WithMany(p => p.Relatives).HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Relative).WithMany().HasForeignKey(x => x.RelativeId);

            builder.Property(x => x.Relation).IsRequired();
        }
    }
}
