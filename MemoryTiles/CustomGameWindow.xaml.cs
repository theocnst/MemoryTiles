using System.Windows;
using System.Windows.Controls;

namespace MemoryTiles
{
    public partial class CustomGameWindow : Window
    {
        private string _username;
        public CustomGameWindow(string username)
        {
            InitializeComponent();

            _username = username;

            for (int i = 2; i <= 6; i++)
            {
                RowsComboBox.Items.Add(i);
            }

            RowsComboBox.SelectedIndex = 0;
            UpdateColumnsComboBox();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        private void RowsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateColumnsComboBox();
        }

        private void UpdateColumnsComboBox()
        {
            int selectedRow = (int)RowsComboBox.SelectedItem;
            ColumnsComboBox.Items.Clear();

            for (int j = 2; j <= 6; j++)
            {
                if (selectedRow % 2 == 0)
                {
                    ColumnsComboBox.Items.Add(j);
                }
                else
                {
                    if (j%2 == 0)
                        ColumnsComboBox.Items.Add(j);

                }
            }

            if (ColumnsComboBox.Items.Count > 0)
            {
                ColumnsComboBox.SelectedIndex = 0;
            }
        }
        private void StartCustomGame_Click(object sender, RoutedEventArgs e)
        {
            int rows = (int)RowsComboBox.SelectedItem;
            int columns = (int)ColumnsComboBox.SelectedItem;

            PlayWindow playWindow = new PlayWindow(_username, 1, 10, rows, columns);
            playWindow.Show();
            Close();
        }
    }
}
