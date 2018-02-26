using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MR_LAW_DB
{
    /// <summary>
    /// Interaction logic for Notes.xaml
    /// </summary>
    public partial class Notes : Window
    {
        int idOfRow;
        secondWindow window;
        Thread update;

        //Creates a new notes window with an id and the openining window reference
        public Notes(string note, int id, secondWindow openiningWindow)
        {
            InitializeComponent();
            notes.Text = note;
            idOfRow = id;
            window = openiningWindow;
        }
        
        /// <summary>
        /// Saves the notes if the save button was clicked, and then exit the window
        /// </summary>
        /// <param name="sender">The save button</param>
        /// <param name="e"></param>
        private void saveNotes(object sender, RoutedEventArgs e)
        {
            //Update the notes
            if (mySQL.updateNotes(notes.Text, idOfRow))
            {
                //Update the datagrid on a new thread
                update = new Thread(() => window.dataGridClass.updateDataGrid("SELECT * FROM contracts;"));
                update.Start();
                //Show that the notes were saved
                MessageBoxResult result = MessageBox.Show("Notes saved successfully", "Saved", MessageBoxButton.OKCancel);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        this.Close();
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                }
            } else
            {
                MessageBox.Show("Notes were not saved", "Error");
            }
        }
    }
}
