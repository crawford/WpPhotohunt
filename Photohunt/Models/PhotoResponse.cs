using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Photohunt.Models
{
    [DataContract]
    public class PhotoResponse : Response
    {
        public PhotoResponse(STATUS status, string message, string id)
            : base(status, message)
        {
            Id = id;
        }

        [DataMember(Name = "data", IsRequired=true)]
        public string Id { get; set; }
    }
}
