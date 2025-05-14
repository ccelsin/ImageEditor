using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageEditor.Views;
using ImageEditor.Helpers;

namespace ImageEditor
{
    public partial class MainWindow : Window // Retirer IComponentConnector
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseImage(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif" };
            if (dialog.ShowDialog() == true)
                LoadImage(dialog.FileName);
        }

        private void ImageDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void DropImage(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                    LoadImage(files[0]);
            }
        }

        private void LoadImage(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage(new Uri(filePath));
                ImagePreview.Source = bitmap;
                ImagePreview.Visibility = Visibility.Visible;
                NavigationHelper.Navigate(new EditImagePage(bitmap));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }
    }
}