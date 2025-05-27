using ComponentFactory.Krypton.Toolkit;

namespace Monitorizare.Forms.Common;

public static class DatePickerContent
{
    public static (long, long) ParseBounds(KryptonDateTimePicker minPicker, KryptonDateTimePicker maxPicker)
    {
        var (min, max) = (minPicker.Value.Date, maxPicker.Value.Date);
        if (min > max)
        {
            (min, max) = (max, min);
            (minPicker.Value, maxPicker.Value) = (min, max);
        }

        return (min.ToUnixTime(), max.ToUnixTime());
    }
}