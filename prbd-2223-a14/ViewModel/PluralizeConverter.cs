using System;
using System.Globalization;
using System.Windows.Data;
using PRBD_Framework;

namespace MyPoll.ViewModel;
public class PluralizeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string text = parameter.ToString();
            int count = (int)value;
            if (count == 1) {
                return text;
            } else if (count == 0)
                return "";
            return  text + "s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
}
