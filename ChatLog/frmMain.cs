using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;

namespace ChatLog
{
    public partial class frmMain : Form
    {
        protected XDocument folderNamesXml;

        public frmMain()
        {
            InitializeComponent();

            // Initialization
            contactListBox.DisplayMember = "DisplayName";
            folderNamesXml = XDocument.Load(Path.Combine(getTranscriptPath(), "personfolders.xml"));

            populateContactList(dateTimePicker1.Value);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
            populateContactList(dateTimePicker1.Value);
        }

        private void populateContactList(DateTime chatDate)
        {
            // Clear list
            contactListBox.Items.Clear();
            chatBrowser.Url = null;

            // Get Chat transcripts path
            string chatDir = getTranscriptPath();

            // Get date folder name
            string dateFolderName = getDateFolderName(chatDate);

            // Repopulate list
            List<string> contactDirs = new List<string>(Directory.EnumerateDirectories(chatDir));
            foreach (var contactDir in contactDirs)
            {
                string[] chatDates = Directory.GetDirectories(contactDir, dateFolderName + "*");
                if (chatDates.Length > 0)
                {
                    string contactFolder = Path.GetFileName(contactDir);
                    if ("[multi-way]".Equals(contactFolder)) // special logic for group chats
                    {
                        foreach (var multiChatFolder in chatDates)
                    	{
                            // For group chats get display name from folder name - xml file will just say "Multi-person chat"
                            contactListBox.Items.Add(new Chat(getMultiChatDisplayName(Path.GetFileName(multiChatFolder)), multiChatFolder, true));
                    	}
                    }
                    else
                    {
                        // look up display name for folder in xml file
                        IEnumerable<XElement> names =
                            from el in folderNamesXml.Descendants("folder")
                            where (string)el.Attribute("folderPath") == contactFolder
                            select el;
                        List<XElement> nameList = names.ToList();

                        contactListBox.Items.Add(new Chat(nameList[0].Attribute("displayName").Value, contactDir));
                    }
                }
            }
        }

        private void contactListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            Chat selectedChat = (Chat)contactListBox.SelectedItem;
            string dateFolderPath;

            if (selectedChat.IsMultiway)
            {
                dateFolderPath = selectedChat.Path;
            }
            else
            {
                // Get date folder name
                string dateFolderName = getDateFolderName(dateTimePicker1.Value);
                // Get date folder path
                dateFolderPath = Path.Combine(selectedChat.Path, dateFolderName);
            }

            // Look for an html file
            string[] htmlFiles = Directory.GetFiles(dateFolderPath, "*.html");

            // Display the first html file found
            if (htmlFiles.Length > 0)
                chatBrowser.Url = new Uri(@"file://" + htmlFiles[0]);
        }

        private string getTranscriptPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                "SametimeTranscripts");
        }

        private string getDateFolderName(DateTime chatDate)
        {
            return chatDate.Date.ToString("yyyyMMdd");
        }

        private string getMultiChatDisplayName(string folderName)
        {
            // get text between curly braces
            int index = folderName.IndexOf('{');
            if (index == 0)
                return folderName;

            index++; // We want the substring inside the braces without including the braces
            string textInsideBraces = folderName.Substring(index, folderName.IndexOf('}') - index);

            // Replace encoded slashes
            return textInsideBraces.Replace("x2fx", "/");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
