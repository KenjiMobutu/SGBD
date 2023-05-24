using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System;

using PRBD_Framework;


namespace MyPoll.ViewModel;

public class TruncateTextConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string text && parameter is string lengthString && int.TryParse(lengthString, out int maxLength)) {
            if (text.Length > maxLength) {
                return text.Substring(0, maxLength) + "...";
            } else {
                return text;
            }
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException();
    }
}
