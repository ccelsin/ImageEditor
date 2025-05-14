using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Services
{
    public static class ImageCropService
    {
        public static BitmapSource CropToCircle(ImageSource source, int diameter)
        {
            var drawingVisual = new DrawingVisual();
            using (DrawingContext ctx = drawingVisual.RenderOpen())
            {
                EllipseGeometry clip = new EllipseGeometry(new Point(diameter / 2, diameter / 2), diameter / 2, diameter / 2);
                ctx.PushClip(clip);
                ctx.DrawImage(source, new Rect(0, 0, diameter, diameter));
            }
            var bmp = new RenderTargetBitmap(diameter, diameter, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }
    }
}
