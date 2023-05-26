using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryTiles
{
    public partial class PlayWindow : Window
    {
        private Button _previousButton;
        private int _hearts;
        private int _level;
        private int _matchesFound;
        private DispatcherTimer _timer;
        private TimeSpan _elapsedTime;
        private bool _firstClick;
        private int _consecutiveWins;
        private string _username;

        public PlayWindow(string username, int level, int hearts, int rows, int columns)
        {
            InitializeComponent();
            _username = username;
            _level = level;
            _hearts = 3;
            _firstClick = true;
            TileGrid.Rows = rows;
            TileGrid.Columns = columns;
            InitializeGame();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void InitializeGame()
        {
            // Set the user's name, profile picture, level, and hearts
            UserName.Text = _username;
            UserProfileImage.Source = GetProfileImageSource(_username);
            Level.Text = $"Level: {_level}";
            Timer.Text = "Time: 00:00"; // Initialize the timer display

            UpdateHeartsDisplay();

            CreateButtons();
            ShuffleAndAssignImages();

            DisplayAllTiles();
            DisableAllButtons();

            DispatcherTimer hideAllTilesTimer = new DispatcherTimer();
            hideAllTilesTimer.Interval = TimeSpan.FromSeconds(5);
            hideAllTilesTimer.Tick += (s, args) =>
            {
                hideAllTilesTimer.Stop();
                HideAllTiles();
                EnableAllButtons();
            };
            hideAllTilesTimer.Start();
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

        private void CreateButtons()
        {
            TileGrid.Children.Clear();

            for (int i = 0; i < TileGrid.Rows * TileGrid.Columns; i++)
            {
                Button button = new Button
                {
                    Name = $"Button{i + 1}",
                    Tag = "TileImagePath"
                };
                button.Click += Button_Click;
                TileGrid.Children.Add(button);
            }
        }

        private void DisplayAllTiles()
        {
            foreach (Button button in TileGrid.Children.OfType<Button>())
            {
                string imagePath = button.Tag.ToString();
                button.Content = new Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Relative))
                };
            }
        }

        private void HideAllTiles()
        {
            foreach (Button button in TileGrid.Children.OfType<Button>())
            {
                button.Content = null;
            }
        }

        private void UpdateHeartsDisplay()
        {
            Hearts.Text = $"Hearts: {_hearts}";
        }

        private void SetHeartsByLevel()
        {
            switch (_level)
            {
                case 1:
                    _hearts = 10;
                    break;
                case 2:
                    _hearts = 7;
                    break;
                case 3:
                    _hearts = 4;
                    break;
                default:
                    _hearts = 10;
                    break;
            }
        }

        private void ShuffleAndAssignImages()
        {
            // Load tile images from the folder
            string[] tileImages = Directory.GetFiles(@"..\..\TilesImages", "*.png");

            // Select the required number of images based on the number of tiles in the game
            int numberOfPairs = (TileGrid.Rows * TileGrid.Columns) / 2;
            var random = new Random();
            var selectedImages = tileImages.OrderBy(x => random.Next()).Take(numberOfPairs).ToArray();

            // Duplicate the selected images to have pairs
            List<string> tileImagePairs = new List<string>(selectedImages.Concat(selectedImages));

            // Shuffle the tile images
            int n = tileImagePairs.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                string value = tileImagePairs[k];
                tileImagePairs[k] = tileImagePairs[n];
                tileImagePairs[n] = value;
            }

            // Assign the shuffled images to the buttons
            int i = 0;
            foreach (Button button in TileGrid.Children.OfType<Button>())
            {
                button.Tag = tileImagePairs[i++];
            }
        }

        private void StartTimer()
        {
            _elapsedTime = TimeSpan.Zero;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void ResetTimer()
        {
            _timer.Stop();
            _elapsedTime = TimeSpan.Zero;
            Timer.Text = $"Time: {_elapsedTime.Minutes:D2}:{_elapsedTime.Seconds:D2}";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Increment the elapsed time
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));

            // Update the timer display
            Timer.Text = $"Time: {_elapsedTime.Minutes:D2}:{_elapsedTime.Seconds:D2}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_firstClick)
            {
                StartTimer();
                _firstClick = false;
            }

            Button clickedButton = sender as Button;

            // Disable the clicked button
            clickedButton.IsEnabled = false;

            // Show the image on the clicked button
            DisplayTile(clickedButton);

            if (_previousButton == null)
            {
                _previousButton = clickedButton;
            }
            else
            {
                DisableAllButtons();
                CompareTilesWithDelay(clickedButton);
            }
        }

        private void DisplayTile(Button button)
        {
            string imagePath = button.Tag.ToString();
            button.Content = new Image
            {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Relative))
            };
        }

        private void EnableAllButtons()
        {
            foreach (Button button in TileGrid.Children.OfType<Button>())
            {
                button.IsEnabled = true;
            }
        }

        private void DisableAllButtons()
        {
            foreach (Button button in TileGrid.Children.OfType<Button>())
            {
                button.IsEnabled = false;
            }
        }

        private void EnableUnmatchedButtons()
        {
            foreach (Button button in TileGrid.Children.OfType<Button>())
            {
                if (button.Content == null)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private void CompareTilesWithDelay(Button clickedButton)
        {
            DispatcherTimer comparisonTimer = new DispatcherTimer();
            comparisonTimer.Interval = TimeSpan.FromSeconds(1);
            comparisonTimer.Tick += (s, args) =>
            {
                comparisonTimer.Stop();

                if (_previousButton.Tag.ToString() == clickedButton.Tag.ToString() && _previousButton != clickedButton)
                {
                    HandleMatchedTiles(clickedButton);
                }
                else
                {
                    HandleUnmatchedTiles(clickedButton);
                }

                EnableUnmatchedButtons();
                _previousButton = null;
            };

            comparisonTimer.Start();
        }

        private void HandleMatchedTiles(Button clickedButton)
        {
            _previousButton.IsEnabled = false;
            clickedButton.IsEnabled = false;
            _matchesFound++;

            if (_matchesFound == (TileGrid.Rows * TileGrid.Columns) / 2)
            {
                _timer.Stop(); // Stop the timer when the level is completed
                _consecutiveWins++;

                if (_consecutiveWins == 3)
                {
                    UpdateStatistics(true);
                    MessageBox.Show("Congratulations! You have completed 3 levels!", "Game Completed");
                    GameWindow gameWindow = new GameWindow(_username);
                    gameWindow.Show();
                    this.Close();
                    return;
                }

                MessageBoxResult result = MessageBox.Show("Level completed! Do you want to play the next level?", "Level Completed", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _level++;
                    Level.Text = $"Level: {_level}";
                    SetHeartsByLevel();
                    UpdateHeartsDisplay();
                    ShuffleAndAssignImages();
                    DisplayAllTiles();
                    DisableAllButtons();

                    DispatcherTimer hideAllTilesTimer = new DispatcherTimer();
                    hideAllTilesTimer.Interval = TimeSpan.FromSeconds(5);
                    hideAllTilesTimer.Tick += (s, args) =>
                    {
                        hideAllTilesTimer.Stop();
                        HideAllTiles();
                        EnableAllButtons();
                    };
                    hideAllTilesTimer.Start();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void HandleUnmatchedTiles(Button clickedButton)
        {
            _hearts--;
            UpdateHeartsDisplay();

            if (_hearts == 0)
            {
                _timer.Stop(); // Stop the timer when the game is over
                UpdateStatistics(false);
                MessageBoxResult result = MessageBox.Show("Game over! Do you want to play again?", "Game Over", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    RestartGame();
                }
                else
                {
                    GameWindow gameWindow = new GameWindow(_username);
                    gameWindow.Show();
                    this.Close();
                }
            }
            else
            {
                _previousButton.Content = null;
                clickedButton.Content = null;
            }
        }

        private void UpdateStatistics(bool gameWon)
        {
            string statisticsFilePath = $@"..\..\Statistics\{_username}.txt";
            int gamesPlayed = 0;
            int gamesWon = 0;

            if (File.Exists(statisticsFilePath))
            {
                string[] lines = File.ReadAllLines(statisticsFilePath);
                string[] statistics = lines[0].Split(',');
                gamesPlayed = int.Parse(statistics[1]);
                gamesWon = int.Parse(statistics[2]);
            }

            gamesPlayed++;

            if (gameWon)
            {
                gamesWon++;
            }

            string output = $"{_username},{gamesPlayed},{gamesWon}";
            File.WriteAllText(statisticsFilePath, output);
        }


        private void RestartGame()
        {
            _firstClick = true;
            _matchesFound = 0;
            _level = 1;
            SetHeartsByLevel();
            UpdateHeartsDisplay();
            ResetTimer();
            ShuffleAndAssignImages();
            DisplayAllTiles();
            DisableAllButtons();

            DispatcherTimer hideAllTilesTimer = new DispatcherTimer();
            hideAllTilesTimer.Interval = TimeSpan.FromSeconds(5);
            hideAllTilesTimer.Tick += (s, args) =>
            {
                hideAllTilesTimer.Stop();
                HideAllTiles();
                EnableAllButtons();
            };
            hideAllTilesTimer.Start();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            //TO BE IMPLEMENTED
        }


        private void AutoWinButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop(); // Stop the timer
            _consecutiveWins++;

            if (_consecutiveWins == 3)
            {
                UpdateStatistics(true);
                MessageBox.Show("Congratulations! You have completed 3 levels!", "Game Completed");
                GameWindow gameWindow = new GameWindow(_username);
                gameWindow.Show();
                this.Close();
                return;
            }

            MessageBoxResult result = MessageBox.Show("Level completed! Do you want to play the next level?", "Level Completed", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _level++;
                Level.Text = $"Level: {_level}";
                SetHeartsByLevel();
                UpdateHeartsDisplay();
                ResetTimer();
                ShuffleAndAssignImages();
                DisplayAllTiles();
                DisableAllButtons();

                DispatcherTimer hideAllTilesTimer = new DispatcherTimer();
                hideAllTilesTimer.Interval = TimeSpan.FromSeconds(5);
                hideAllTilesTimer.Tick += (s, args) =>
                {
                    hideAllTilesTimer.Stop();
                    HideAllTiles();
                    EnableAllButtons();
                };
                hideAllTilesTimer.Start();
            }
            else
            {
                this.Close();
            }
        }


    }
}