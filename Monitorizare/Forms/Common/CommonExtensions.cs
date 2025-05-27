namespace Monitorizare.Forms.Common;

public static class CommonExtensions
{
    public static void ChangeSizeBasedOn(this GroupBox groupBox, UITabNames tab) =>
        groupBox.Size = UILayoutSizes.GroupBoxSizes.GetValueOrDefault(tab, groupBox.Size);
}