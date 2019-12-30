using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Utils;
using System.Drawing.Drawing2D;
using Tobii.Interaction;

namespace TobiiJoystick
{
    public partial class Visualizer : UserControl
    {
        public Visualizer()
        {
            InitializeComponent();
            SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            this.Resize += _Resize;
        }
        private void _Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        Targets targets = null;
        Configuration cfg = null;
        public void SetTargets(Targets targets, Configuration cfg)
        {
            this.targets = targets;
            this.cfg = cfg;
        }
        RectangleF m_bounds;
        float m_zoomFit = 1;
        float border = 20;
        protected virtual RectangleF getBoundary(out Point2D leftbottom, out Point2D righttop)
        {
            double leftMost = double.PositiveInfinity;
            double rightMost = double.NegativeInfinity;
            double bottomMost = double.PositiveInfinity;
            double topMost = double.NegativeInfinity;

            RectangleF bounds = RectangleF.Empty;

            foreach (var kvp in targets.TargetRects)
            {
                bounds = RectangleF.Union(bounds, kvp.Value);
            }
            bounds.Inflate(border * 2, border * 2);
            leftMost = Math.Min(bounds.Left, bounds.Right);
            rightMost = Math.Max(bounds.Right, bounds.Left);
            bottomMost = Math.Min(bounds.Bottom, bounds.Top);
            topMost = Math.Max(bounds.Top, bounds.Bottom);
            leftbottom = new Utils.Point2D(leftMost, bottomMost);
            righttop = new Utils.Point2D(rightMost, topMost);
            m_bounds = bounds;
            return bounds;
        }
        /// <summary>
        /// Calculate scale factor needed to zoom full size
        /// </summary>
        /// <returns>scale factor</returns>
        protected virtual float calcZoomFit()
        {
            Point2D leftbottom, righttop;
            m_bounds = getBoundary(out leftbottom, out righttop);
            double largestRatio = Math.Max((double)m_bounds.Width / this.Width, (double)m_bounds.Height / this.Height);
            float zoomlevel = 1.0f / (float)largestRatio;
            m_zoomFit = zoomlevel;
            return zoomlevel;
        }
        System.Drawing.Point fixationPoint;
        public void SetFixationPoint(System.Drawing.Point p)
        {
            fixationPoint = p;
            Invalidate();
        }
        public void SetFixationData(FixationData fixationData)
        {
            System.Drawing.Point p = new System.Drawing.Point((int)fixationData.X, (int)fixationData.Y);
            fixationPoint = p;
            if (fixationData.EventType == Tobii.Interaction.Framework.FixationDataEventType.Data)
            {
                Invalidate();
            }
        }
        private void _Paint(object sender, PaintEventArgs e)
        {
            if (DesignMode)
            {
                //If in design mode
                return;
            }
            if (targets == null)
                return;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color backcolor = Color.Gainsboro;
            g.Clear(backcolor);
            Brush borderBrush = new SolidBrush(Color.LightGray);
            Pen borderPen = new Pen(Color.DarkGray, -1.0f);
            Brush markerBrush = new SolidBrush(Color.FromArgb(128, Color.Blue));
            Pen markerBorder = new Pen(Color.Blue, -1.0f);
            Brush currposBrush = new SolidBrush(Color.FromArgb(128, Color.Red));
            Pen currposBorder = new Pen(Color.Red, -1.0f);
            Pen movePen = new Pen(Color.Green, -1.0f);
            Brush avoidBrush = new SolidBrush(Color.FromArgb(255 / 4, Color.Red));
            Pen avoidBorder = new Pen(Color.FromArgb(255 / 2, Color.Red), -1.0f);
            float markRadius = 10;

            float zoom = calcZoomFit();
            g.ScaleTransform(zoom, zoom);
            g.TranslateTransform(border, border);
            g.TranslateTransform(-m_bounds.X, -m_bounds.Y);

            foreach (var kvp in targets.TargetRects)
            {
                RectangleF r = kvp.Value;
                g.DrawRectangle(avoidBorder, r.X, r.Y, r.Width, r.Height);
                string s = kvp.Key.ToString();
                int btn = 0;
                if (cfg != null)
                {
                    if (cfg.ButtonMap.ContainsKey(kvp.Key))
                    {
                        btn = cfg.ButtonMap[kvp.Key];
                    }
                }
                if (btn > 0)
                {
                    s = string.Format("{0}\r\nButton {1}", kvp.Key, btn);
                }
                GraphicsHelper.ShowText(new PointF(r.X + (r.Width / 2.0f), r.Y + (r.Height / 2.0f)), s, g, this.Font);
            }
            g.FillEllipse(markerBrush, (float)(fixationPoint.X - markRadius), (float)(fixationPoint.Y - markRadius), (float)(markRadius * 2.0), (float)(markRadius * 2.0));
            g.DrawEllipse(markerBorder, (float)(fixationPoint.X - markRadius), (float)(fixationPoint.Y - markRadius), (float)(markRadius * 2.0), (float)(markRadius * 2.0));

            TargetLocation hit = targets.CheckHit(fixationPoint);
            if (hit != TargetLocation.None)
            {
                RectangleF r = targets.TargetRects[hit];
                g.FillRectangle(avoidBrush, r.X, r.Y, r.Width, r.Height);
            }
            g.ResetTransform();
            borderBrush.Dispose();
            borderPen.Dispose();
            markerBrush.Dispose();
            markerBorder.Dispose();
            currposBrush.Dispose();
            currposBorder.Dispose();
            movePen.Dispose();
            avoidBrush.Dispose();
            avoidBorder.Dispose();
        }
    }
}
