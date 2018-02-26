using System;
using Microsoft.Office.Interop.Word;
using System.IO;

namespace MR_LAW_DB
{
    /// <summary>
    /// Class to create a word document from a contract
    /// </summary>
    class wordDocument
    {
        static Application winWord;

        /// <summary>
        /// Creates a very simple word document
        /// </summary>
        /// <param name="nameOfContract"></param>
        /// <param name="dateRecieved"></param>
        /// <param name="dateCompleted"></param>
        /// <param name="attorney"></param>
        /// <param name="notes"></param>
        /// <param name="uploadedContract"></param>
        /// <param name="extContract"></param>
        /// <param name="tribalCover"></param>
        /// <param name="extTribal"></param>
        public static void fillWordDocument(string nameOfContract, string dateRecieved, string dateCompleted, string attorney, string notes, byte[] uploadedContract, string extContract, byte[] tribalCover, string extTribal)
        {
            winWord = new Application();
            winWord.Visible = false;

            //Create new document
            Document document = winWord.Documents.Add();

            Paragraph para = document.Content.Paragraphs.Add();
            para.Range.Text = nameOfContract;
            para.Range.Font.Bold = 1;
            para.Range.Font.Size = 32;
            para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            para.Range.InsertParagraphAfter();

            para = document.Content.Paragraphs.Add();
            para.Range.Text = "Date Received: " + dateRecieved + "     Date Completed: " + dateCompleted + "\v\v"
                + "Assigned to: " + attorney + "\v\v" + "Notes: " + notes;
            para.Range.Font.Size = 18;
            para.Range.Font.Bold = 0;
            para.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            para.Range.InsertParagraphAfter();

            para = document.Content.Paragraphs.Add();
            para.Range.InsertParagraphBefore();
            if (uploadedContract != null)
            {
                para.Range.Hyperlinks.Add(para.Range, convertToFile(uploadedContract, extContract, nameOfContract, false), null, null, "Uploaded Contract");
            }
            para.Range.Font.Size = 18;

            para.Range.InsertParagraphAfter();
            if (tribalCover != null)
            {
                para.Range.Hyperlinks.Add(para.Range, convertToFile(tribalCover, extTribal, nameOfContract, true), null, null, "Uploaded Tribal Cover Sheet");
            }

            para.Range.Font.Size = 18;
            winWord.Visible = true;

        
        }

        /// <summary>
        /// Converts byte arrays to the corresponding file
        /// </summary>
        /// <param name="file">The byte array</param>
        /// <param name="ext">the file exstention</param>
        /// <param name="name">The name of the contract</param>
        /// <param name="tribalCoverSheet">Whether it is a tribal sheet or contract file</param>
        /// <returns></returns>
        private static string convertToFile(byte[] file, string ext, string name, bool tribalCoverSheet)
        {
            Directory.CreateDirectory(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts")));
            string path;
            if (tribalCoverSheet)
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts\\" + name + "CoverSheet") + ext;
            else
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Contracts\\" + name + "UploadedContract") + ext;
            //Create a stream to write to the file
            using (Stream docStream = File.OpenWrite(path))
            {
                docStream.Write(file, 0, file.Length);
                docStream.Close();
            }

            if (File.Exists(path))
            {
                return path;
            }

            return null;
        }
    }
}
