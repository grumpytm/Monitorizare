using ComponentFactory.Krypton.Toolkit;

namespace Monitorizare.Forms.Common;

public static class DataGridViewFilter
{
    public static IEnumerable<TransportContentDTO> FilterBy(this IEnumerable<TransportContentDTO> content, IEnumerable<KryptonComboBox> comboBoxes)
    {
        var predicates = new List<Func<TransportContentDTO, bool>>();
        foreach (var comboBox in comboBoxes)
        {
            var propName = comboBox.Name.Replace("ComboBox", string.Empty);
            if (!int.TryParse(comboBox.SelectedValue?.ToString(), out int parsedValue)) continue;
            predicates.Add(item => item.GetType().GetProperty(propName)?.GetValue(item) is int value && value == parsedValue);
        }

        return content.Where(item => predicates.All(predicate => predicate(item)));
    }

    public static IEnumerable<LogsDTO> FilterBy(this IEnumerable<LogsDTO> content, KryptonComboBox comboBox)
    {
        var parsedType = comboBox.SelectedValue?.ToString() ?? string.Empty;
        return Enum.GetNames(typeof(LogType)).Contains(parsedType) ? content.Where(item => item.Type == parsedType) : content;
    }
}