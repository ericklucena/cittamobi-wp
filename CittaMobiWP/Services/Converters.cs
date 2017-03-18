using CittaMobiWP.Models;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CittaMobiWP.Services
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value != null)
            {
                var i = (Int32)value;

                if (i > 0)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return new NotImplementedException();
        }
    }

    public class TypeStringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value != null)
            {
                string type = (string) value;

                if (type.Equals(Vehicle.TYPE_MESSAGE, StringComparison.OrdinalIgnoreCase))
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else if (type.Equals(Vehicle.TYPE_SCHEDULE, StringComparison.OrdinalIgnoreCase))
                {
                    return new SolidColorBrush(Colors.Gray);
                }
            }
            return (SolidColorBrush) Application.Current.Resources["PhoneAccentColor"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return new NotImplementedException();
        }
    }

}