using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MR_LAW_DB
{
    /// <summary>
    /// Class that handles all of the buttons and text dealing with removing an
    /// attorney from the database on the main screen
    /// </summary>
    class RemoveAttorney
    {
        /// <summary>
        /// All of the UI elements needed to remove an attorney
        /// </summary>
        TextBox nameOfAttorney = null;
        Button submitRemoveAttorney = null;

        /// <summary>
        /// initializes all of the variables in the class to the refrences of the UI elements
        /// </summary>
        /// <param name="removeAttorney">The textblock that displays Remove Attorney on the Main screen</param>
        /// <param name="nameOf">The textBox for the name of the attorney</param>
        /// <param name="removeAttorneyButton">The button to click once you want to remove an attorney</param>
        public void initialize(ref TextBox nameOf, ref Button removeAttorneyButton)
        {
            nameOfAttorney = nameOf;
            submitRemoveAttorney = removeAttorneyButton;

            //Sets the textBoxs color to a semi transparent grey
            setColor();
        }

        /// <summary>
        /// Removes an attorney from the database
        /// </summary>
        public bool removeAttorney()
        {
            //if no name is enterd ask the user to enter a name
            if (nameOfAttorney.Text == "Name of Attorney")
            {
                MessageBox.Show("Please enter the name of the Attorney to be removed", "Name Required");
                return false;
            }

            //Remove the attorney using mySQL and the name of the attorney
            //if the removal is succesful returns true
            //else returns false
            if (mySQL.removeFromAttorney(nameOfAttorney.Text))
            {
                //Show that the attorney was succesfully removed and change the nameOfAttorney text back to the placeholder
                MessageBox.Show("Attorney was succesfully removed", "Removal Successful");
                nameOfAttorney.Text = "Name of Attorney";
                setColor();
                return true;
            }
            else
            {
                //Show an error to the user
                MessageBox.Show("Attorney was not able to be removed, please check to make sure the name is typed correctly", "Removal Failed");
                return false;
            }
        }

        /// <summary>
        /// Sets the text of the name of attorney text block to the place holder text
        /// </summary>
        public void setText()
        {
            //make sure it is empty, if so set to placeholder text
            if (nameOfAttorney.Text == "")
            {
                nameOfAttorney.Text = "Name of Attorney";
                setColor();
            }
        }

        /// <summary>
        /// Sets the text of the name of attorney to nothing if the placeholder text is there
        /// </summary>
        public void clearText()
        {
            //Check if the text is placeholder
            if (nameOfAttorney.Text == "Name of Attorney")
            {
                nameOfAttorney.Text = "";
                nameOfAttorney.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Sets the color to a semi transparent grey
        /// </summary>
        public void setColor()
        {
            nameOfAttorney.Foreground = new SolidColorBrush(Color.FromArgb(211, 190, 190, 190));
        }
    }
}
