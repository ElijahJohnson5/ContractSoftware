using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace MR_LAW_DB
{
    /// <summary>
    /// Interaction logic for settings.xaml
    /// The settings window, very simple settings writes to text file
    /// </summary>
    public partial class Settings : Window
    {
        secondWindow secondWin = null;
        public Settings(secondWindow window)
        {
            InitializeComponent();
            secondWin = window;
            try
            {
                //Open the settings file if it exsists
                FileStream settings = new FileStream(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts\\settings.txt"), FileMode.Open);
                using (var streamReader = new StreamReader(settings, Encoding.UTF8))
                {
                    string line;
                    int i = 0;
                    //Read the settings and set the text boxes
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        switch (i)
                        {
                            case 0:
                                IP.Text = line;
                                i++;
                                break;
                            case 1:
                                port.Text = line;
                                i++;
                                break;
                            case 2:
                                defaultEmailText.Text = line;
                                i++;
                                break;
                            case 3:
                                database.Text = line;
                                i++;
                                break;
                        }
                    }
                }
                settings.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return;
            };
        }

        /// <summary>
        /// Write a string to the file stream
        /// </summary>
        /// <param name="toWrite">The string to write</param>
        /// <param name="file">The file to write the string in</param>
        private void writeString(string toWrite, FileStream file)
        {
            byte[] bytes = new UTF8Encoding(true).GetBytes(toWrite);
            file.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Apply the settings when the button is clicked or the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applySettings(object sender, RoutedEventArgs e)
        {
            //Set the values in the main window to the values being saved
            MainWindow.host = IP.Text;
            MainWindow.port = Convert.ToUInt32(port.Text);
            MainWindow.emailString = defaultEmailText.Text;
            MainWindow.databaseName = database.Text;
            //Create a new mysql connection string
            mySQL.conString = new MySqlConnectionStringBuilder();
            mySQL.setConnectionString(MainWindow.host, MainWindow.user, MainWindow.pass, MainWindow.databaseName, MainWindow.port);
            mySQL.con = new MySqlConnection(mySQL.conString.ToString());
          
            //Create the directory, does nothing if it exsists
            Directory.CreateDirectory(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts")));
            FileStream settings = new FileStream(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts\\settings.txt"), FileMode.Create);
            //Write all of the important values
            writeString(MainWindow.host + "\n", settings);
            writeString(MainWindow.port.ToString() + "\n", settings);
            writeString(MainWindow.emailString + "\n", settings);
            writeString(MainWindow.databaseName + "\n", settings);
            //Close the file stream
            settings.Close();    
            //If settings change then reset the login
            if (secondWin != null)
            {
                secondWin.Close();
                MainWindow mainWin = new MainWindow();
                mainWin.Show();
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
