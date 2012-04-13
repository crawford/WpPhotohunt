using System.Collections.Generic;
using Photohunt.Models;
using System;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace Photohunt.Data
{
    public class SettingsService
    {
        private const string KEY_AUTOSTART_CAMERA = "AutostartCamera";
        private const string KEY_GAME_KEY         = "GameKey";
        private const string KEY_ACTIVE_GAME      = "ActiveGame";

        private bool _autostartCamera;
        private string _gameKey;
        private bool _activeGame;

        private IsolatedStorageSettings _settings;

        public SettingsService() {
            _settings = IsolatedStorageSettings.ApplicationSettings;

            lock (App.IsolatedStorageSettingsLock)
            {
                if (!_settings.Contains(KEY_AUTOSTART_CAMERA)) _settings[KEY_AUTOSTART_CAMERA] = false;
                if (!_settings.Contains(KEY_GAME_KEY)) _settings[KEY_GAME_KEY] = "";
                if (!_settings.Contains(KEY_ACTIVE_GAME)) _settings[KEY_ACTIVE_GAME] = false;

                _autostartCamera = (bool)_settings[KEY_AUTOSTART_CAMERA];
                _gameKey = (string)_settings[KEY_GAME_KEY];
                _activeGame = (bool)_settings[KEY_ACTIVE_GAME];
            }
        }

        #region Getters and Setters

        public bool AutostartCamera
        {
            get
            {
                return _autostartCamera;
            }
            set
            {
                if (_autostartCamera != value)
                {
                    _autostartCamera = value;
                    lock (App.IsolatedStorageSettingsLock)
                    {
                        _settings[KEY_AUTOSTART_CAMERA] = value;
                        _settings.Save();
                    }
                }
            }
        }

        public string GameKey
        {
            get
            {
                return _gameKey;
            }
            set
            {
                if (_gameKey != value)
                {
                    _gameKey = value;
                    lock (App.IsolatedStorageSettingsLock)
                    {
                        _settings[KEY_GAME_KEY] = value;
                        _settings.Save();
                    }
                }
            }
        }

        public bool ActiveGame
        {
            get
            {
                return _activeGame;
            }
            set
            {
                if (_activeGame != value)
                {
                    _activeGame = value;
                    lock (App.IsolatedStorageSettingsLock)
                    {
                        _settings[KEY_ACTIVE_GAME] = value;
                        _settings.Save();
                    }
                }
            }
        }

        #endregion
    }
}
