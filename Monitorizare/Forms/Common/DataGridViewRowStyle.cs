namespace Monitorizare.Forms.Common;

public static class DataGridViewRowStyle
{
    public static class Color
    {
        public static readonly System.Drawing.Color Error = System.Drawing.Color.FromArgb(255, 252, 209, 215);
        public static readonly System.Drawing.Color Warning = System.Drawing.Color.FromArgb(255, 254, 252, 229);
        public static readonly System.Drawing.Color Selection = System.Drawing.Color.FromArgb(150, 180, 226, 244);
    }

    public static class Font
    {
        private static readonly System.Drawing.Font Base = SystemFonts.DefaultFont;
        public static readonly System.Drawing.Font Bold = new(Base, FontStyle.Bold);
        public static readonly System.Drawing.Font Regular = new(Base, FontStyle.Regular);
    }
}