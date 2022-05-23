using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using TestWebApp.Domain.PersonManagement;
using TestWebApp.Resources;

namespace TestWebApp.Models.Person
{
    public class PersonModel
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public Sex Sex { get; set; }

        public string PersonalId { get; set; }

        public DateTime DOB { get; set; }

        public int CityId { get; set; }

        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
    }

    public class PersonModelValidator : AbstractValidator<PersonModel>
    {
        public PersonModelValidator(IStringLocalizer<Translations> localizer)
        {
            RuleFor(x => x.Name)
                .MinimumLength(2)
                .MaximumLength(50)
                .NotEmpty()
                .Matches("^(?:([a-zA-Z][^ა-ჰ]{2,50})|([ა-ჰ][^a-zA-Z]{2,50}))$");

            RuleFor(x => x.LastName).MinimumLength(2).MaximumLength(50).NotEmpty()
                .Matches("^(?:([a-zA-Z][^ა-ჰ]{2,50})|([ა-ჰ][^a-zA-Z]{2,50}))$");

            RuleFor(x => x.PersonalId).NotEmpty().Length(11).Matches(@"^\d{11}$");

            RuleFor(x => x.DOB).NotEmpty().Must(x =>
            {
                var now = DateTime.Now;
                var ts = now.AddYears(18).Subtract(now);
                return now.Subtract(x) > ts;
            }).WithMessage(localizer["Person should be 18+"]);

            RuleFor(x => x.Sex).NotEmpty();

            RuleFor(x => x.CityId).NotEmpty();
        }
    }
}
