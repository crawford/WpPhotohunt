using System.Collections.ObjectModel;

namespace Photohunt.Models
{
    public class ClueCategory
    {
        private string _name;
        private ObservableCollection<Clue> _clues;

        public ClueCategory(string name)
        {
            _name = name;
            _clues = new ObservableCollection<Clue>();
        }
    }
}
