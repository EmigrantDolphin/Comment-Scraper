using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommentScraperApp {
    class SettingsForm : Form{
        public const int FormHeight = 400;
        public const int FormWidth = 400;

        private const string SettingsFilePath = @"settings.txt";

        private readonly TextBox fileEndingsTextBox = new TextBox();
        private readonly TextBox lineDelimiterTextBox = new TextBox();
        private readonly TextBox startDelimiterTextBox = new TextBox();
        private readonly TextBox endDelimiterTextBox = new TextBox();
        private readonly TextBox ignoreLineDelimiterTextBox = new TextBox();
        private readonly TextBox ignoreStartDelimiterTextBox = new TextBox();
        private readonly TextBox ignoreEndDelimiterTextBox = new TextBox();
        private readonly TextBox specialCharacterDelimiterTextBox = new TextBox();
        private readonly ComboBox fileEndingsComboBox = new ComboBox();
        private readonly TextBox statusTextBox = new TextBox();

        private readonly Button addButton = new Button();
        private const char openDelim = '{';
        private const char closeDelim = '}';

        List<CommentScraper.DelimiterInfo> delimInfos = new List<CommentScraper.DelimiterInfo>();


        public SettingsForm() {
            FormInit();
            LoadSavedSettings();
        }

        private void FormInit() {
            this.SetBounds(0, 0, FormWidth, FormHeight);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = "Settings";
            this.MaximizeBox = false;
            this.MinimizeBox = false;


            fileEndingsComboBox.Location = new Point(5, 10);
            fileEndingsComboBox.MaxDropDownItems = 5;
            fileEndingsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            fileEndingsComboBox.SelectedIndexChanged += new EventHandler(this.FileEndingsComboBoxChanged);
            this.Controls.Add(fileEndingsComboBox);

            fileEndingsTextBox.Location = new Point(5, fileEndingsComboBox.Location.Y + fileEndingsComboBox.Height + 10);
            Label fileEndingsLabel = new Label();
            fileEndingsLabel.Text = "File Endings";
            fileEndingsLabel.Location = new Point(fileEndingsTextBox.Location.X + fileEndingsTextBox.Width + 10, fileEndingsTextBox.Location.Y);
            fileEndingsLabel.Width = this.Width;
            this.Controls.Add(fileEndingsTextBox);
            this.Controls.Add(fileEndingsLabel);

            lineDelimiterTextBox.Location = new Point(fileEndingsTextBox.Location.X, fileEndingsTextBox.Location.Y + fileEndingsTextBox.Height + 10);
            Label lineDelimiterLabel = new Label();
            lineDelimiterLabel.Text = "Line comment delimiters";
            lineDelimiterLabel.Location = new Point(lineDelimiterTextBox.Location.X + lineDelimiterTextBox.Width + 10, lineDelimiterTextBox.Location.Y);
            lineDelimiterLabel.Width = this.Width;
            this.Controls.Add(lineDelimiterTextBox);
            this.Controls.Add(lineDelimiterLabel);

            startDelimiterTextBox.Location = new Point(lineDelimiterTextBox.Location.X, lineDelimiterTextBox.Location.Y + lineDelimiterTextBox.Height + 10);
            Label startDelimiterLabel = new Label();
            startDelimiterLabel.Text = "Multi-line comment starting delimiters";
            startDelimiterLabel.Location = new Point(startDelimiterTextBox.Location.X + startDelimiterTextBox.Width + 10, startDelimiterTextBox.Location.Y);
            startDelimiterLabel.Width = this.Width;
            this.Controls.Add(startDelimiterTextBox);
            this.Controls.Add(startDelimiterLabel);

            endDelimiterTextBox.Location = new Point(startDelimiterTextBox.Location.X, startDelimiterTextBox.Location.Y + startDelimiterTextBox.Height + 10);
            Label endDelimiterLabel = new Label();
            endDelimiterLabel.Text = "Multi-line comment ending delimiters";
            endDelimiterLabel.Location = new Point(endDelimiterTextBox.Location.X + endDelimiterTextBox.Width + 10, endDelimiterTextBox.Location.Y);
            endDelimiterLabel.Width = this.Width;
            this.Controls.Add(endDelimiterTextBox);
            this.Controls.Add(endDelimiterLabel);

            ignoreLineDelimiterTextBox.Location = new Point(endDelimiterTextBox.Location.X, endDelimiterTextBox.Location.Y + endDelimiterTextBox.Height + 10);
            Label lineIgnoreDelimiterLabel = new Label();
            lineIgnoreDelimiterLabel.Text = "Comment ignoring line delimiters";
            lineIgnoreDelimiterLabel.Location = new Point(ignoreLineDelimiterTextBox.Location.X + ignoreLineDelimiterTextBox.Width + 10, ignoreLineDelimiterTextBox.Location.Y);
            lineIgnoreDelimiterLabel.Width = this.Width;
            this.Controls.Add(ignoreLineDelimiterTextBox);
            this.Controls.Add(lineIgnoreDelimiterLabel);

            ignoreStartDelimiterTextBox.Location = new Point(ignoreLineDelimiterTextBox.Location.X, ignoreLineDelimiterTextBox.Location.Y + ignoreLineDelimiterTextBox.Height + 10);
            Label startIgnoreDelimiterLabel = new Label();
            startIgnoreDelimiterLabel.Text = "Comment ignoring start delimiters";
            startIgnoreDelimiterLabel.Location = new Point(ignoreStartDelimiterTextBox.Location.X + ignoreStartDelimiterTextBox.Width + 10, ignoreStartDelimiterTextBox.Location.Y);
            startIgnoreDelimiterLabel.Width = this.Width;
            this.Controls.Add(ignoreStartDelimiterTextBox);
            this.Controls.Add(startIgnoreDelimiterLabel);

            ignoreEndDelimiterTextBox.Location = new Point(ignoreStartDelimiterTextBox.Location.X, ignoreStartDelimiterTextBox.Location.Y + ignoreStartDelimiterTextBox.Height + 10);
            Label endIgnoreDelimiterLabel = new Label();
            endIgnoreDelimiterLabel.Text = "Comment ignoring end delimiters";
            endIgnoreDelimiterLabel.Location = new Point(ignoreEndDelimiterTextBox.Location.X + ignoreEndDelimiterTextBox.Width + 10, ignoreEndDelimiterTextBox.Location.Y);
            endIgnoreDelimiterLabel.Width = this.Width;
            this.Controls.Add(ignoreEndDelimiterTextBox);
            this.Controls.Add(endIgnoreDelimiterLabel);

            specialCharacterDelimiterTextBox.Location = new Point(ignoreEndDelimiterTextBox.Location.X, ignoreEndDelimiterTextBox.Location.Y + ignoreEndDelimiterTextBox.Height + 10);
            Label specialCharacterDelimiterLabel = new Label();
            specialCharacterDelimiterLabel.Text = "Special character delimiters";
            specialCharacterDelimiterLabel.Location = new Point(specialCharacterDelimiterTextBox.Location.X + specialCharacterDelimiterTextBox.Width + 10, specialCharacterDelimiterTextBox.Location.Y);
            specialCharacterDelimiterLabel.Width = this.Width;
            this.Controls.Add(specialCharacterDelimiterTextBox);
            this.Controls.Add(specialCharacterDelimiterLabel);

            addButton.Text = "Add";
            addButton.MouseClick += new MouseEventHandler(this.OnAddButton);
            addButton.Location =  new Point(specialCharacterDelimiterTextBox.Location.X, specialCharacterDelimiterTextBox.Location.Y + specialCharacterDelimiterTextBox.Height + 10);
            this.Controls.Add(addButton);

            Button removeButton = new Button();
            removeButton.Text = "Delete";
            removeButton.Width = removeButton.Width + 2;
            removeButton.MouseClick += new MouseEventHandler(this.OnRemoveButton);
            removeButton.Location = new Point(addButton.Location.X + addButton.Width + 10, addButton.Location.Y);
            this.Controls.Add(removeButton);

            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.MouseClick += new MouseEventHandler(this.OnSaveButton);
            saveButton.Location = new Point(removeButton.Location.X + removeButton.Width + 10, removeButton.Location.Y);
            this.Controls.Add(saveButton);

            statusTextBox.Multiline = true;
            statusTextBox.WordWrap = true;
            statusTextBox.Enabled = false;
            statusTextBox.Location = new Point(addButton.Location.X, addButton.Location.Y + addButton.Height + 10);
            statusTextBox.Size = new Size(this.Width - statusTextBox.Location.X * 2, 60);
            this.Controls.Add(statusTextBox);
        }

        private void OnAddButton(object sender, MouseEventArgs e) {
            CommentScraper.DelimiterInfo delimInfo = new CommentScraper.DelimiterInfo();
            try {
                delimInfo.fileEndings = ParseStringBy(fileEndingsTextBox.Text, openDelim, closeDelim);
                if (delimInfo.fileEndings.Length == 0)
                    throw new Exception("You must have at least one file ending");
                delimInfo.lineDelimiters = ParseStringBy(lineDelimiterTextBox.Text, openDelim, closeDelim);
                delimInfo.startDelimiters = ParseStringBy(startDelimiterTextBox.Text, openDelim, closeDelim);
                delimInfo.endDelimiters = ParseStringBy(endDelimiterTextBox.Text, openDelim, closeDelim);
                delimInfo.ignoreLineDelimiters = ParseStringBy(ignoreLineDelimiterTextBox.Text, openDelim, closeDelim);
                delimInfo.startIgnoreDelimiters = ParseStringBy(ignoreStartDelimiterTextBox.Text, openDelim, closeDelim);
                delimInfo.endIgnoreDelimiters = ParseStringBy(ignoreEndDelimiterTextBox.Text, openDelim, closeDelim);
                delimInfo.specialCharacterDelimiters = ParseStringBy(specialCharacterDelimiterTextBox.Text, openDelim, closeDelim);

                if ((string)fileEndingsComboBox.SelectedItem == "New") {
                    fileEndingsComboBox.Items.Add(EncloseInDelims(delimInfo.fileEndings, openDelim, closeDelim));
                    delimInfos.Add(delimInfo);
                    fileEndingsComboBox.SelectedIndex = fileEndingsComboBox.Items.Count - 1;
                    statusTextBox.Text = "Added successfully";
                } else {
                    delimInfos[fileEndingsComboBox.SelectedIndex - 1] = delimInfo;
                    statusTextBox.Text = "Updated successfully";
                }
                
            } catch(Exception err) {
                statusTextBox.Text = err.Message;
            }   
        }

        private void OnRemoveButton(object sender, MouseEventArgs e) {
            if ((string)fileEndingsComboBox.SelectedItem != "New") {
                delimInfos.RemoveAt(fileEndingsComboBox.SelectedIndex - 1);
                fileEndingsComboBox.Items.RemoveAt(fileEndingsComboBox.SelectedIndex);
                fileEndingsComboBox.SelectedIndex = 0;
            } else
                statusTextBox.Text = "No entry selected";
        }

        private void OnSaveButton(object sender, MouseEventArgs e) {
            if (File.Exists(SettingsFilePath)) {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, delimInfos.ToArray());
                stream.Close();
            }
            this.Close();
        }

        private void FileEndingsComboBoxChanged(object sender, EventArgs e) {
            statusTextBox.Text = "";
            if ((string)fileEndingsComboBox.SelectedItem == "New") {
                addButton.Text = "Add";
                fileEndingsTextBox.Text = "";
                lineDelimiterTextBox.Text = "";
                startDelimiterTextBox.Text = "";
                endDelimiterTextBox.Text = "";
                ignoreLineDelimiterTextBox.Text = "";
                ignoreStartDelimiterTextBox.Text = "";
                ignoreEndDelimiterTextBox.Text = "";
                specialCharacterDelimiterTextBox.Text = "";
            } else {
                addButton.Text = "Update";

                CommentScraper.DelimiterInfo delimInfo = delimInfos[fileEndingsComboBox.SelectedIndex - 1];

                fileEndingsTextBox.Text = EncloseInDelims(delimInfo.fileEndings, openDelim, closeDelim);
                lineDelimiterTextBox.Text = EncloseInDelims(delimInfo.lineDelimiters, openDelim, closeDelim);
                startDelimiterTextBox.Text = EncloseInDelims(delimInfo.startDelimiters, openDelim, closeDelim);
                endDelimiterTextBox.Text = EncloseInDelims(delimInfo.endDelimiters, openDelim, closeDelim);
                ignoreLineDelimiterTextBox.Text = EncloseInDelims(delimInfo.ignoreLineDelimiters, openDelim, closeDelim);
                ignoreStartDelimiterTextBox.Text = EncloseInDelims(delimInfo.startIgnoreDelimiters, openDelim, closeDelim);
                ignoreEndDelimiterTextBox.Text = EncloseInDelims(delimInfo.endIgnoreDelimiters, openDelim, closeDelim);
                specialCharacterDelimiterTextBox.Text = EncloseInDelims(delimInfo.specialCharacterDelimiters, openDelim, closeDelim);
            }
        }

        private string[] ParseStringBy(string line, char openDelim, char closeDelim) {
            bool isOpen = false;
            List<string> parts = new List<string>();
            string buffString = "";

            for (int i = 0; i < line.Length; i++)
                if (isOpen) {
                    if (line[i] == closeDelim) {
                        isOpen = false;
                        if (buffString != "")
                            parts.Add(buffString);
                        buffString = "";
                        continue;
                    }
                    buffString += line[i].ToString();
                } else {
                    if (line[i] == openDelim) {
                        isOpen = true;
                        continue;
                    }
                }
            if (isOpen)
                throw new Exception("Surround delimiters by " + openDelim + " " + closeDelim);
            return parts.ToArray();
        }

        private string EncloseInDelims(string[] items, char openDelim, char closeDelim) {
            string line = "";

            foreach (var item in items)
                line += openDelim + item + closeDelim;

            return line;
        }

        private void LoadSavedSettings() {
            fileEndingsComboBox.Items.Add("New");

            if (!File.Exists(SettingsFilePath)) {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, CommentScraper.DefaultDelimiterInfos.GetDelimiterInfos());
                stream.Close();
            }

            if (File.Exists(SettingsFilePath)) {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(SettingsFilePath, FileMode.Open, FileAccess.Read);
                foreach (var delimInfo in (CommentScraper.DelimiterInfo[])formatter.Deserialize(stream)) {
                    delimInfos.Add(delimInfo);
                    fileEndingsComboBox.Items.Add(EncloseInDelims(delimInfo.fileEndings, openDelim, closeDelim));
                }
                stream.Close();

            }
            
            if (fileEndingsComboBox.Items.Count <= 1)
                fileEndingsComboBox.SelectedItem = fileEndingsComboBox.Items[0];
            else
                fileEndingsComboBox.SelectedItem = fileEndingsComboBox.Items[1];
        }

        public CommentScraper.DelimiterInfo[] GetDelimInfos() {
            return delimInfos.ToArray();
        }

    }
}
