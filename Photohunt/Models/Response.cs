using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace Photohunt.Models
{
    [DataContract]
    public class Response
    {
        public enum STATUS
        {
            ERR_SUCCESS = 0,
            ERR_UNSPEC = 1,
            ERR_NOTAUTH = 2,
            ERR_GAMEOVER = 3,
            ERR_INTERNAL
        };

        public Response(STATUS status, string message)
        {
            Status = status;
            Message = message;
        }

        [DataMember(Name = "code", IsRequired = true)]
        public STATUS Status { get; set; }

        [DataMember(Name = "message", IsRequired = true)]
        public string Message { get; set; }
    }
}