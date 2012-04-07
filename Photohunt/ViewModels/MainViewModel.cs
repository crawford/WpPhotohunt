using System.ComponentModel;
using Photohunt.Models;
using System.Collections.ObjectModel;
using Photohunt.Data;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Photohunt.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _clock;

        public MainViewModel()
        {
            App.ContestService.PropertyChanged += new PropertyChangedEventHandler(ContestService_PropertyChanged);

            _clock = new Timer((o) =>
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => { NotifyPropertyChanged("TimeRemaining"); });
            }, null, 0, 15 * 1000);
        }

        private void ContestService_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                case "TeamName":
                    NotifyPropertyChanged("TeamName");
                    break;
                case "StartTime":
                case "EndTime":
                    NotifyPropertyChanged("TimeRemaining");
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

        public ObservableCollection<Photo> Photos
        {
            get
            {
                return App.ContestService.Photos;
            }
        }

        public string TeamName
        {
            get
            {
                return App.ContestService.TeamName;
            }
        }

        public string TimeRemaining
        {
            get
            {
                TimeSpan span = App.ContestService.EndTime - DateTime.Now;
                if ((int)span.TotalMinutes < 0)
                    return string.Format("-{0}:{1:00}", (int)-span.TotalHours, -span.Minutes);
                else
                    return string.Format("{0}:{1:00}", (int)span.TotalHours, span.Minutes);
            }
        }

        #endregion
    }
}
