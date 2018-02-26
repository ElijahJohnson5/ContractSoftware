using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MR_LAW_DB
{
    
    public delegate void updateDelegate(string arg);
    //The class that handles everything to do with the dataGrid on the main screen
    public class ContractDataGrid
    {
        DataGrid dataGrid;
        DataSet fullds = new DataSet();
        /// <summary>
        /// Initializes the dataGrid with items and a reference to the dataGrid on the main window
        /// </summary>
        /// <param name="windowDataGrid">The dataGrid that already exsists on the main screen</param>
        /// <param name="_ds">The data set the exsits from the main screen</param>
        public void initialize(ref DataGrid windowDataGrid, ref DataSet _ds)
        {
            dataGrid = windowDataGrid;
            fullds = new DataSet();
            //Initialize the indivdual dataGrid and then update it
            Thread update = new Thread(() => updateDataGrid("SELECT * FROM contracts;"));
            update.Start();
        }

        /// <summary>
        /// Sets basic values of the dataGrid
        /// </summary>

        /// <summary>
        /// Update the datagrid from the database and fill the dataset with new data
        /// </summary>
        public void updateDataGrid(string updateString)
        {
            object[] args = new object[1];
            args[0] = updateString;
            Application.Current.Dispatcher.BeginInvoke(new updateDelegate(updateThread), args);
        }

        private void updateThread(string updateString)
        {
            //Get it as a new dataset whenever it gets updated
            fullds = new DataSet();
            //Get the data from the contracts table in mysql
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(updateString, mySQL.con);
            //Fill the dataset and bind it with "loadDataBinding"
            Application.Current.Dispatcher.Invoke(() => dataAdapter.Fill(fullds, "loadDataBinding"));

            //Set the dataContext for the datagrid to the dataset
            Application.Current.Dispatcher.Invoke(() => dataGrid.DataContext = fullds);
        }

    }
}
