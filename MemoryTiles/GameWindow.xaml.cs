using System.Windows;


namespace MemoryTiles
{
    public partial class GameWindow : Window
    {

        private string _username;
        public GameWindow(string username)
        {
            _username = username;
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            PlayWindow playWindow = new PlayWindow(_username, 1, 10, 4, 4);
            playWindow.Show();
            this.Close();
        }

        private void CustomGameButton_Click(object sender, RoutedEventArgs e)
        {
            CustomGameWindow customGameWindow = new CustomGameWindow(_username);
            customGameWindow.ShowDialog();
        }

        private void LoadGame_Click(object sender, RoutedEventArgs e)
        {
            //TO BE IMPLEMENTED
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statisticsWindow = new StatisticsWindow(_username);
            statisticsWindow.ShowDialog();
        }


        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Constantin Theodor Grupa 211 Informatica", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
