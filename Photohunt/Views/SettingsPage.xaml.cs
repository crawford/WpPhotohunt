

using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls;
using Photohunt.Models;
using System.Net;
using Microsoft.Phone.Net.NetworkInformation;
using System.Collections.Generic;

namespace Photohunt
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            if (DataContext == null)
                DataContext = App.SettingsService;

            BtnEndGame.IsEnabled = App.SettingsService.ActiveGame;
            BtnNewGame.IsEnabled = !App.SettingsService.ActiveGame;
            TxtPassword.IsEnabled = !App.SettingsService.ActiveGame;
            if (App.SettingsService.ActiveGame)
                TxtPassword.Password = App.SettingsService.GameKey;
        }

        private void BtnNewGame_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to start a new game? All local photos will be erased.", "New Game", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.SettingsService.GameKey = TxtPassword.Password;
                App.ApiService.FetchTeamInfo(ApiService_FetchTeamInfoCompleted);
                App.ApiService.FetchClues(ApiService_FetchCluesCompleted);
            }
        }

        void ApiService_FetchTeamInfoCompleted(bool success, string errorString, TeamInfo info)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (success)
                {
                    App.ContestService.MaxPhotoCount = info.MaxPhotos;
                    App.ContestService.MaxJudgedPhotoCount = info.MaxJudgeablePhotos;
                    App.ContestService.TeamName = info.Name.ToLower();
                    App.ContestService.StartTime = info.StartTime;
                    App.ContestService.EndTime = info.EndTime;

                    BtnEndGame.IsEnabled = true;
                    BtnNewGame.IsEnabled = false;
                    TxtPassword.IsEnabled = false;
                    App.ContestService.NewGame();
                }
                else
                {
                    MessageBox.Show(errorString);
                }
            });
        }

        void ApiService_FetchCluesCompleted(bool success, string errorString, Clue[] clues)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (success)
                {
                    Dictionary<string, List<Clue>> categories = new Dictionary<string, List<Clue>>();
                    categories["all"] = new List<Clue>();
                    foreach (Clue clue in clues)
                    {
                        foreach (string tag in clue.Tags)
                        {
                            if (!categories.ContainsKey(tag))
                                categories[tag] = new List<Clue>();

                            categories[tag].Add(clue);
                        }

                        categories["all"].Add(clue);
                    }

                    ClueCategory[] cats = new ClueCategory[categories.Keys.Count];
                    int i = 0;
                    foreach (string cat in categories.Keys)
                    {
                        cats[i] = new ClueCategory(cat.ToLower(), categories[cat].ToArray());
                        i++;
                    }
                    App.ContestService.Categories = cats;
                }
                else
                {
                    MessageBox.Show(errorString);
                }
            });
        }

        private void BtnEndGame_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to end your game? You will not be able to make any further changes.", "End Game", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.ContestService.EndGame();
                BtnEndGame.IsEnabled = false;
                BtnNewGame.IsEnabled = true;
                TxtPassword.IsEnabled = true;
                TxtPassword.Password = "";
            }
        }
    }
}
