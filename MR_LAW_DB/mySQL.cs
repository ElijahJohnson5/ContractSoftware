using System;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows;

namespace MR_LAW_DB
{
    /// <summary>
    /// Static class that handles everything to do with mySQl
    /// </summary>
    static public class mySQL
    {
        //Declare static strings and variables needed throughout the program
        public static MySqlConnection con = new MySqlConnection();
        public static MySqlConnectionStringBuilder conString;
        public static MySqlCommand command;
        //All command strings used, using paramaters to stop injection into the database
        public static string insertContracts = "INSERT INTO contracts (name, DateReceived, Attorney, CertificateOfInsurance, DateCompleted, UploadedContract, TribalCoverSheet, Notes, fileExtContract, fileExtTribal) VALUES (?Name, ?DateReceived, ?Attorney, ?CertificateOfInsurace, ?DateCompleted, ?UploadedContractFile, ?TribalCoverFile, ?Notes, ?fileExtContract, ?fileExtTribal);";
        public static string insertAttorney = "INSERT INTO attorney (name, email) VALUES (?NAME, ?EMAIL);";
        public static string setSqlSafeUpdate = "SET SQL_SAFE_UPDATES = 0;";
        public static string removeAttorney = "DELETE FROM attorney WHERE name=?NAME;";
        public static string selectAttorneyByName = "SELECT email FROM attorney WHERE name=?NAME;";
        public static string removeContract = "DELETE FROM contracts WHERE ID=?ID;";
        public static string updateNotesString = "UPDATE contracts SET Notes=?Notes WHERE ID=?ID;";
        public static string searchString = "SELECT * FROM contracts WHERE lower(Concat(Name, '', Attorney, '', DateReceived, '', DateCompleted)) LIKE lower(\"%?SEARCH%\");";

        /// <summary>
        /// Sets the connection string based off of the strings pased in
        /// </summary>
        /// <param name="server">The ip to the server</param>
        /// <param name="user">The user logging in</param>
        /// <param name="pass">The users password</param>
        /// <param name="database">The database to connect to, default none</param>
        /// <param name="port">The port the server is serving, default 3306</param>
        public static void setConnectionString(string server, string user, string pass, string database = null, uint port = 3306)
        {
            conString.Server = server;
            conString.UserID = user;
            conString.Password = pass;
            conString.Database = database;
            conString.Port = port;
            conString.AllowUserVariables = true;
        }

        /// <summary>
        /// Sets the update value in the database table to true, 
        /// so that the data grid gets upgraded
        /// </summary>
        public static void setUpdate()
        {
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                    command = new MySqlCommand("UPDATE updateTable SET updateValue = false WHERE ID = 1;", con);
                    command.ExecuteNonQuery();
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Insert a new entry into the contracts table in the database
        /// </summary>
        /// <param name="name">The name of the contract</param>
        /// <param name="dateReceived">The date the contract was recieved</param>
        /// <param name="attorney">The attorney handeling the contract, can be left blank</param>
        /// <param name="certificateOfInsurance">Whether or not a certificateOfInsurance is recieved</param>
        /// <param name="dateCompleted">The date the contract was completed, can be left blank</param>
        /// <param name="uploadedContractFile">The file uploaded along with the contract, can be left blank</param>
        /// <param name="tribalCoverFile">The tribal cover sheet for the file</param>
        /// <param name="notes">The notes to go along with the contract</param>
        /// <param name="fileExtContract">The ext of the uploaded contract file</param>
        /// <param name="fileExtTribal">The ext of the uploaded tribal file</param>
        /// <param name="byteContract">The byte array of the contract uploaded, can be left blank, used for updating</param>
        /// <param name="byteTribal">The byte array of the tribal file uploaded, can be left blank, used for updating</param>
        public static void insertIntoContracts(string name, string dateReceived, string attorney, bool certificateOfInsurance, string dateCompleted = "", string uploadedContractFile = null, string tribalCoverFile = null, string notes = "", string fileExtContract = "", string fileExtTribal = "", byte[] byteContract = null, byte[] byteTribal = null)
        {
            byte[] rawDataContract = null;
            byte[] rawDataTribal = null;
            //Read in the file as bytes into two byte arrays
            if (uploadedContractFile != null)
            {
                rawDataContract = File.ReadAllBytes(uploadedContractFile);

            }
            if (tribalCoverFile != null)
            {
                rawDataTribal = File.ReadAllBytes(tribalCoverFile);
            }

            //Using the mySQL connection
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                    //Insert all the values into the command
                    command = new MySqlCommand(insertContracts, con);
                    command.Parameters.AddWithValue("?Name", name);
                    command.Parameters.AddWithValue("?DateReceived", dateReceived);
                    command.Parameters.AddWithValue("?Attorney", attorney);
                    command.Parameters.AddWithValue("?CertificateOfInsurace", certificateOfInsurance);
                    command.Parameters.AddWithValue("?DateCompleted", dateCompleted);
                    command.Parameters.Add("?UploadedContractFile", MySqlDbType.LongBlob).Value = (rawDataContract == null) ? byteContract : rawDataContract;
                    command.Parameters.Add("?TribalCoverFile", MySqlDbType.LongBlob).Value = (rawDataTribal == null) ? byteTribal : rawDataTribal;
                    command.Parameters.AddWithValue("?Notes", notes);
                    command.Parameters.AddWithValue("?fileExtContract", fileExtContract);
                    command.Parameters.AddWithValue("?fileExtTribal", fileExtTribal);
                    command.ExecuteNonQuery();
                    //Update the table
                    command = new MySqlCommand("UPDATE updateTable SET updateValue = true WHERE ID = 1;", con);
                    command.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
                    
        }

        /// <summary>
        /// Insert a new attorney into the attorney table in the mySQL database
        /// </summary>
        /// <param name="name">The name of the attorney to be added</param>
        /// <param name="email">The email of the attorney to be added</param>
        public static void insertIntoAttorney(string name, string email)
        {
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                } catch
                {
                    return;
                }
                finally
                {
                    //Insert paramaters into the command
                    command = new MySqlCommand(insertAttorney, con);
                    command.Parameters.AddWithValue("?NAME", name);
                    command.Parameters.AddWithValue("?EMAIL", email);
                    command.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        /// <summary>
        /// Removes an attorney from the attorney table in the mySQL database
        /// </summary>
        /// <param name="name">The name of the attorney to be removed</param>
        /// <returns>Returns true if the attorney was removed otherwise returns false</returns>
        public static bool removeFromAttorney(string name)
        {
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                } catch
                {
                    return false;
                }
                command = new MySqlCommand(setSqlSafeUpdate, con);
                command.ExecuteNonQuery();

                command = new MySqlCommand(removeAttorney, con);
                command.Parameters.AddWithValue("?NAME", name);
                var reader = command.ExecuteNonQuery();
                con.Close();
                if (reader > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the email of the attorney with the name attorneyName
        /// </summary>
        /// <param name="attorneyName">The name of the attorney</param>
        /// <returns>The string of the email associated with the attorney</returns>
        public static string getEmailOfAttorney(string attorneyName)
        {
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                }
                catch
                {
                    return null;
                }

                command = new MySqlCommand(selectAttorneyByName, con);
                command.Parameters.AddWithValue("?NAME", attorneyName);

                var reader = command.ExecuteReader();
                reader.Read();
                con.Close();
                //Return the string of the email
                return reader[0].ToString();
            }
            
        }

        /// <summary>
        /// Removes a contract from the contract table based off of an ID number
        /// </summary>
        /// <param name="id">The id of the contract to be removed</param>
        /// <returns>Returns true if the contract was removed, false if it wasnt</returns>
        public static bool removeFromContracts(int id)
        {
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                } catch
                {
                    return false;
                }
                command = new MySqlCommand(setSqlSafeUpdate, con);
                command.ExecuteNonQuery();

                //Create a new command with the remove contract string
                command = new MySqlCommand(removeContract, con);
                command.Parameters.AddWithValue("?ID", id);

                var rowsAffected = command.ExecuteNonQuery();
                con.Close();
                //Should only ever remove 1 row
                if (rowsAffected == 1)
                {
                    return true;
                } else
                {
                    return false;
                }
                
            }
        }

        /// <summary>
        /// Updates the notes on a contract with id equal to the contracts id
        /// </summary>
        /// <param name="notes">The new notes to be updated</param>
        /// <param name="id">The id of the contract the notes came from</param>
        /// <returns>Returns true if the notes were updated succesfully otherwise return false</returns>
        public static bool updateNotes(string notes, int id)
        {
            using (mySQL.con)
            {
                try
                {
                    mySQL.con.Open();
                } catch
                {
                    return false;
                }

                command = new MySqlCommand(setSqlSafeUpdate, con);
                command.ExecuteNonQuery();

                command = new MySqlCommand(updateNotesString, con);
                command.Parameters.AddWithValue("?ID", id);
                command.Parameters.AddWithValue("?Notes", notes);
                var rowsAffected = command.ExecuteNonQuery();
                con.Close();
                if (rowsAffected == 1)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }

        public static bool CallProc()
        {
            using (con)
            {//Establish connection
                try
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    return false;
                }
                //Set up myCommand to reference stored procedure 'get_update'
                MySqlCommand myCommand = new MySqlCommand();
                myCommand.Connection = con;
                myCommand.CommandText = "get_update";
                myCommand.CommandType = System.Data.CommandType.StoredProcedure;

                //Create placeholder for return value
                myCommand.Parameters.AddWithValue("@ReturnVal", MySqlDbType.Int32);
                myCommand.Parameters["@ReturnVal"].Direction = System.Data.ParameterDirection.Output;

                //Execute the function. ReturnValue parameter receives result of the stored function
                myCommand.ExecuteNonQuery();
                con.Close();
                int tryCast;
                try
                {
                    tryCast = (int)myCommand.Parameters["@ReturnVal"].Value;
                }
                catch
                {
                    return false;
                }

                return tryCast == 1 ? true : false;
            }
        }
    }
}
