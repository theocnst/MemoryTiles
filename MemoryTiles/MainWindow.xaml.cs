using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MemoryTiles
{
    public partial class MainWindow : Window
    {
        private string[] profileImages;
        private int currentImageIndex = 0;
        private string usersFilePath = @"..\..\Users.txt";

        private string _username;
        private ImageSource _profileImage;

        public MainWindow()
        {
            InitializeComponent();
            LoadUsers();
            LoadProfileImages();
            UpdateProfileImage();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void LoadUsers()
        {
            if (File.Exists(usersFilePath))
            {
                string[] lines = File.ReadAllLines(usersFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    string username = parts[0];
                    string imageName = parts[1];
                    UserList.Items.Add(username);
                }
            }
        }

        private void LoadProfileImages()
        {
            profileImages = Directory.GetFiles(@"..\..\ProfileImages", "*.jpg");
        }

        private void UpdateProfileImage()
        {
            if (profileImages.Length > 0)
            {
                using (FileStream stream = new FileStream(profileImages[currentImageIndex], FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Necessary to pass the image across threads
                    ProfileImage.Source = bitmap;
                }
            }
        }


        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserList.SelectedIndex != -1)
            {
                string selectedUser = UserList.SelectedItem.ToString();
                _username = selectedUser;
                string[] lines = File.ReadAllLines(usersFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (selectedUser == parts[0])
                    {
                        using (FileStream stream = new FileStream(parts[1], FileMode.Open, FileAccess.Read))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();
                            ProfileImage.Source = bitmap;
                            _profileImage= bitmap;
                        }
                        break;
                    }
                }
            }
        }

        private void NewUser_Click(object sender, RoutedEventArgs e)
        {
            string userName = NewUserName.Text.Trim();
            if (!string.IsNullOrEmpty(userName))
            {
                if (!UserList.Items.Contains(userName))
                {
                    UserList.Items.Add(userName);
                    File.AppendAllText(usersFilePath, $"{userName},{profileImages[currentImageIndex]}\n");
                    NewUserName.Clear();
                }
                else
                {
                    MessageBox.Show("User already exists. Please choose a different name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid user name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserList.SelectedItem != null)
            {
                string userToDelete = UserList.SelectedItem.ToString();

                // Remove user from the list
                UserList.Items.Remove(userToDelete);

                // Remove user from the Users.txt file
                List<string> updatedUsers = new List<string>();
                string[] lines = File.ReadAllLines(usersFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (userToDelete != parts[0])
                    {
                        updatedUsers.Add(line);
                    }
                }
                File.WriteAllLines(usersFilePath, updatedUsers);

                // Clear the ProfileImage
                ProfileImage.Source = null;
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            currentImageIndex = (currentImageIndex - 1 + profileImages.Length) % profileImages.Length;
            UpdateProfileImage();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            currentImageIndex = (currentImageIndex + 1) % profileImages.Length;
            UpdateProfileImage();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (UserList.SelectedItem != null)
            {
                GameWindow gameWindow = new GameWindow(_username);
                gameWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a user to play the game.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Close the application
            Application.Current.Shutdown();
        }
    }

}