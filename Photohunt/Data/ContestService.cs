using System.Collections.ObjectModel;
using Photohunt.Models;
using System.ComponentModel;
using System;

namespace Photohunt.Data
{
    public class ContestService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Photo> _photos;
        private Photo _currentPhoto;
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
            _currentPhoto = null;
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

        void Photo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Photo photo = (Photo)sender;
            if (photo.Judge)
                _judgeCount++;
            else
                _judgeCount--;

            NotifyPropertyChanged("JudgedPhotoCount");
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #region Getters and Setters

        public Photo CurrentPhoto
        {
            get
            {
                return _currentPhoto;
            }
            set
            {
                if (_currentPhoto != value)
                {
                    _currentPhoto = value;
                }
            }
        }

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

        #endregion
    }
}
