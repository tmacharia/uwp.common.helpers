﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Common.Helpers.Converters
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                if (parameter != null)
                {
                    bool a = (bool)parameter;
                    return (bool)value == a ? Visibility.Visible : Visibility.Collapsed;
                }
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => value;
    }
}