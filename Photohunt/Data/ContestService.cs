using System.Collections.ObjectModel;
using Photohunt.Models;
using System.ComponentModel;
using System;
using System.IO.IsolatedStorage;

namespace Photohunt.Data
{
    public class ContestService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Photo> _photos;
        private int _maxPhotoCount;
        private int _judgeCount;
        private int _maxJudgeCount;

        public ContestService()
        {
            _photos = new ObservableCollection<Photo>();
            _photos.CollectionChanged += (o, r) =>
            {
                NotifyPropertyChanged("PhotoCount");
            };

            _maxPhotoCount = 0;
            _judgeCount = 0;
            _maxJudgeCount = 0;
        }

        public Photo CreatePhoto(Uri path)
        {
            Photo photo = new Photo(path);
            photo.PropertyChanged += new PropertyChangedEventHandler(Photo_PropertyChanged);
            _photos.Add(photo);
            return photo;
        }

        public void Photo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Judge")
            {
                Photo photo = (Photo)sender;
                if (photo.Judge)
                    _judgeCount++;
                else
                    _judgeCount--;

                NotifyPropertyChanged("JudgedPhotoCount");
            }
        }

        public void Save() {
            IsolatedStorageSettings.ApplicationSettings["o"] = _photos;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public void Load()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("o"))
                _photos = (ObservableCollection<Photo>)IsolatedStorageSettings.ApplicationSettings["o"];

            _photos.CollectionChanged += (o, r) =>
            {
                NotifyPropertyChanged("PhotoCount");
            };

            foreach (Photo p in _photos)
                if (p.Judge)
                    _judgeCount++;
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #region Getters and Setters

        public int PhotoCount
        {
            get
            {
                return _photos.Count;
            }
        }

        public int MaxPhotoCount
        {
            get
            {
                return _maxPhotoCount;
            }
        }

        public int JudgedPhotoCount
        {
            get
            {
                return _judgeCount;
            }
        }

        public int MaxJudgedPhotoCount
        {
            get
            {
                return _maxJudgeCount;
            }
        }

        public ObservableCollection<Photo> Photos
        {
            get
            {
                return _photos;
            }
        }

        #endregion
    }
}
