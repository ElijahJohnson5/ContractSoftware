using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using System.IO;

namespace MR_LAW_DB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Login window
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string user = "";
        public static string pass = "";
        public static string host = "localhost";
        public static uint port = 3306;
        public static string emailString = "A new contract has been assigned to you";
        public static string databaseName = "law_firm";
        TextBlock passwordRequired = new TextBlock();
        TextBlock usernameRequired = new TextBlock();


        public MainWindow()
        {
            System.Threading.Thread.Sleep(2000);
            InitializeComponent();
        }

        private void getUserPass(object sender, RoutedEventArgs e)
        {
            user = userName.Text;
            pass = passwordBox.Password;
            /// Check if the user entered a user name or password and make sure they entered both because they are required
            if (pass == "" || user == "")
            {
                /// if pass is empty show a message to the user saying they need to enter a password
                if (pass == "")
                {
                    setPasswordRequired();
                }

                /// if user is empty show a message to the user sayinmg they need to enter one
                if (user == "")
                {
                    setUsernameRequired();
                }
                return;

            }
            try
            {
                FileStream settings = new FileStream(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts\\settings.txt"), FileMode.Open);
                using (var streamReader = new StreamReader(settings, Encoding.UTF8))
                {
                    string line;
                    int i = 0;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        switch (i)
                        {
                            case 0:
                                host = line;
                                i++;
                                break;
                            case 1:
                                port = Convert.ToUInt32(line);
                                i++;
                                break;
                            case 2:
                                emailString = line;
                                i++;
                                break;
                            case 3:
                                databaseName = line;
                                i++;
                                break;
                        }
                    }
                }
                settings.Close();
            } catch { };
            /// Create a connection string and try connect to mysql server
            /// using a mySql class with static members so only one connection can be active at a time
            mySQL.conString = new MySqlConnectionStringBuilder();
            mySQL.setConnectionString(host, user, pass, databaseName, port);
            mySQL.con = new MySqlConnection(mySQL.conString.ToString());

            /// Try to open the connection
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                    //If connection is succesful open the next window and close the login window
                    secondWindow Window = new secondWindow();
                    Window.Show();
                    this.Close();
                }
                /// If unsucessful catch teh exception and do different things for different exceptions
                catch (MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        /// Login failed ask for new credentials
                        case 1045:
                        case 1042:
                        case 0:
                            MessageBox.Show(ex.ToString(), "Error connecting to database", MessageBoxButton.OK);
                            new MainWindow().Show();
                            usernameRequired = new TextBlock();
                            usernameRequired.Text = "Invalid login password or username";
                            loginGrid.Children.Add(usernameRequired);
                            Grid.SetColumn(usernameRequired, 1);
                            Grid.SetRow(usernameRequired, 2);
                            usernameRequired.Foreground = Brushes.Red;
                            usernameRequired.Margin = new Thickness(0, submit.Height + 20, 0, 0);
                            userName.Text = "";
                            passwordBox.Password = "";
                            Close();
                            return;
                    }
                }
                    
            }
        }

        /// clear the password text and remove it from the screen while the user is entering a password
        private void clearPasswordRequired(object sender, RoutedEventArgs e)
        {
            if (passwordRequired != new TextBlock())
            {
                loginGrid.Children.Remove(passwordRequired);
                passwordRequired = new TextBlock();
            }
        }

        //Clear username required text from the screen
        private void clearUsernameRequired(object sender, RoutedEventArgs e)
        {
            if (usernameRequired != new TextBlock())
            {
                loginGrid.Children.Remove(usernameRequired);
                usernameRequired = new TextBlock();
            }
        }

        private void setUsernameRequired()
        {
            usernameRequired = new TextBlock();
            usernameRequired.Text = "Username is required, please enter one";
            /// Position the text in the right space on the grid and make the text red and noticible
            loginGrid.Children.Add(usernameRequired);
            Grid.SetColumn(usernameRequired, 1);
            Grid.SetRow(usernameRequired, 1);
            usernameRequired.Foreground = Brushes.Red;
            usernameRequired.Margin = new Thickness(200, 0, 0, 0);
        }

        private void setPasswordRequired()
        {
            passwordRequired = new TextBlock();
            passwordRequired.Text = "Password is required, please enter one";

            /// Poisition the text next to the password box
            loginGrid.Children.Add(passwordRequired);
            Grid.SetColumn(passwordRequired, 1);
            Grid.SetRow(passwordRequired, 1);
            passwordRequired.Foreground = Brushes.Red;
            passwordRequired.Margin = new Thickness(200, 25, 0, 0);
        }

        private void submitWithEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                getUserPass(sender, new RoutedEventArgs());
            }
        }

        private void openSettings(object sender, RoutedEventArgs e)
        {
            Settings setting = new Settings(null);
            setting.Show();
        }
    }
}

