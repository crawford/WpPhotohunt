using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Photohunt.Models
{
    [DataContract]
    public class ClueCategory
    {
        private string _name;
        private List<Clue> _clues;

        public ClueCategory(string name)
        {
            _name = name;
            _clues = new List<Clue>();
        }

        [DataMember]
        public List<Clue> Clues
        {
            get
            {
                return _clues;
            }
            set
            {
                _clues = value;
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
