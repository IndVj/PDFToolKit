using Microsoft.Win32;
using PDFToolKit.Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PdfToolkitApp
{
    public partial class MainWindow : Window
    {
        private readonly string[] selectedFiles;

        private readonly PdfMergeService mergeService = new PdfMergeService();
        public ObservableCollection<string> PdfFiles { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; 
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
                foreach (var file in dlg.FileNames)
                {
                    if (!PdfFiles.Contains(file)) PdfFiles.Add(file);
                }
            }
        }

        private void MergeFiles_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFiles == null || selectedFiles.Length < 2)
            {
                MessageBox.Show("Please select at least two PDF files to merge.");
                return;
            }

            var saveDlg = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = "Merged.pdf"
            };

            if (saveDlg.ShowDialog() == true)
            {
                bool success = mergeService.MergeFiles(selectedFiles, saveDlg.FileName);
                MessageBox.Show(success ? "PDFs merged successfully!" : "Merge failed.");
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = PdfListBox.SelectedIndex;
            if (selectedIndex > 0)
            {
                var item = PdfFiles[selectedIndex];
                PdfFiles.RemoveAt(selectedIndex);
                PdfFiles.Insert(selectedIndex - 1, item);
                PdfListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = PdfListBox.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < PdfFiles.Count - 1)
            {
                var item = PdfFiles[selectedIndex];
                PdfFiles.RemoveAt(selectedIndex);
                PdfFiles.Insert(selectedIndex + 1, item);
                PdfListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = PdfListBox.SelectedItem as string;
            if (selectedItem != null)
            {
                PdfFiles.Remove(selectedItem);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
