using Microsoft.Win32;
using PDFToolKit.Service;
using System.Windows;

namespace PdfToolkitApp
{
    public partial class MainWindow : Window
    {
        private string[] selectedFiles;
        private PdfMergeService mergeService = new PdfMergeService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFiles_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
            {
                selectedFiles = dlg.FileNames;
                MessageBox.Show($"{selectedFiles.Length} files selected.");
            }
        }

        private void MergeFiles_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFiles == null || selectedFiles.Length == 0)
            {
                MessageBox.Show("Please select PDF files first.");
                return;
            }

            var saveDlg = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = "Merged.pdf"
            };


            if (selectedFiles.Length > 1)
            {
                MessageBox.Show("Please select multiple PDF files.");
                return;
            }

            if (saveDlg.ShowDialog() == true)
            {
                bool success = mergeService.MergeFiles(selectedFiles, saveDlg.FileName);
                MessageBox.Show(success ? "PDFs merged successfully!" : "Merge failed.");
            }
        }
    }
}
