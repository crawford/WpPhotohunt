using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Photohunt.Models
{
    public class Photo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _notes;
        private bool _judge;
        private List<Clue> _clues;
        private Uri _path;

        public Photo()
        {
            _notes = "";
            _judge = false;
            _clues = new List<Clue>();
        }

        public Photo(Uri path) : this()
        {
            _path = path;
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #region Getters and Setters

        public string Notes
        {
            get
            {
                return _notes;
            }

            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    NotifyPropertyChanged("Notes");
                }
            }
        }

        public bool Judge
        {
            get
            {
                return _judge;
            }

            set
            {
                if (_judge != value)
                {
                    _judge = value;
                    NotifyPropertyChanged("Judge");
                }
            }
        }

        public List<Clue> Clues
        {
            get
            {
                return _clues;
            }

            set
            {
                if (_clues != value)
                {
                    _clues = value;
                    NotifyPropertyChanged("Clues");
                }
            }
        }

        public Uri Path
        {
            get
            {
                return _path;
            }
        }

        #endregion
    }
}
