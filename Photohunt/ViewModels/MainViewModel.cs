using System.ComponentModel;
using Photohunt.Models;
using System.Collections.ObjectModel;
using Photohunt.Data;
using System.Collections.Generic;

namespace Photohunt.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            App.ContestService.PropertyChanged += new PropertyChangedEventHandler(ContestService_PropertyChanged);
        }

        void ContestService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "PhotoCount":
                case "MaxPhotoCount":
                    NotifyPropertyChanged("PhotosTakenString");
                    break;
                case "JudgedPhotoCount":
                case "MaxJudgedPhotoCount":
                    NotifyPropertyChanged("PhotosJudgedString");
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

        #region Getters and Setters

        public string PhotosTakenString
        {
            get
            {
                return string.Format("{0} / {1}", App.ContestService.PhotoCount, App.ContestService.MaxPhotoCount);
            }
        }

        public string PhotosJudgedString
        {
            get
            {
                return string.Format("{0} / {1}", App.ContestService.JudgedPhotoCount, App.ContestService.MaxJudgedPhotoCount);
            }
        }

        #endregion
    }
}
