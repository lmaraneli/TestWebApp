using System;
using System.Collections.Generic;
using TestWebApp.Domain.CityManagement;

namespace TestWebApp.Domain.PersonManagement
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public Sex Sex { get; set; }

        public string PersonalId { get; set; }

        public DateTime DOB { get; set; }

        public string ImageUrl { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<PersonPhone> PhoneNumbers { get; set; }

        public virtual ICollection<PersonRelation> Relatives { get; set; }
    }
}
