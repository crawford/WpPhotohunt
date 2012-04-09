using System.ComponentModel;
using Photohunt.Models;
using System.Collections.Generic;

namespace Photohunt.ViewModels
{
    public class CluesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private List<ClueCategory> _categories;
        private int _categoryAllIndex;

        public CluesViewModel()
        {
            BuildCategories();
            App.ContestService.PropertyChanged += new PropertyChangedEventHandler(ContestService_PropertyChanged);
        }

        public void SelectClues(List<Clue> clues)
        {
            foreach (CheckableClue clue in _categories[_categoryAllIndex].Clues)
            {
                clue.IsChecked = false;
                foreach (Clue sel in clues) {
                    if (!clue.IsBonus && clue.Id == sel.Id)
                    {
                        clue.IsChecked = true;
                        break;
                    }
                    foreach (Clue selB in sel.Bonuses)
                    {
                        if (clue.IsBonus && clue.Id == selB.Id)
                        {
                            clue.IsChecked = true;
                            break;
                        }
                    }
                }
            }
        }

        public List<Clue> GetClues()
        {
            List<Clue> selected = new List<Clue>();

            foreach (CheckableClue clue in _categories[_categoryAllIndex].Clues)
            {
                if (clue.IsChecked && !clue.IsBonus)
                {
                    Clue newClue = clue.Clone();
                    List<Clue> bonuses = new List<Clue>();

                    foreach (CheckableClue bonus in clue.Bonuses)
                    {
                        if (bonus.IsChecked)
                            bonuses.Add(bonus.Clone());
                    }

                    newClue.Bonuses = bonuses.ToArray();
                    selected.Add(newClue);
                }
            }
            return selected;
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
            Dictionary<int, CheckableClue> clueMap = new Dictionary<int, CheckableClue>();
            foreach (string tag in App.ContestService.Categories.Keys)
            {
                if (tag == @"all") _categoryAllIndex = _categories.Count;

                ClueCategory category = new ClueCategory(tag);
                List<Clue> clues = App.ContestService.Categories[tag];

                foreach (Clue clue in clues)
                {
                    if (!clueMap.ContainsKey(clue.Id))
                        clueMap[clue.Id] = new CheckableClue(clue);

                    category.Clues.Add(clueMap[clue.Id]);
                    foreach (CheckableClue bonus in clueMap[clue.Id].Bonuses)
                    {
                        bonus.IsBonus = true;
                        bonus.IsEnabled = false;
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
