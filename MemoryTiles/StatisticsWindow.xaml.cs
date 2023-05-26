using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MemoryTiles
{
    public partial class StatisticsWindow : Window
    {
        private string _username;

        public StatisticsWindow(string username)
        {
            InitializeComponent();
            _username = username;
            UserName.Text = _username;
            UserProfileImage.Source = GetProfileImageSource(_username);
            LoadStatistics();
        }

        private ImageSource GetProfileImageSource(string username)
        {
            string usersFilePath = @"..\..\Users.txt";
            string[] lines = File.ReadAllLines(usersFilePath);

            foreach (string line in lines)
            {
                string[] userData = line.Split(',');

                if (userData[0] == username)
                {
                    string imagePath = userData[1];
                    ImageSource profileImage = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                    return profileImage;
                }
            }

            return null;
        }

        private void LoadStatistics()
        {
            string statisticsFilePath = $@"..\..\Statistics\{_username}.txt";

            if (!File.Exists(statisticsFilePath))
            {
                GamesPlayed.Text = "0";
                GamesWon.Text = "0";
                return;
            }

            string[] lines = File.ReadAllLines(statisticsFilePath);

            if (lines.Length == 0)
            {
                GamesPlayed.Text = "0";
                GamesWon.Text = "0";
                return;
            }

            string[] statistics = lines[0].Split(',');

            if (statistics.Length == 3)
            {
                GamesPlayed.Text = statistics[1];
                GamesWon.Text = statistics[2];
            }
            else
            {
                GamesPlayed.Text = "0";
                GamesWon.Text = "0";
            }
        }
    }
}
