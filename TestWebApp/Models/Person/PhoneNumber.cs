using FluentValidation;
using TestWebApp.Domain.PersonManagement;

namespace TestWebApp.Models.Person
{
    public class PhoneNumber
    {
        public string Number { get; set; }

        public PhoneType Type { get; set; }
    }

    public class PhoneNumberValidator : AbstractValidator<PhoneNumber>
    {
        public PhoneNumberValidator()
        {
            RuleFor(x => x.Number).NotEmpty().MinimumLength(4).MaximumLength(50)
                .Matches(@"^[\d\-]{4,50}$");

            RuleFor(x => x.Type).NotEmpty();
        }
    }
}
