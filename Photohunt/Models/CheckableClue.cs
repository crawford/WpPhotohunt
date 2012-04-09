using System.ComponentModel;
using System.Collections.Generic;

namespace Photohunt.Models
{
    public class CheckableClue : Clue, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _checked;
        private bool _enabled;
        private bool _bonus;

        public CheckableClue(Clue clue)
        {
            Id = clue.Id;
            Description = clue.Description;
            Points = clue.Points;
            Tags = clue.Tags;
            IsBonus = false;
            IsEnabled = true;
            if (clue.Bonuses == null)
                Bonuses = new CheckableClue[0];
            else
                Bonuses = new CheckableClue[clue.Bonuses.Length];

            for (int i = 0; i < Bonuses.Length; i++)
                Bonuses[i] = new CheckableClue(clue.Bonuses[i]);

            _checked = false;
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public bool IsChecked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    NotifyPropertyChanged("IsChecked");

                    if (!IsBonus)
                    {
                        if (_checked)
                        {
                            foreach (CheckableClue bonus in Bonuses)
                                bonus.IsEnabled = true;
                        }
                        else
                        {
                            foreach (CheckableClue bonus in Bonuses)
                            {
                                bonus.IsChecked = false;
                                bonus.IsEnabled = false;
                            }
                        }
                    }
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    NotifyPropertyChanged("IsEnabled");
                }
            }
        }

        public bool IsBonus
        {
            get
            {
                return _bonus;
            }
            set
            {
                if (_bonus != value)
                {
                    _bonus = value;
                    NotifyPropertyChanged("IsBonus");
                }
            }
        }
    }
}
