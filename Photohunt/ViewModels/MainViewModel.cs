using System.ComponentModel;
using Photohunt.Models;
using System.Collections.ObjectModel;
using Photohunt.Data;
using System.Collections.Generic;
using System;
using System.Threading;
using Microsoft.Phone.Shell;

namespace Photohunt.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _clock;
        private int _submittedPoints;

        public MainViewModel()
        {
            App.ContestService.PropertyChanged += new PropertyChangedEventHandler(ContestService_PropertyChanged);

            _clock = new Timer((o) =>
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => { NotifyPropertyChanged("TimeRemaining"); });
            }, null, 0, 15 * 1000);
        }

        public void UpdatePointCount()
        {
            if (!App.ContestService.Categories.ContainsKey(@"all"))
                return;

            _submittedPoints = 0;
            foreach (Clue clue in App.ContestService.Categories[@"all"])
            {
                foreach (Photo photo in App.ContestService.Photos)
                {
                    bool found = false;
                    if (!photo.Judge)
                        continue;

                    foreach (Clue photoClue in photo.Clues)
                    {
                        if (photoClue.Id == clue.Id)
                        {
                            _submittedPoints += clue.Points;
                            found = true;

                            foreach (Clue bonus in clue.Bonuses)
                            {
                                foreach (Clue photoBonus in photoClue.Bonuses)
                                {
                                    if (photoBonus.Id == bonus.Id)
                                    {
                                        _submittedPoints += bonus.Points;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (found)
                        break;
                }
            }
            NotifyPropertyChanged("SubmittedPoints");
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
                case "SyncComplete":
                    NotifyPropertyChanged("SyncInProgress");
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

        public bool SyncInProgress
        {
            get
            {
                return !App.ContestService.SyncComplete;
            }
        }

        public int SubmittedPoints
        {
            get
            {
                return _submittedPoints;
            }
        }

        #endregion
    }
}
