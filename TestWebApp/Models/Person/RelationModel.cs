using TestWebApp.Domain.PersonManagement;

namespace TestWebApp.Models.Person
{
    public class RelationModel
    {
        public int RelativeId { get; set; }

        public RelationType Relation { get; set; }
    }
}
