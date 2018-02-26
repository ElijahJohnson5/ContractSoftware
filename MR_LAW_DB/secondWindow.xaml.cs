using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Data;
using System.Threading;

namespace MR_LAW_DB
{
    /// <summary>
    /// Interaction logic for secondWindow.xaml
    /// </summary>
    public partial class secondWindow : Window
    {
        //Keep the dataset as a variable, will be getting it from mysql
        public static DataSet ds = null;
        newContract newContract = null;
        openContract window = null;
        Notes note = null;
        AddAttorney addAttorneyClass = new AddAttorney();
        RemoveAttorney removeAttorneyClass = new RemoveAttorney();
        public ContractDataGrid dataGridClass = new ContractDataGrid();
        string lastSearch = "";
        Thread update;
        Thread checkDatabase;

        public secondWindow()
        {
            InitializeComponent();
            //Initilize the data grid, the starting size and format options
            if (dataGrid == null)
            {
                MessageBox.Show("Data grid is null");
                return;
            }
            dataGridClass.initialize(ref dataGrid, ref ds);
            
            addAttorneyClass.initialize(ref nameOfAttorneyAdd, ref emailOfAttorney, ref submitAttorney);
            removeAttorneyClass.initialize(ref nameOfAttorneyRemove, ref removeAttorney);
            searchBox.Foreground = new SolidColorBrush(Color.FromArgb(190, 190, 190, 211));

            checkDatabase = new Thread(checkUpdate);
            checkDatabase.Start();

        }


        private void openNewContractWindow(object sender, RoutedEventArgs e)
        {
            //Open the window to create a new contract and add it to the database
            newContract = new newContract(this);
            newContract.Show();
        }

        private void rowDoubleClick(object sender, RoutedEventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            //Open a contract if it is double clicked on the dataGrid
            DataGridRow row = (DataGridRow)sender;

            //Only open a new window if right mouse is not pressed
            if (me.RightButton == MouseButtonState.Pressed)
            {
                return;
            }
            else
            {
                var view = row.Item as DataRowView;
                window = new openContract(view, this);
                window.Show();
            }
        }
        
        /// <summary>
        /// Open a open contract window the the contract that was selected
        /// </summary>
        /// <param name="sender">The object that was clickde</param>
        /// <param name="e"></param>
        private void editContractClick(object sender, RoutedEventArgs e)
        {
            DataRowView row;
            try
            {
                //Get the first selected item as a DataRowView
               row = dataGrid.SelectedItems[0] as DataRowView;
            } catch
            {
                return;
            }
            //Open a new openContract window with the row, and this window as the opening window
            window = new openContract(row, this);
            window.Show();
        }

        /// <summary>
        /// When this window is closed close all other windows this window can create, exit the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeOtherWindows(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Check all the references to the other windows, if they are not null close them
            if (window != null)
            {
                window.Close();
            }

            if (newContract != null)
            {
                newContract.Close();
            }

            if (note != null)
            {
                note.Close();
            }

            //Close the mySQL connection and abort the update thread
            mySQL.con.Close();
            checkDatabase.Abort();
        }

        /// <summary>
        /// Query the database for the search term typed in
        /// </summary>
        /// <param name="sender">The object sending this event</param>
        /// <param name="e"></param>
        private void searchDataSet(object sender, TextChangedEventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            if (searchBox.Text == "Search")
                return;
            
            //If nothing is typed in select all from the database
            if (searchBox.Text == "")
            {
                update = new Thread(() => dataGridClass.updateDataGrid("SELECT * FROM contracts;"));
                update.Start();
                return;
            }

            //Create a new MySqlCommand with the search term
            lastSearch = searchBox.Text;
            MySql.Data.MySqlClient.MySqlCommand searchCommand = new MySql.Data.MySqlClient.MySqlCommand(mySQL.searchString, mySQL.con);
            searchCommand.Parameters.AddWithValue("?SEARCH", searchBox.Text);
            string query = searchCommand.CommandText;

            //Turn the searchCommand into a string and then loop through
            //Needed because the updateDataGrid takes a string but we dont want any injection possible in the search box
            foreach (MySql.Data.MySqlClient.MySqlParameter p in searchCommand.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            //Update the data grid on a new thread
            update = new Thread(() => dataGridClass.updateDataGrid(query));
            update.Start();
        }

        /// <summary>
        /// Reset the text back to default, or clear it when it is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearText(object sender, RoutedEventArgs e)
        {
            TextBox s = sender as TextBox;

            if (s == searchBox)
            {
                if (s.Text == "Search")
                {
                    s.Text = "";
                    s.Foreground = Brushes.Black;
                }
            } else if (s.Name == "nameOfAttorneyAdd")
            {
                addAttorneyClass.clearText(true);
            } else if (s.Name == "emailOfAttorney")
            {
                addAttorneyClass.clearText(false);
            } else if ( s.Name == "nameOfAttorneyRemove")
            {
                removeAttorneyClass.clearText();
            }
        }

        /// <summary>
        /// Set the text back to the default when they are not in focus anymore and they dont have any text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setText(object sender, RoutedEventArgs e)
        {
            TextBox s = sender as TextBox;

            if (s == searchBox)
            {
                if (s.Text == "")
                {
                    s.Text = "Search";
                    Color color = Color.FromArgb(190, 190, 190, 211);
                    SolidColorBrush grey = new SolidColorBrush(color);
                    s.Foreground = grey;
                    update = new Thread(() => dataGridClass.updateDataGrid("SELECT * FROM contracts"));
                    update.Start();
                }
            } else if (s.Name == "nameOfAttorneyAdd" || s.Name == "nameOfAttorneyRemove")
            {
                removeAttorneyClass.setText();
                addAttorneyClass.setText(true);
            } else if (s.Name == "emailOfAttorney")
            {
                addAttorneyClass.setText(false);
            }
        }

        /// <summary>
        /// Adds a new attorney to the mySql database
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e"></param>
        private void addAttorneyToDatabase(object sender, RoutedEventArgs e)
        {
            //Add the attorney based on information given
            if (addAttorneyClass.addAttorney() == false)
            {
                return;
            }
            Button send = sender as Button;
            send.Visibility = Visibility.Hidden;
            nameOfAttorneyAdd.Visibility = Visibility.Hidden;
            emailOfAttorney.Visibility = Visibility.Hidden;

            if (cancelButton.Visibility == Visibility.Visible)
            {
                cancelButton.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Removes an attorney from the mySql database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeAttorneyFromDatabase(object sender, RoutedEventArgs e)
        {
            if (removeAttorneyClass.removeAttorney() == false)
            {
                return;
            }
            Button send = sender as Button;
            send.Visibility = Visibility.Hidden;
            nameOfAttorneyRemove.Visibility = Visibility.Hidden;
            cancelButton.Visibility = Visibility.Hidden;
            removeButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Opens the notes from a datagrid entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openNotes(object sender, RoutedEventArgs e)
        {
            DataRowView row = dataGrid.SelectedItems[0] as DataRowView;
            note = new Notes(row[6].ToString(), (int)row[10], this);
            note.Show();
        }
        
        /// <summary>
        /// Sets the visiblity of the add attorney ui elements if the add attorney button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showAddAttorney(object sender, RoutedEventArgs e)
        {
            nameOfAttorneyRemove.Visibility = Visibility.Hidden;
            removeButton.Visibility = Visibility.Hidden;
            nameOfAttorneyRemove.Text = "Name of Attorney";
            removeAttorneyClass.setColor();
            nameOfAttorneyAdd.Visibility = Visibility.Visible;
            emailOfAttorney.Visibility = Visibility.Visible;
            okButton.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Sets the visibility of the remove attorney ui elements if the remove attorney button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showRemoveAttorney(object sender, RoutedEventArgs e)
        {
            nameOfAttorneyAdd.Visibility = Visibility.Hidden;
            emailOfAttorney.Visibility = Visibility.Hidden;
            nameOfAttorneyAdd.Text = "Name of Attorney";
            addAttorneyClass.setColorTransparent(true);
            addAttorneyClass.setColorTransparent(false);
            emailOfAttorney.Text = "Email Address";
            nameOfAttorneyRemove.Visibility = Visibility.Visible;
            removeButton.Visibility = Visibility.Visible;
            cancelButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clears the add attorney/remove attorney ui elements and resets the text boxes if the cancel button was
        /// clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clear(object sender, RoutedEventArgs e)
        {
            okButton.Visibility = Visibility.Hidden;
            removeButton.Visibility = Visibility.Hidden;
            cancelButton.Visibility = Visibility.Hidden;
            nameOfAttorneyAdd.Text = "Name of Attorney";
            addAttorneyClass.setColorTransparent(true);
            addAttorneyClass.setColorTransparent(false);
            emailOfAttorney.Text = "Email Address";
            nameOfAttorneyRemove.Text = "Name of Attorney";
            removeAttorneyClass.setColor();
            nameOfAttorneyAdd.Visibility = Visibility.Hidden;
            emailOfAttorney.Visibility = Visibility.Hidden;
            nameOfAttorneyRemove.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Opens a new settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSettings(object sender, RoutedEventArgs e)
        {
            Settings setting = new Settings(this);
            setting.Show();
        }

        /// <summary>
        /// Trys to delete a contract from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteContract(object sender, RoutedEventArgs e)
        {
            int id;
            DataRowView row;
            try
            {
                row = dataGrid.SelectedItems[0] as DataRowView;
            }
            catch
            {
                return;
            }

            try
            {
                //Get the id, the tenth element of the row
                id = (int)row.Row[10];
                //Remove a contract with mySql class based off of id
                mySQL.removeFromContracts(id);
                update = new Thread(() => dataGridClass.updateDataGrid("SELECT * FROM contracts"));
                update.Start();
            } catch
            {
                MessageBox.Show("Contract could not be deleted");
                return;
            }
        }

        /// <summary>
        /// Check if the data grid needs to be updated, based on whether another computer has entered a contract
        /// TODO make this work better, and not be so cumbersome
        /// </summary>
        private void checkUpdate()
        {
            while (true)
            {
                if (mySQL.CallProc())
                {
                    Thread update = new Thread(() => dataGridClass.updateDataGrid("SELECT * FROM contracts"));
                    update.Start();
                    System.Threading.Thread.Sleep(5000);
                    mySQL.setUpdate();
                }
                else
                {
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
            
    }
}
