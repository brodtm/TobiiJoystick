using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiJoystick
{
    public enum TargetLocation
    {
        None,
        Screen,
        TopLeft, Top, TopRight,
        RightUpper, Right, RightLower,
        BottomRight, Bottom, BottomLeft,
        LeftLower, Left, LeftUpper,
        Corner_Upper_Left,
        Corner_Upper_Right,
        Corner_Lower_Left,
        Corner_Lower_Right,
    }

    public class Targets
    {
        System.Drawing.Rectangle screen;
        System.Drawing.Size bufferZone;
        double targetPercent = 1.0 / 3.0;//thirds
        Dictionary<TargetLocation, System.Drawing.Rectangle> targetRects = new Dictionary<TargetLocation, System.Drawing.Rectangle>();
        public Targets(System.Drawing.Rectangle r, double bufferPercentX, double bufferPercentY)
        {
            screen = r;
            bufferZone = new System.Drawing.Size((int)(r.Width * bufferPercentX), (int)(r.Height * bufferPercentY));
            System.Drawing.Size targetSize = new System.Drawing.Size((int)(r.Width * targetPercent), (int)(r.Height * targetPercent));
            targetRects.Add(TargetLocation.Screen, screen);
            targetRects.Add(TargetLocation.Top, new System.Drawing.Rectangle(screen.X + (targetSize.Width * 1), screen.Y - targetSize.Height - bufferZone.Height, targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.TopLeft, new System.Drawing.Rectangle(screen.X + (targetSize.Width * 0), screen.Y - targetSize.Height - bufferZone.Height, targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.TopRight, new System.Drawing.Rectangle(screen.X + (targetSize.Width * 2), screen.Y - targetSize.Height - bufferZone.Height, targetSize.Width, targetSize.Height));

            targetRects.Add(TargetLocation.Bottom, new System.Drawing.Rectangle(screen.X + (targetSize.Width * 1), screen.Y + screen.Height + bufferZone.Height, targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.BottomLeft, new System.Drawing.Rectangle(screen.X + (targetSize.Width * 0), screen.Y + screen.Height + bufferZone.Height, targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.BottomRight, new System.Drawing.Rectangle(screen.X + (targetSize.Width * 2), screen.Y + screen.Height + bufferZone.Height, targetSize.Width, targetSize.Height));

            targetRects.Add(TargetLocation.Right, new System.Drawing.Rectangle(screen.X + screen.Width + bufferZone.Width, screen.Y + (targetSize.Height * 1), targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.RightLower, new System.Drawing.Rectangle(screen.X + screen.Width + bufferZone.Width, screen.Y + (targetSize.Height * 2), targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.RightUpper, new System.Drawing.Rectangle(screen.X + screen.Width + bufferZone.Width, screen.Y + (targetSize.Height * 0), targetSize.Width, targetSize.Height));

            targetRects.Add(TargetLocation.Left, new System.Drawing.Rectangle(screen.X - bufferZone.Width - targetSize.Width, screen.Y + (targetSize.Height * 1), targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.LeftLower, new System.Drawing.Rectangle(screen.X - bufferZone.Width - targetSize.Width, screen.Y + (targetSize.Height * 2), targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.LeftUpper, new System.Drawing.Rectangle(screen.X - bufferZone.Width - targetSize.Width, screen.Y + (targetSize.Height * 0), targetSize.Width, targetSize.Height));
            //corners
            targetRects.Add(TargetLocation.Corner_Upper_Left, new System.Drawing.Rectangle(screen.X - bufferZone.Width - targetSize.Width, screen.Y - targetSize.Height - bufferZone.Height, targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.Corner_Upper_Right, new System.Drawing.Rectangle(screen.X + screen.Width + bufferZone.Width, screen.Y - targetSize.Height - bufferZone.Height, targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.Corner_Lower_Left, new System.Drawing.Rectangle(screen.X - bufferZone.Width - targetSize.Width, screen.Y + screen.Height + bufferZone.Height, targetSize.Width, targetSize.Height));
            targetRects.Add(TargetLocation.Corner_Lower_Right, new System.Drawing.Rectangle(screen.X + screen.Width + bufferZone.Width, screen.Y + screen.Height + bufferZone.Height, targetSize.Width, targetSize.Height));
        }

        public Dictionary<TargetLocation, Rectangle> TargetRects { get => targetRects; set => targetRects = value; }

        public TargetLocation CheckHit(System.Drawing.Point p)
        {
            foreach (var kvp in targetRects)
            {
                if (kvp.Value.Contains(p))
                    return kvp.Key;
            }
            return TargetLocation.None;
        }
    }
}