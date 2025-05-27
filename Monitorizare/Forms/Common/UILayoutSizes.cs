namespace Monitorizare.Forms.Common;

public static class UILayoutSizes
{
    public static Dictionary<UITabNames, Size> GroupBoxSizes { get; } = new()
    {
        { UITabNames.Incarcare, new Size(110, 62) },
        { UITabNames.Descarcare, new Size(308, 62) }
    };

    public static Dictionary<UITabNames, Size> DataGridViewSizes { get; } = new()
    {
        { UITabNames.Incarcare, new Size(345, 330) },
        { UITabNames.Descarcare, new Size(475, 330) }
    };
}