namespace TestWebApp.Domain.PersonManagement
{
    public class PersonPhone
    {
        public int Id { get; set; }

        public PhoneType Type { get; set; }

        public string Number { get; set; }

        public int PersonId { get; set; }
    }
}
