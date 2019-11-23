using System;

namespace CommentScraperApp {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            CommentScraperForm commentScraperForm = new CommentScraperForm();
            commentScraperForm.ShowDialog();
        }
    }
}
