namespace JiraAssistant.Domain.Ui
{
    public class ColorInfo
    {
        public byte Alpha = 255;
        public byte R;
        public byte G;
        public byte B;

        public static readonly ColorInfo White = new ColorInfo { Alpha = 255, R = 255, G = 255, B = 255 };
    }
}
