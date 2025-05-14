using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ImageEditor.Helpers
{
    public static class NavigationHelper
    {
        public static void Navigate(UserControl newPage)
        {
            var window = Application.Current.MainWindow as Window;
            if (window != null)
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));

                fadeOut.Completed += (s, e) =>
                {
                    window.Content = newPage;
                    newPage.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };

                if (window.Content is UIElement oldContent)
                    oldContent.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                else
                    window.Content = newPage;
            }
        }
    }
}