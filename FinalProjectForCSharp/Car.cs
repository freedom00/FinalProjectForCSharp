using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectForCSharp
{
    [Table("Cars")]
    public class Car
    {
        public int Id { get; set; }
        public string MakeModel { get; set; }
        public int OwnerId { get; set; }

        public Owner CurrentOwner { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Id, MakeModel);
        }
    }
}