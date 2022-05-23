namespace TestWebApp.Domain.PersonManagement
{
    public class PersonRelation
    {
        public int PersonId { get; set; }

        public virtual Person Person { get; set; }

        public int RelativeId { get; set; }

        public virtual Person Relative { get; set; }

        public RelationType Relation { get; set; }
    }
}
