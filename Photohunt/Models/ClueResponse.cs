using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Photohunt.Models
{
    [DataContract]
    public class ClueResponse : Response
    {
        public ClueResponse(STATUS status, string message, Clue[] clues)
            : base(status, message)
        {
            Clues = clues;
        }

        [DataMember(Name = "data", IsRequired=true)]
        public Clue[] Clues { get; set; }
    }
}
