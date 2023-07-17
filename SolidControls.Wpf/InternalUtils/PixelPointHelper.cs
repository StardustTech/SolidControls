using System;
using System.Windows;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    using Core;

    internal static class PixelPointHelper
    {
        public static PixelPoint Convert(this Point point) {
            return new PixelPoint {
                X = (int)Math.Round(point.X),
                Y = (int)Math.Round(point.Y),
            };
        }

        public static Point Convert(this PixelPoint pixelPoint) {
            return new Point(pixelPoint.X, pixelPoint.Y);
        }
    }
}
