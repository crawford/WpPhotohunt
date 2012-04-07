using System.ComponentModel;
using Photohunt.Models;
using System.Collections.Generic;

namespace Photohunt.ViewModels
{
    public class CluesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private List<ClueCategory> _categories;

        public CluesViewModel()
        {
            BuildCategories();
            App.ContestService.PropertyChanged += new PropertyChangedEventHandler(ContestService_PropertyChanged);
        }

        private void ContestService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Categories":
                    BuildCategories();
                    NotifyPropertyChanged("Categories");
                    break;
            }
        }

        private void BuildCategories()
        {
            _categories = new List<ClueCategory>(App.ContestService.Categories.Keys.Count);
            foreach (string tag in App.ContestService.Categories.Keys)
            {
                ClueCategory category = new ClueCategory(tag);
                List<Clue> clues = App.ContestService.Categories[tag];

                foreach (Clue clue in clues)
                {
                    category.Clues.Add(clue);
                    foreach (Clue bonus in clue.Bonuses)
                    {
                        bonus.IsBonus = true;
                        category.Clues.Add(bonus);
                    }
                }

                _categories.Add(category);
            }
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public List<ClueCategory> Categories
        {
            get
            {
                return _categories;
            }
        }
    }
}
