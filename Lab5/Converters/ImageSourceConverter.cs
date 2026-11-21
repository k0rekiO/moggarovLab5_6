using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Lab5.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        public static string ImagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string filename = value as string;
                if (string.IsNullOrWhiteSpace(filename))
                    return new BitmapImage();

                string fullPath = Path.Combine(ImagesPath, filename);

                if (!File.Exists(fullPath))
                    return new BitmapImage(); 
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(fullPath);
                image.EndInit();
                image.Freeze(); 
                return image;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}