using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Photohunt.Models
{
    [DataContract]
    public class InfoResponse : Response
    {
        public InfoResponse(STATUS status, string message, TeamInfo info)
            : base(status, message)
        {
            Info = info;
        }

        [DataMember(Name = "data", IsRequired=true)]
        public TeamInfo Info { get; set; }
    }
}
