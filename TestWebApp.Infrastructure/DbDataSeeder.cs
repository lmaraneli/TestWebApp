using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TestWebApp.Domain.CityManagement;
using TestWebApp.Domain.PersonManagement;

namespace TestWebApp.Infrastructure
{
    public class DbDataSeeder
    {
        public async Task SeedData(DbContext db)
        {
            var phoneFaker = new Faker<PersonPhone>()
                .RuleFor(x => x.Number, t => t.Random.String2(15, "0123456789"))
                .RuleFor(x => x.Type, t => t.PickRandom(new PhoneType[] { PhoneType.Home, PhoneType.Mobile, PhoneType.Office, PhoneType.Other }));

            var cityFaker = new Faker<City>()
                .RuleFor(x => x.Name, t => t.Random.String2(10));

            var personFaker = new Faker<Domain.PersonManagement.Person>()
                .RuleFor(x => x.Name, t => t.Random.String2(15))
                .RuleFor(x => x.LastName, t => t.Random.String2(20))
                .RuleFor(x => x.PersonalId, t => t.Random.String2(11, "0123456789"))
                .RuleFor(x => x.Sex, t => t.PickRandom(new Sex[] { Sex.Female, Sex.Male }))
                .RuleFor(x => x.DOB, t => new DateTime(2020, t.Random.Int(1, 12), t.Random.Int(1, 28)))
                .RuleFor(x => x.PhoneNumbers, t => phoneFaker.Generate(t.Random.Int(1, 5)))
                .RuleFor(x => x.CityId, t => t.Random.Int(1, 10));

            db.Set<City>().AddRange(cityFaker.Generate(10));

            db.Set<Domain.PersonManagement.Person>().AddRange(personFaker.Generate(100));

            await db.SaveChangesAsync();
        }
    }
}
