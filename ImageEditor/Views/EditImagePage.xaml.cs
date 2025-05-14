using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using ImageEditor.Services;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace ImageEditor.Views
{
    public partial class EditImagePage : UserControl
    {
        private BitmapImage originalImage;
        private WriteableBitmap editableBitmap;


        public EditImagePage(BitmapImage image)
        {
            InitializeComponent();
            originalImage = image;
            editableBitmap = new WriteableBitmap(originalImage);
            EditableImage.Source = editableBitmap;
        }


        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ZoomTransform.ScaleX = ZoomTransform.ScaleY = 1;
            RotateTransform.Angle = 0;
            TranslateTransform.X = TranslateTransform.Y = 0;
            editableBitmap = new WriteableBitmap(originalImage);
            EditableImage.Source = editableBitmap;
            OpacitySlider.Value = 1;
            SaturationSlider.Value = 1;

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var cropped = ImageCropService.CropToCircle(EditableImage.Source, 300);
            var dialog = new SaveFileDialog { Filter = "PNG Image|*.png" };
            if (dialog.ShowDialog() == true)
            {
                using (var stream = new System.IO.FileStream(dialog.FileName, System.IO.FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(cropped));
                    encoder.Save(stream);
                }
            }
        }


        private BitmapSource AdjustSaturation(BitmapSource source, double saturation)

        {
            WriteableBitmap wb = new WriteableBitmap(source);
            int width = wb.PixelWidth;
            int height = wb.PixelHeight;
            int stride = width * ((wb.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[height * stride];
            wb.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                // Convert to grayscale intensity (luminance)
                double gray = r * 0.299 + g * 0.587 + b * 0.114;

                // Interpolate between grayscale and original based on saturation
                pixels[i + 2] = (byte)Math.Clamp(gray + (r - gray) * saturation, 0, 255); // R
                pixels[i + 1] = (byte)Math.Clamp(gray + (g - gray) * saturation, 0, 255); // G
                pixels[i] = (byte)Math.Clamp(gray + (b - gray) * saturation, 0, 255); // B
                                                                                      // Alpha (pixels[i + 3]) reste inchangé
            }

            WriteableBitmap result = new WriteableBitmap(width, height, source.DpiX, source.DpiY, PixelFormats.Bgra32, null);
            result.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return result;
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (EditableImage != null)
            {
                EditableImage.Opacity = e.NewValue;
            }
        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (EditableImage.Source is BitmapImage)
            {
                EditableImage.Source = AdjustSaturation(editableBitmap, e.NewValue);

            }
        }



    }
}