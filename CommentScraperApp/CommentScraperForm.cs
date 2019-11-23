using System;
using System.Windows.Forms;
using System.Drawing;


namespace CommentScraperApp {
    class CommentScraperForm : Form {
        public const int FormHeight = 400;
        public const int FormWidth = 400;

        private readonly TextBox inputPathTextBox = new TextBox();
        private readonly TextBox outputPathTextBox = new TextBox();
        private readonly TextBox statusTextBox = new TextBox();
        private SettingsForm settingsForm = new SettingsForm();

        public CommentScraperForm() {
            FormInit();
        }

        private void FormInit() {
            this.SetBounds(0,0, FormWidth, FormHeight);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Text = "Comment Scraper";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Input Group
            GroupBox inputGroupBox = new GroupBox();
            inputGroupBox.Size = new Size(this.Width - 40, 60);
            inputGroupBox.Location = new Point(this.Width / 2 - inputGroupBox.Width / 2, 10);
            inputGroupBox.Text = "Input Folder";
            this.Controls.Add(inputGroupBox);

            Button selectInputPathButton = new Button();
            selectInputPathButton.Text = "Browse";
            selectInputPathButton.MouseClick += new MouseEventHandler(this.OnInputPathSelected);
            selectInputPathButton.Location = new Point(5, 20);
            inputGroupBox.Controls.Add(selectInputPathButton);

            inputPathTextBox.Location = new Point(selectInputPathButton.Location.X + selectInputPathButton.Width + 5, selectInputPathButton.Location.Y);
            inputPathTextBox.Width = inputGroupBox.Width - inputPathTextBox.Location.X - 5;
            inputGroupBox.Controls.Add(inputPathTextBox);

            // Output Group
            GroupBox outputGroupBox = new GroupBox();
            outputGroupBox.Size = new Size(this.Width - 40, 60);
            outputGroupBox.Location = new Point(this.Width / 2 - outputGroupBox.Width / 2, inputGroupBox.Location.Y + inputGroupBox.Height + 30);
            outputGroupBox.Text = "Output Location";
            this.Controls.Add(outputGroupBox);

            Button selectOutputPathButton = new Button();
            selectOutputPathButton.Text = "Browse";
            selectOutputPathButton.Location = new Point(5, 20);
            selectOutputPathButton.MouseClick += new MouseEventHandler(this.OnOutputPathSelected);
            outputGroupBox.Controls.Add(selectOutputPathButton);

            outputPathTextBox.Location = new Point(selectOutputPathButton.Location.X + selectOutputPathButton.Width + 5, selectOutputPathButton.Location.Y);
            outputPathTextBox.Width = outputGroupBox.Width - outputPathTextBox.Location.X - 5;
            outputGroupBox.Controls.Add(outputPathTextBox);

            // Scrape Button
            Button scrapeButton = new Button();
            scrapeButton.Text = "Scrape";
            scrapeButton.Location = new Point(outputGroupBox.Location.X, outputGroupBox.Location.Y + outputGroupBox.Height + 20);
            scrapeButton.MouseClick += new MouseEventHandler(this.OnScrapeButton);
            this.Controls.Add(scrapeButton);

            // Settings Button
            Button settingsButton = new Button();
            settingsButton.Text = "Settings";
            settingsButton.Location = new Point(scrapeButton.Location.X + scrapeButton.Width + 10, scrapeButton.Location.Y);
            settingsButton.MouseClick += new MouseEventHandler(this.OnSettingsButton);
            this.Controls.Add(settingsButton);

            // Status Text Box
            statusTextBox.Multiline = true;
            statusTextBox.Enabled = false;
            statusTextBox.WordWrap = true;
            statusTextBox.Location = new Point(scrapeButton.Location.X, scrapeButton.Location.Y + scrapeButton.Height + 20);
            statusTextBox.Size = new Size(this.Width - statusTextBox.Location.X * 2, 120);
            this.Controls.Add(statusTextBox);
        }

        private void OnInputPathSelected(object sender, MouseEventArgs e) {
            FolderBrowserDialog folderBrowseDialog = new FolderBrowserDialog();
            if (folderBrowseDialog.ShowDialog() == DialogResult.OK)
                inputPathTextBox.Text = folderBrowseDialog.SelectedPath;
            else
                statusTextBox.Text = "Folder was not selected";
        }
        private void OnOutputPathSelected(object sender, MouseEventArgs e) {
            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.FileName = "out.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                outputPathTextBox.Text = openFileDialog.FileName;
            else
                statusTextBox.Text = "File was not selected";
        }
        private void OnScrapeButton(object sender, MouseEventArgs e) {
            statusTextBox.Text = "";
            try {          
                CommentScraper.Scrape(inputPathTextBox.Text, outputPathTextBox.Text, settingsForm.GetDelimInfos());
                statusTextBox.Text = "Scrape successful";
            }catch (Exception err) {
                statusTextBox.Text = err.Message;
            }
        }

        private void OnSettingsButton(object sender, MouseEventArgs e) {
            settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

    }
}
