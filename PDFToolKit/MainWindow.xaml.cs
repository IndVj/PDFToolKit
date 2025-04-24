using Microsoft.Win32;
using PDFToolKit.Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PdfToolkitApp
{
    public partial class MainWindow : Window
    {
        private readonly PdfMergeService mergeService = new PdfMergeService();
        public ObservableCollection<string> PdfFiles { get; set; } = new();

        private Point _dragStartPoint;
        private object _draggedItem;

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
                    if (!PdfFiles.Contains(file))
                        PdfFiles.Add(file);
                }
            }
        }

        private void MergeFiles_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = PdfFiles.ToArray();

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

        private void PdfListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void PdfListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (System.IO.Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        // Assuming PdfFiles is an ObservableCollection<string>
                        if (!PdfFiles.Contains(file))
                            PdfFiles.Add(file);
                    }
                }
            }
        }

        private void PdfListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            _draggedItem = (sender as ListBox).SelectedItem;
        }

        private void PdfListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = _dragStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (_draggedItem != null)
                {
                    DragDrop.DoDragDrop(PdfListBox, _draggedItem, DragDropEffects.Move);
                }
            }
        }

        private void PdfListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void PdfListBox_DropForReorder(object sender, DragEventArgs e)
        {
            if (_draggedItem == null) return;

            var droppedData = _draggedItem;
            var target = ((FrameworkElement)e.OriginalSource).DataContext;

            if (target == null || target == droppedData) return;

            int removedIdx = PdfFiles.IndexOf(droppedData.ToString());
            int targetIdx = PdfFiles.IndexOf(target.ToString());

            if (removedIdx >= 0 && targetIdx >= 0)
            {
                PdfFiles.Move(removedIdx, targetIdx);
            }
        }

        private void MoveItemUp_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as string;
            if (item != null)
            {
                int index = PdfFiles.IndexOf(item);
                if (index > 0)
                {
                    PdfFiles.Move(index, index - 1);
                }
            }
        }

        private void MoveItemDown_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as string;
            if (item != null)
            {
                int index = PdfFiles.IndexOf(item);
                if (index < PdfFiles.Count - 1)
                {
                    PdfFiles.Move(index, index + 1);
                }
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as string;
            if (item != null && PdfFiles.Contains(item))
            {
                PdfFiles.Remove(item);
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
