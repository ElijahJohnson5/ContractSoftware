using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MR_LAW_DB
{
    /// <summary>
    /// Class which handles anything to do with adding an attorney to the database
    /// in the main window
    /// </summary>
    class AddAttorney
    {
        //UI elements that are need to keep track of
        
        TextBox nameOfAttorney = null;
        TextBox emailOfAttorney = null;
        Button submitNewAttorney = null;

        /// <summary>
        /// Gets the refrences for the ui elements and sets them to
        /// their corresponding variable
        /// </summary>
        /// <param name="addAttorney">The textblock which says adda attorney</param>
        /// <param name="nameOf">The textbox which contains the name of the attorney to be added</param>
        /// <param name="emailOf">The textbox which contains the email of the attorney to be added</param>
        /// <param name="submit">The button to submit a new attorney</param>
        public void initialize(ref TextBox nameOf, ref TextBox emailOf, ref Button submit)
        {
            nameOfAttorney = nameOf;
            emailOfAttorney = emailOf;
            submitNewAttorney = submit;

            //Set the placeholder text to a transparent grey
            setColorTransparent(true);
            setColorTransparent(false);
        }

        /// <summary>
        /// Adds an attorney into the attorney database
        /// </summary>
        public bool addAttorney()
        {
            //If there is no name entered ask for a name of the attorney
            //and exit the function
            if (nameOfAttorney.Text == "Name of Attorney")
            {
                MessageBox.Show("Enter a name for the Attorney", "Name Required");
                return false;
            }

            //Insert into the attorney database with the correct information
            mySQL.insertIntoAttorney(nameOfAttorney.Text, (emailOfAttorney.Text == "Email Address") ? null : emailOfAttorney.Text);

            //Show that the attorney was succesfully added into the database
            MessageBox.Show("Attorney " + nameOfAttorney.Text + " successfully added", "Attorney Added");

            //Reset the name of attorney and email of attorney text boxes
            nameOfAttorney.Text = "Name of Attorney";
            setColorTransparent(true);
            emailOfAttorney.Text = "Email Address";
            setColorTransparent(false);
            return true;
        }

        /// <summary>
        /// Sets the text back to the placeholder text when clicked away from
        /// </summary>
        /// <param name="nameEmail">True if it is the name of the attorney false if it is the email</param>
        public void setText(bool nameEmail)
        {
            //Check to make sure the text field is empty and then set back to the placeholder text same for email
            if (nameOfAttorney.Text == "" && nameEmail == true)
            {
                nameOfAttorney.Text = "Name of Attorney";
                setColorTransparent(nameEmail);
            } else if (emailOfAttorney.Text == "" && nameEmail == false) 
            {
                emailOfAttorney.Text = "Email Address";
                setColorTransparent(nameEmail);
            }
        }

        /// <summary>
        /// Clears the text to nothing when the name or email boxes are clicked on
        /// </summary>
        /// <param name="nameEmail">True if it is the name of the attorney false if it is the email</param>
        public void clearText(bool nameEmail)
        {
            //If the text is the placeholder text clear it and turn the color to black, same for email
            if (nameOfAttorney.Text == "Name of Attorney" && nameEmail == true)
            {
                nameOfAttorney.Text = "";
                nameOfAttorney.Foreground = Brushes.Black;
            } else if (emailOfAttorney.Text == "Email Address" && nameEmail == false)
            {
                emailOfAttorney.Text = "";
                emailOfAttorney.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Sets the placeholder text color to a semi transparent grey
        /// </summary>
        /// <param name="nameEmail">True if it is the name false if it is the email</param>
        public void setColorTransparent(bool nameEmail)
        {
            if (nameEmail)
            {
                nameOfAttorney.Foreground = new SolidColorBrush(Color.FromArgb(211, 190, 190, 190));
            } else
            {
                emailOfAttorney.Foreground = new SolidColorBrush(Color.FromArgb(211, 190, 190, 190));
            }
        }
    }
}
