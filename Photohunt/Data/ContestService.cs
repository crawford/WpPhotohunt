using System.Collections.ObjectModel;
using Photohunt.Models;
using System.ComponentModel;
using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Threading;

namespace Photohunt.Data
{
    public class ContestService : INotifyPropertyChanged
    {
        private const int RETRY_BACKOFF = 10 * 1000;

        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Photo> _photos;
        private int _maxPhotoCount;
        private int _judgeCount;
        private int _maxJudgeCount;
        private string _teamName;
        private DateTime _startTime;
        private DateTime _endTime;
        private IsolatedStorageSettings _settings;
        private Dictionary<string, List<Clue>> _clues;
        private Thread _sync;
        private bool _syncComplete;

        private const string KEY_PHOTOS               = "photos";
        private const string KEY_MAX_PHOTOS           = "MaxPhotos";
        private const string KEY_MAX_JUDGEABLE_PHOTOS = "MaxJudgeablePhotos";
        private const string KEY_TEAM_NAME            = "TeamName";
        private const string KEY_START_TIME           = "StartTime";
        private const string KEY_END_TIME             = "EndTime";
        private const string KEY_CLUES                = "Clues";

        public ContestService()
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;
            _syncComplete = true;
        }

        public Photo CreatePhoto(Uri path)
        {
            Photo photo = new Photo(path);
            photo.PropertyChanged += new PropertyChangedEventHandler(Photo_PropertyChanged);
            _photos.Add(photo);
            return photo;
        }

        public void Load()
        {
            if (!_settings.Contains(KEY_MAX_PHOTOS))           _settings[KEY_MAX_PHOTOS]           = 0;
            if (!_settings.Contains(KEY_MAX_JUDGEABLE_PHOTOS)) _settings[KEY_MAX_JUDGEABLE_PHOTOS] = 0;
            if (!_settings.Contains(KEY_TEAM_NAME))            _settings[KEY_TEAM_NAME]            = "team";
            if (!_settings.Contains(KEY_START_TIME))           _settings[KEY_START_TIME]           = DateTime.Now;
            if (!_settings.Contains(KEY_END_TIME))             _settings[KEY_END_TIME]             = DateTime.Now;
            if (!_settings.Contains(KEY_PHOTOS))               _settings[KEY_PHOTOS]               = new ObservableCollection<Photo>();
            if (!_settings.Contains(KEY_CLUES))                _settings[KEY_CLUES]                = new Dictionary<string, List<Clue>>();

            MaxPhotoCount       = (int)                           _settings[KEY_MAX_PHOTOS];
            MaxJudgedPhotoCount = (int)                           _settings[KEY_MAX_JUDGEABLE_PHOTOS];
            TeamName            = (string)                        _settings[KEY_TEAM_NAME];
            StartTime           = (DateTime)                      _settings[KEY_START_TIME];
            EndTime             = (DateTime)                      _settings[KEY_END_TIME];
            _photos             = (ObservableCollection<Photo>)   _settings[KEY_PHOTOS];
            _clues              = (Dictionary<string, List<Clue>>)_settings[KEY_CLUES];

            NotifyPropertyChanged("Photos");

            _photos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Photos_CollectionChanged);

            _judgeCount = 0;
            foreach (Photo p in _photos)
            {
                p.PropertyChanged += new PropertyChangedEventHandler(Photo_PropertyChanged);
                if (p.Judge)
                    JudgedPhotoCount++;
            }
        }

        public void NewGame()
        {
            _photos.Clear();
            JudgedPhotoCount = 0;
            App.SettingsService.ActiveGame = true;
        }

        public void EndGame()
        {
            App.SettingsService.ActiveGame = false;
        }

        private void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _settings[KEY_PHOTOS] = _photos;
            _settings.Save();
            NotifyPropertyChanged("PhotoCount");
            SyncPhotos();
        }

        public void Photo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Judge")
            {
                Photo photo = (Photo)sender;
                if (photo.Judge)
                    JudgedPhotoCount++;
                else
                    JudgedPhotoCount--;
            }
            _settings[KEY_PHOTOS] = _photos;
            _settings.Save();
            SyncPhotos();
        }

        private void SyncPhotos()
        {
            SyncComplete = false;
            if (_sync == null)
            {
                _sync = new Thread(new ThreadStart(BackgroundSync));
                _sync.Start();
            }
        }

        private void BackgroundSync() {
            System.Diagnostics.Debug.WriteLine("Beginning sync");
            List<Photo> updating = new List<Photo>();

            while (true) {
                
                foreach (Photo photo in _photos)
                {
                    if (!photo.Dirty)
                        continue;

                    if (updating.Contains(photo))
                    {
                        System.Diagnostics.Debug.WriteLine("Waiting on " + photo.GetHashCode());
                        continue;
                    }

                    updating.Add(photo);

                    if (photo.ServerId == null)
                        App.ApiService.UploadPhoto(photo, updating, UploadPhotoCallback);
                    else
                        App.ApiService.SendPhotoMetadata(photo, updating, SendPhotoMetadataCallback);
                }
                if (updating.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Sync complete");
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        SyncComplete = true;
                    });
                    break;
                }

                System.Diagnostics.Debug.WriteLine("Sync sleeping");
                Thread.Sleep(RETRY_BACKOFF);
            }

            _sync = null;
        }

        private void SendPhotoMetadataCallback(bool success, Photo photo, List<Photo> updating)
        {
            updating.Remove(photo);
            //This should work...
            /*if (updating.Count == 0)
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    SyncComplete = true;
                });*/

            if (success)
            {
                photo.Dirty = false;
                System.Diagnostics.Debug.WriteLine("Done updating " + photo.GetHashCode());
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Done updating - Error " + photo.GetHashCode());
            }
        }

        private void UploadPhotoCallback(bool success, Photo photo, List<Photo> updating)
        {
            if (success)
            {
                App.ApiService.SendPhotoMetadata(photo, updating, SendPhotoMetadataCallback);
            }
            else
            {
                updating.Remove(photo);
                System.Diagnostics.Debug.WriteLine("Done updating - Error " + photo.GetHashCode());
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
            set
            {
                if (_maxPhotoCount != value)
                {
                    _maxPhotoCount = value;
                    _settings[KEY_MAX_PHOTOS] = value;
                    _settings.Save();
                    NotifyPropertyChanged("MaxPhotoCount");
                }
            }
        }

        public int JudgedPhotoCount
        {
            get
            {
                return _judgeCount;
            }
            private set
            {
                if (_judgeCount != value)
                {
                    _judgeCount = value;
                    NotifyPropertyChanged("JudgedPhotoCount");
                }
            }
        }

        public int MaxJudgedPhotoCount
        {
            get
            {
                return _maxJudgeCount;
            }
            set
            {
                if (_maxJudgeCount != value)
                {
                    _maxJudgeCount = value;
                    _settings[KEY_MAX_JUDGEABLE_PHOTOS] = value;
                    _settings.Save();
                    NotifyPropertyChanged("MaxJudgedPhotoCount");
                }
            }
        }

        public ObservableCollection<Photo> Photos
        {
            get
            {
                return _photos;
            }
        }

        public string TeamName
        {
            get
            {
                return _teamName;
            }
            set
            {
                if (_teamName != value)
                {
                    _teamName = value;
                    _settings[KEY_TEAM_NAME] = value;
                    _settings.Save();
                    NotifyPropertyChanged("TeamName");
                }
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    _settings[KEY_START_TIME] = value;
                    _settings.Save();
                    NotifyPropertyChanged("StartTime");
                }
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    _settings[KEY_END_TIME] = value;
                    _settings.Save();
                    NotifyPropertyChanged("EndTime");
                }
            }
        }

        public Dictionary<string, List<Clue>> Categories
        {
            get
            {
                return _clues;
            }
            set
            {
                if (_clues != value)
                {
                    _clues = value;
                    _settings[KEY_CLUES] = value;
                    _settings.Save();
                    NotifyPropertyChanged("Categories");
                }
            }
        }

        public bool SyncComplete
        {
            get
            {
                return _syncComplete;
            }
            set
            {
                if (_syncComplete != value)
                {
                    _syncComplete = value;
                    NotifyPropertyChanged("SyncComplete");
                }
            }
        }

        #endregion
    }
}
