using System.Runtime.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Photohunt.Models
{
    [DataContract]
    public class TeamInfo
    {
        [DataMember(Name = "team", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "startTime", IsRequired = true)]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "endTime", IsRequired = true)]
        public DateTime EndTime { get; set; }

        [DataMember(Name = "maxPhotos", IsRequired = true)]
        public int MaxPhotos { get; set; }

        [DataMember(Name = "maxJudgedPhotos", IsRequired = true)]
        public int MaxJudgeablePhotos { get; set; }
    }
}
