using System;
using System.Windows;

namespace Stardust.OpenSource.SolidControls.Core
{
    public static class LineGeneratorUtils
    {
        // See wikipedia Bresenham's line algorithm
        public static void BresenhamLineGenerator(PixelPoint startPoint, PixelPoint endPoint, Action<int, int> plotAction) {
            if (plotAction == null)
                return;

            int x0 = startPoint.X, x1 = endPoint.X;
            int y0 = startPoint.Y, y1 = endPoint.Y;

            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;

            while (true) {
                plotAction.Invoke(x0, y0);

                if (x0 == x1 && y0 == y1)
                    break;

                e2 = err;

                if (e2 > -dx) {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dy) {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        // See wikipedia Xiaolin Wu's line algorithm
        // Anti-aliasing but slower
        public static void XiaolinWuLineGenerator(PixelPoint startPoint, PixelPoint endPoint, Action<int, int, byte> plotAction) {
            if (plotAction == null)
                return;

            double x0 = startPoint.X, x1 = endPoint.X;
            double y0 = startPoint.Y, y1 = endPoint.Y;

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (steep) {
                double tempExchange;

                // swap (ref x0, ref y0)
                tempExchange = x0;
                x0 = y0;
                y0 = tempExchange;

                // swap (ref x1, ref y1)
                tempExchange = x1;
                x1 = y1;
                y1 = tempExchange;
            }

            if (x0 > x1) {
                double tempExchange;

                // swap (ref x0, ref x1)
                tempExchange = x0;
                x0 = x1;
                x1 = tempExchange;

                // swap (ref y0, ref y1)
                tempExchange = y0;
                y0 = y1;
                y1 = tempExchange;
            }

            double dx = x1 - x0;
            double dy = y1 - y0;
            double gradient = dx == 0 ? 1 : dy / dx;

            // handle first endpoint
            double xEnd = Math.Round(x0);
            double yEnd = y0 + gradient * (xEnd - x0);
            double xGap = RFPart(x0 + 0.5);
            int xpxl1 = (int)xEnd; // this will be used in the main loop
            int ypxl1 = (int)Math.Floor(yEnd);
            if (steep) {
                plotAction.Invoke(ypxl1, xpxl1, DoubleBrightnesssToByte(RFPart(yEnd) * xGap));
                plotAction.Invoke(ypxl1 + 1, xpxl1, DoubleBrightnesssToByte(FPart(yEnd) * xGap));
            }
            else {
                plotAction.Invoke(xpxl1, ypxl1, DoubleBrightnesssToByte(RFPart(yEnd) * xGap));
                plotAction.Invoke(xpxl1, ypxl1 + 1, DoubleBrightnesssToByte(FPart(yEnd) * xGap));
            }

            double intery = yEnd + gradient; // first y-intersection for the main loop

            // handle second endpoint
            xEnd = Math.Round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = FPart(x1 + 0.5);
            int xpxl2 = (int)xEnd; // this will be used in the main loop
            int ypxl2 = (int)Math.Floor(yEnd);
            if (steep) {
                plotAction.Invoke(ypxl2, xpxl2, DoubleBrightnesssToByte(RFPart(yEnd) * xGap));
                plotAction.Invoke(ypxl2 + 1, xpxl2, DoubleBrightnesssToByte(FPart(yEnd) * xGap));
            }
            else {
                plotAction.Invoke(xpxl2, ypxl2, DoubleBrightnesssToByte(RFPart(yEnd) * xGap));
                plotAction.Invoke(xpxl2, ypxl2 + 1, DoubleBrightnesssToByte(FPart(yEnd) * xGap));
            }

            // main loop
            if (steep) {
                for (int x = xpxl1 + 1; x < xpxl2; x++) {
                    plotAction.Invoke((int)Math.Floor(intery), x, DoubleBrightnesssToByte(RFPart(intery)));
                    plotAction.Invoke((int)Math.Floor(intery) + 1, x, DoubleBrightnesssToByte(FPart(intery)));
                    intery += gradient;
                }
            }
            else {
                for (int x = xpxl1 + 1; x < xpxl2; x++) {
                    plotAction.Invoke(x, (int)Math.Floor(intery), DoubleBrightnesssToByte(RFPart(intery)));
                    plotAction.Invoke(x, (int)Math.Floor(intery) + 1, DoubleBrightnesssToByte(FPart(intery)));
                    intery += gradient;
                }
            }
        }

        private static double RFPart(double x) {
            return 1 - FPart(x);
        }

        private static double FPart(double x) {
            return x - Math.Floor(x);
        }

        private static byte DoubleBrightnesssToByte(double brightness) {
            return (byte)Math.Round(brightness * 255);
        }
    }
}
