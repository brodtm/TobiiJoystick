using System.Drawing;
using System.Drawing.Drawing2D;

namespace Utils
{
    public class GraphicsHelper
    {
        public static void DrawRectLines(Graphics g, Pen pen, RectangleF r)
        {
            DrawRectLines(g, pen, r, 0);
        }
        public static void DrawRectLines(Graphics g, Pen pen, RectangleF r, double orientation)
        {
            PointF[] points = RectToPolygon(r, orientation);
            g.DrawPolygon(pen, points);
        }
        public static void FillRectLines(Graphics g, Brush brush, RectangleF r, double orientation)
        {
            PointF[] points = RectToPolygon(r, orientation);
            g.FillPolygon(brush, points);
        }
        public static PointF[] RectToPolygon(RectangleF r, double orientation)
        {
            PointF center = new PointF(r.X + r.Width / 2.0F, r.Y + r.Height / 2.0F);
            PointF TL = new PointF(r.Left, r.Top);
            PointF TR = new PointF(r.Right, r.Top);
            PointF BL = new PointF(r.Left, r.Bottom);
            PointF BR = new PointF(r.Right, r.Bottom);
            PointF[] points = new PointF[] { BL, TL, TR, BR, BL };
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();
            m.RotateAt((float)orientation, center);
            m.TransformPoints(points);
            return points;
        }
        public static void ShowText(PointF pt, string text, Graphics g, Font font)
        {
            ShowText(pt, text, g, font, Color.Black, false);
        }
        public static void ShowText(PointF pt, string text, Graphics g, Font font, Color forecolor)
        {
            ShowText(pt, text, g, font, forecolor, false);
        }
        public static void ShowText(PointF pt, string text, Graphics g, Font font, Color forecolor, bool background)
        {
            ShowText(pt, text, g, font, forecolor, background, StringAlignment.Center, StringAlignment.Center);
        }
        public static void ShowText(PointF pt, string text, Graphics g, Font font, Color forecolor, bool background, StringAlignment linealign, StringAlignment align)
        {
            GraphicsState saved = g.Save();
            PointF[] pttxtlocation = new PointF[] { pt };
            g.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, pttxtlocation);
            g.ResetTransform();
            StringFormat sformat = new StringFormat();
            sformat.Alignment = align;
            sformat.LineAlignment = linealign;
            SolidBrush brush = new SolidBrush(forecolor);
            if (background)
            {
                SolidBrush backbrush = new SolidBrush(Color.Black);
                for (int i = 1; i < 2; i++)
                {
                    g.DrawString(text, font, backbrush, pttxtlocation[0].X - 1 * i, pttxtlocation[0].Y - 1 * i, sformat);
                    g.DrawString(text, font, backbrush, pttxtlocation[0].X - 1 * i, pttxtlocation[0].Y + 1 * i, sformat);
                    g.DrawString(text, font, backbrush, pttxtlocation[0].X + 1 * i, pttxtlocation[0].Y - 1 * i, sformat);
                    g.DrawString(text, font, backbrush, pttxtlocation[0].X + 1 * i, pttxtlocation[0].Y + 1 * i, sformat);
                }
                backbrush.Dispose();
            }
            g.DrawString(text, font, brush, pttxtlocation[0].X, pttxtlocation[0].Y, sformat);
            brush.Dispose();
            g.Restore(saved);
        }
    }
}