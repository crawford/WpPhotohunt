using System.Runtime.Serialization;

namespace Photohunt.Models
{
    [DataContract]
    public class Clue
    {
        [DataMember(Name = "id", IsRequired = true)]
        public int Id { get; set; }

        [DataMember(Name = "description", IsRequired = true)]
        public string Description { get; set; }

        [DataMember(Name = "points", IsRequired = true)]
        public int Points { get; set; }

        [DataMember(Name = "bonuses", IsRequired = false)]
        public Clue[] Bonuses { get; set; }

        [DataMember(Name = "tags", IsRequired = false)]
        public string[] Tags { get; set; }

        public bool IsBonus { get; set; }
    }
}
