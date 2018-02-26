using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Threading;

namespace MR_LAW_DB
{
    /// <summary>
    /// Interaction logic for openContract.xaml
    /// The window that displays a contract
    /// </summary>
    public partial class openContract : Window
    {
        string nameOfContract;
        string dateRecieved;
        string dateCompleted;
        string attorney;
        string notes;
        string contractExt;
        string tribalExt;
        string contractFilePath;
        string tribalFilePath;
        byte[] uploadedDocument;
        byte[] tribalCoverSheet;
        bool certificateOfInsurance;
        int id;
        Thread update;

     
        TextBox dateCompetedTextBox = null;
        DatePicker dateCompletedPicker = null;
        secondWindow openiningWindow = null;

        /// <summary>
        /// Open a contract from a data row view
        /// </summary>
        /// <param name="contractToOpen">The contract that is being opened</param>
        /// <param name="openiningWindow"></param>
        public openContract(DataRowView contractToOpen, secondWindow openiningWindow)
        {
            InitializeComponent();
            this.openiningWindow = openiningWindow;
            DataRowView rowView = contractToOpen;

            //Get all values one by one
            nameOfContract = rowView.Row[0].ToString();
            dateRecieved = rowView.Row[1].ToString();
            dateCompleted = rowView.Row[2].ToString();
            attorney = rowView.Row[3].ToString();
            notes = rowView.Row[6].ToString();
            contractExt = rowView.Row[8].ToString();
            tribalExt = rowView.Row[9].ToString();
            
            //For casting try all of them
            try
            {
                id = (int)rowView.Row[10];
            } catch
            {
                this.Close();
            }

            //Not always going to have an uploadedDocument
            try
            {
                uploadedDocument = (byte[])rowView.Row[4];
            } catch
            {
                uploadedDocument = null;
            }

            try
            {
                tribalCoverSheet = (byte[])rowView.Row[5];
            } catch
            {
                tribalCoverSheet = null;
            }
            try
            {
                certificateOfInsurance = (bool)rowView.Row[7];
            } catch { certificateOfInsurance = false; }


      
            createDateCompleted();

            nameOfContractTextBox.Text = nameOfContract;
            dateRecievedTextBox.Text = dateRecieved;
            noteTextBox.Text = notes;
            //Fill the attorney combo box
            ThreadStart starter = fillCombo;
            starter += () =>
            {
                this.Dispatcher.BeginInvoke(new Action(() => attorneys.SelectedItem = attorney));
            };
            update = new Thread(starter) { IsBackground = true };
            update.Start();

            COI.IsChecked = certificateOfInsurance;

        }

        /// <summary>
        /// Creates a file from a byte array
        /// </summary>
        /// <param name="docFile">The byte array to be turned into a file</param>
        /// <param name="ext">the ext for the file</param>
        /// <param name="name">The name of the contract</param>
        /// <returns>The filename if it was created successfully</returns>
        private string convertToFile(byte[] docFile, string ext, string name)
        {
            Directory.CreateDirectory(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts")));
            string fileString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts\\" + nameOfContractTextBox.Text + name) + ext;
            if (docFile != null)
            {
                using (Stream docStream = File.OpenWrite(fileString))
                {
                    docStream.Write(docFile, 0, docFile.Length);
                }

                if (File.Exists(fileString))
                {
                    return fileString;
                }
            }
            return null;
        }

        /// <summary>
        /// Downloads the contract or tribal sheet to the local computer from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createFile(object sender, RoutedEventArgs e)
        {
            Button s = sender as Button;
            string filepath;
            string messageString = "Files Downloaded: ";

            if ((filepath = convertToFile(uploadedDocument, contractExt, " Contract")) != null)
            {
                messageString += "\nContract";
               
            }
            if ((filepath = convertToFile(tribalCoverSheet, tribalExt, " Tribal Cover Sheet")) != null)
            {
                messageString += "\nTribal Cover Sheet";
            }
            if (messageString.Equals("Files Downloaded: "))
            {
                messageString = "No Files Available";
            }
            //Show the user what files were downloaded
            MessageBox.Show(messageString);
        }

        /// <summary>
        /// Open a dialog to upload a file
        /// </summary>
        /// <param name="sender"></param>
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
                //Check what file is being uploaded
                if (s == uploadContract)
                {
                    //Get the file extension and display the file that was chosen
                    contractExt = System.IO.Path.GetExtension(dlg.FileName);
                    contractFilePath = dlg.FileName;
                    int lastSlash = contractFilePath.LastIndexOf('\\');
                    uploadedContract.Text = contractFilePath.Remove(0, lastSlash + 1);
                    uploadedDocument = File.ReadAllBytes(contractFilePath);
                  
                }
                else
                {
                    tribalExt = System.IO.Path.GetExtension(dlg.FileName);
                    tribalFilePath = dlg.FileName;
                    int lastSlash = tribalFilePath.LastIndexOf('\\');
                    uploadedTribal.Text = tribalFilePath.Remove(0, lastSlash + 1);
                    tribalCoverSheet = File.ReadAllBytes(tribalFilePath);
                 
                }
            }
        }

        /// <summary>
        /// Create the date completed date picker
        /// </summary>
        private void createDateCompleted()
        {
            //If the datecompleted does not exsist create it
            if (dateCompleted == null || dateCompleted == "")
            {
                dateCompletedPicker = new DatePicker();
                windowGrid.Children.Add(dateCompletedPicker);
                dateCompletedPicker.Margin = new Thickness(140, 85, 0, 0);
                dateCompletedPicker.Width = 120;
                dateCompletedPicker.Height = 23;
                dateCompletedPicker.HorizontalAlignment = HorizontalAlignment.Left;
                dateCompletedPicker.VerticalAlignment = VerticalAlignment.Top;
                dateCompletedPicker.Name = "dateCompletedPicker";

            }
            else
            {
                dateCompetedTextBox = new TextBox();
                windowGrid.Children.Add(dateCompetedTextBox);
                dateCompetedTextBox.Margin = new Thickness(140, 85, 0, 0);
                dateCompetedTextBox.Width = 120;
                dateCompetedTextBox.Height = 23;
                dateCompetedTextBox.Text = dateCompleted;
                dateCompetedTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                dateCompetedTextBox.VerticalAlignment = VerticalAlignment.Top;
            }
        }

        /// <summary>
        /// Creates a data table from an sql query
        /// </summary>
        /// <returns>The new data table filled with the attorney names</returns>
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
        /// Fill the combo box with attorney names
        /// on a different thread
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
        /// Open the contract in a word document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openInWord(object sender, RoutedEventArgs e)
        {
            wordDocument.fillWordDocument(nameOfContract, dateRecieved, dateCompleted, attorney, notes, uploadedDocument, contractExt, tribalCoverSheet, tribalExt);
        }

        /// <summary>
        /// Insert the new contract into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateDatabase(object sender, RoutedEventArgs e)
        {
            //Get all relavent data from the ui elements
            nameOfContract = nameOfContractTextBox.Text;
            dateRecieved = dateRecievedTextBox.Text;
            if (dateCompletedPicker != null)
            {
                dateCompleted = dateCompletedPicker.Text;
            } else
            {
                dateCompleted = dateCompetedTextBox.Text;
            }

            if (attorneys.SelectedItem != null)
            {
                attorney = attorneys.SelectedItem.ToString();
            }
            certificateOfInsurance = (bool)COI.IsChecked;
            notes = noteTextBox.Text;

            //If the contract exsists remove it and re insert it with new data
            mySQL.removeFromContracts(id);
            mySQL.insertIntoContracts(nameOfContract, dateRecieved, attorney, certificateOfInsurance, dateCompleted, contractFilePath, tribalFilePath, notes, contractExt, tribalExt, uploadedDocument, tribalCoverSheet);
            //Email the attorney
            emailAttorney();
            //Update datagrid on a new thread
            update = new Thread(() => openiningWindow.dataGridClass.updateDataGrid("SELECT * FROM contracts;"));
            update.Start();
            Close();
        }

        /// <summary>
        /// Delete a contract if the button is clicked
        /// </summary>
        /// <param name="sender">The delete contract button</param>
        /// <param name="e"></param>
        private void deleteContractClick(object sender, RoutedEventArgs e)
        {
            //Remove it from database and update dataGrid
            mySQL.removeFromContracts(id);
            Thread Update = new Thread(() => openiningWindow.dataGridClass.updateDataGrid("SELECT * FROM contracts;"));
            update.Start();
            Close();
        }

        /// <summary>
        /// Email the attorney on a new contract being assigned to them
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

                if (tribalFilePath != null || contractFilePath != null)
                {
                    mailMessage = new MailMessage("noreplymesonrecursion@gmail.com", email, "New Contract", MainWindow.emailString + " see attachment for more details. \nBasic Details: " + ((nameOfContractTextBox.Text == "Name of Contract") ? "" : nameOfContractTextBox.Text) + "\nDate Received: " + ((dateRecievedTextBox.Text == "") ? DateTime.Today.ToShortDateString() : dateRecievedTextBox.Text) + "\n" + "Extra Notes: " + ((noteTextBox.Text == "Notes") ? "" : noteTextBox.Text));
                    if (contractFilePath != null)
                    {
                        contractFile = new Attachment(contractFilePath);
                        mailMessage.Attachments.Add(contractFile);
                    }

                    if (tribalFilePath != null)
                    {
                        tribalFile = new Attachment(tribalFilePath);
                        mailMessage.Attachments.Add(tribalFile);
                    }

                }
                else
                {
                    mailMessage = new MailMessage("noreplymesonrecursion@gmail.com", email, "New Contract", MainWindow.emailString + "\nDetails: " + ((nameOfContractTextBox.Text == "Name of Contract") ? "" : nameOfContractTextBox.Text) + "\nDate Received: " + ((dateRecievedTextBox.Text == "") ? DateTime.Today.ToShortDateString() : dateRecievedTextBox.Text) + "\n" + "Extra Notes: " + ((noteTextBox.Text == "Notes") ? "" : noteTextBox.Text));
                }

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential("noreplymesonrecursion@gmail.com", "password");
                client.SendAsync(mailMessage, "Test");
            }
            catch
            {
                return;
            }
        }
    }
}
