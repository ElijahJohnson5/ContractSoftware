using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Threading;

namespace MR_LAW_DB
{
    /// <summary>
    /// Interaction logic for newContract.xaml
    /// </summary>
    public partial class newContract : Window
    {
        //Declare Needed variables including a refrence to the window that opened this window
        string contractFilePath = null;
        string tribalCoverSheetFilePath = null;
        string filetypeOfContract = null;
        string filetypeOfTribal = null;
        secondWindow openiningWindow;

        public newContract(secondWindow window)
        {
            
            InitializeComponent();

            //Set the text color to a partially transparent grey
            Color test = Color.FromArgb(190, 190, 198, 212);
            SolidColorBrush color = new SolidColorBrush(test);
            nameOfContract.Foreground = color;
            extraNotes.Foreground = color;
            attorneys.Foreground = color;

            //Set the member variable to the window sent in when this is created
            openiningWindow = window;


            Thread combo = new Thread(fillCombo);
            combo.Start();
        }

        /// <summary>
        /// Clears the text of textboxes when they are clicked on
        /// </summary>
        /// <param name="sender">The text box that was clicked</param>
        /// <param name="e"></param>
        private void clearText(object sender, RoutedEventArgs e)
        {
            //Get the sender object as a text box
            TextBox t = (TextBox)sender;

            //Check which text box sent called the function
            if (t.Name == "nameOfContract")
            {
                //If it is the starting text set it to nothing
                if (t.Text == "Name of Contract")
                {
                    t.Text = "";
                }
            } else
            {
                if (t.Text == "Notes")
                {
                    t.Text = "";
                }
            }
            //When focus is gained change the text color to black
            t.Foreground = Brushes.Black;
        }

        /// <summary>
        /// Set the text of a text box back to the default text if nothing is entered
        /// </summary>
        /// <param name="sender">The text box that called the function</param>
        /// <param name="e"></param>
        private void setText(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;

            //Check which text box called the function
            if (t.Name == "nameOfContract")
            {
                //If there is no text set it back to the default text and change the color to transparent grey
                if (t.Text == "")
                {
                    Color test = Color.FromArgb(190, 190, 198, 212);
                    SolidColorBrush color = new SolidColorBrush(test);
                    t.Text = "Name of Contract";
                    t.Foreground = color;
                }
            } else
            {
                if (t.Text == "")
                {
                    Color test = Color.FromArgb(190, 190, 198, 212);
                    SolidColorBrush color = new SolidColorBrush(test);
                    t.Text = "Notes";
                    t.Foreground = color;
                }
            }
        }


        /// <summary>
        /// Uploads a file when the upload contract button or tribal cover sheet button are clicked
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e"></param>
        private void uploadClick(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;

            //Open a file dialog to be able to pick a file to be uploaded
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //Set the default extension and the allowed file types to pick from the dialog
            dlg.DefaultExt = "*.docx";
            dlg.Filter = "DOCX Files (*.docx)|*.docx|PDF Files (*.pdf)|*.pdf";

            //Show the dialog box and if a file gets selected put it into a string
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                if (s.Name == "uploadDocument")
                {
                    //Get the file extension and display the file that was chosen
                    filetypeOfContract = System.IO.Path.GetExtension(dlg.FileName);
                    contractFilePath = dlg.FileName;
                    int lastSlash = contractFilePath.LastIndexOf('\\');
                    uploadedContractPath.Text = contractFilePath.Remove(0, lastSlash + 1);
                } else
                {
                    filetypeOfTribal = System.IO.Path.GetExtension(dlg.FileName);
                    tribalCoverSheetFilePath = dlg.FileName;
                    int lastSlash = tribalCoverSheetFilePath.LastIndexOf('\\');
                    tribalCoverSheetPath.Text = tribalCoverSheetFilePath.Remove(0, lastSlash + 1);
                }
            }
        }

        /// <summary>
        /// Submits the information from this window into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitToDatabase(object sender, RoutedEventArgs e)
        {
            ThreadStart starter = insertIntoDatabase;
            starter += () =>
            {
                Thread update = new Thread(updateDatagrid);
                update.Start();
            };
            Thread insert = new Thread(starter);
            insert.Start();
            emailAttorney();
            this.Close();
        }

        /// <summary>
        /// Inserts the new contract into the database
        /// </summary>
        private void insertIntoDatabase()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {try
                {

                mySQL.insertIntoContracts((nameOfContract.Text == "Name of Contract") ? "" : nameOfContract.Text, (received.Text == "") ? DateTime.Today.ToShortDateString() : received.Text, attorneys.Text, certificateOfInsurance.IsChecked.Value, (completed.Text == "Select a date") ? "" : completed.Text, contractFilePath, tribalCoverSheetFilePath, (extraNotes.Text == "Notes") ? null : extraNotes.Text, filetypeOfContract, filetypeOfTribal);
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }));       
        }

        /// <summary>
        /// Updates the datagrid on the mainWindow
        /// </summary>
        private void updateDatagrid()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                openiningWindow.dataGridClass.updateDataGrid("SELECT * FROM contracts;");
            }));
        }

        /// <summary>
        /// Changes the color of the combo box when it gains focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeColor(object sender, RoutedEventArgs e)
        {
            ComboBox c = (ComboBox)sender;

            c.Foreground = Brushes.Black;
        }
        
        /// <summary>
        /// Changes the color of the combo box when it looses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeColorCheckText(object sender, RoutedEventArgs e)
        {
            ComboBox c = (ComboBox)sender;

            if (c.Text == "Assigned To")
            {
                Color grey = Color.FromArgb(190, 190, 190, 211);
                SolidColorBrush color = new SolidColorBrush(grey);
                c.Foreground = color;
            }
        }

        /// <summary>
        /// Fills a data table with all of the names of the attorney
        /// </summary>
        /// <returns>Returns the new data table created</returns>
        private DataTable fillDataTable()
        {
            using (mySQL.con)
            {
                mySQL.con.Open();

                MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT name FROM attorney", mySQL.con);
                DataTable dt = new DataTable("name");
                adapter.Fill(dt);

                return dt;
            }
        }

        /// <summary>
        /// Fills the combo box with all of the possible attorneys
        /// </summary>
        private void fillCombo()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                DataTable dt = fillDataTable();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    attorneys.Items.Add(dt.Rows[i].ItemArray[0].ToString());
                }
            }));
        }

        /// <summary>
        /// Email an attorney when a new contract is assigned to them
        /// </summary>
        private void emailAttorney()
        {
            try
            {
                string selectedItem = attorneys.SelectedItem.ToString();
                string email = mySQL.getEmailOfAttorney(selectedItem);
                MailMessage mailMessage;
                Attachment contractFile;
                Attachment tribalFile;

                //If there are files, make sure to attach them
                if (tribalCoverSheetFilePath != null || contractFilePath != null)
                {
                    mailMessage = new MailMessage("noreplymesonrecursion@gmail.com", email, "New Contract", MainWindow.emailString + "see attachment for more details. \nBasic Details: " + ((nameOfContract.Text == "Name of Contract") ? "" : nameOfContract.Text) + "\nDate Received: " + ((received.Text == "") ? DateTime.Today.ToShortDateString() : received.Text) + "\n" + "Extra Notes: " + ((extraNotes.Text == "Notes") ? "" : extraNotes.Text));
                    if (contractFilePath != null)
                    {
                        contractFile = new Attachment(contractFilePath);
                        mailMessage.Attachments.Add(contractFile);
                    }

                    if (tribalCoverSheetFilePath != null)
                    {
                        tribalFile = new Attachment(tribalCoverSheetFilePath);
                        mailMessage.Attachments.Add(tribalFile);
                    }
                    
                } else
                {
                    mailMessage = new MailMessage("noreplymesonrecursion@gmail.com", email, "New Contract", MainWindow.emailString + "\nDetails: " + ((nameOfContract.Text == "Name of Contract") ? "" : nameOfContract.Text) + "\nDate Received: " + ((received.Text == "") ? DateTime.Today.ToShortDateString() : received.Text) + "\n" + "Extra Notes: " + ((extraNotes.Text == "Notes") ? "" : extraNotes.Text));
                }
                //Send the email over gmail
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("noreplymesonrecursion@gmail.com", "password");
                client.SendAsync(mailMessage, "Test");
            } catch
            {
                return;
            }
        }
    }
}
