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
            _categories = new List<ClueCategory>(App.ContestService.Categories);
            App.ContestService.PropertyChanged += new PropertyChangedEventHandler(ContestService_PropertyChanged);
        }

        private void ContestService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Categories":
                    _categories = new List<ClueCategory>(App.ContestService.Categories);
                    NotifyPropertyChanged("Categories");
                    break;
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
