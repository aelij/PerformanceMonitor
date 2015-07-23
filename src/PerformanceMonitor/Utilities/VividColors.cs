using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PerformanceMonitor.Utilities
{
    public static class VividColors
    {
        private static readonly ReadOnlyCollection<Color> _colors = new ReadOnlyCollection<Color>(new[]
            {
                Color.FromRgb(0xFF, 0xB3, 0x00), // Vivid Yellow
                Color.FromRgb(0x80, 0x3E, 0x75), // Strong Purple
                Color.FromRgb(0xFF, 0x68, 0x00), // Vivid Orange
                Color.FromRgb(0xA6, 0xBD, 0xD7), // Very Light Blue
                Color.FromRgb(0xC1, 0x00, 0x20), // Vivid Red
                Color.FromRgb(0xCE, 0xA2, 0x62), // Grayish Yellow
                Color.FromRgb(0x81, 0x70, 0x66), // Medium Gray
                Color.FromRgb(0x00, 0x7D, 0x34), // Vivid Green
                Color.FromRgb(0xF6, 0x76, 0x8E), // Strong Purplish Pink
                Color.FromRgb(0x00, 0x53, 0x8A), // Strong Blue
                Color.FromRgb(0xFF, 0x7A, 0x5C), // Strong Yellowish Pink
                Color.FromRgb(0x53, 0x37, 0x7A), // Strong Violet
                Color.FromRgb(0xFF, 0x8E, 0x00), // Vivid Orange Yellow
                Color.FromRgb(0xB3, 0x28, 0x51), // Strong Purplish Red
                Color.FromRgb(0xF4, 0xC8, 0x00), // Vivid Greenish Yellow
                Color.FromRgb(0x7F, 0x18, 0x0D), // Strong Reddish Brown
                Color.FromRgb(0x93, 0xAA, 0x00), // Vivid Yellowish Green
                Color.FromRgb(0x59, 0x33, 0x15), // Deep Yellowish Brown
                Color.FromRgb(0xF1, 0x3A, 0x13), // Vivid Reddish Orange
                Color.FromRgb(0x23, 0x2C, 0x16), // Dark Olive Green
            });

        public static IReadOnlyList<Color> All => _colors;

        public static Color VividYellow => _colors[0];

        public static Color StrongPurple => _colors[1];

        public static Color VividOrange => _colors[2];

        public static Color VeryLightBlue => _colors[3];

        public static Color VividRed => _colors[4];

        public static Color GrayishYellow => _colors[5];

        public static Color MediumGray => _colors[6];

        public static Color VividGreen => _colors[7];

        public static Color StrongPurplishPink => _colors[8];

        public static Color StrongBlue => _colors[9];

        public static Color StrongYellowishPink => _colors[10];

        public static Color StrongViolet => _colors[11];

        public static Color VividOrangeYellow => _colors[12];

        public static Color StrongPurplishRed => _colors[13];

        public static Color VividGreenishYellow => _colors[14];

        public static Color StrongReddishBrown => _colors[15];

        public static Color VividYellowishGreen => _colors[16];

        public static Color DeepYellowishBrown => _colors[17];

        public static Color VividReddishOrange => _colors[18];

        public static Color DarkOliveGreen => _colors[19];
    }
}
