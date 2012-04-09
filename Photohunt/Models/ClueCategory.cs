﻿using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Photohunt.Models
{
    [DataContract]
    public class ClueCategory
    {
        private string _name;
        private List<CheckableClue> _clues;

        public ClueCategory(string name)
        {
            _name = name;
            _clues = new List<CheckableClue>();
        }

        [DataMember]
        public List<CheckableClue> Clues
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
