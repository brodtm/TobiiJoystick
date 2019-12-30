using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
namespace Utils
{
    /// <summary>
    /// Summary description for SizeD.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public struct SizeD
    {
        private double width;
        private double height;

        /// <summary>
        /// An empty rectangle
        /// </summary>
        public static readonly SizeD Empty;

        static SizeD()
        {
            Empty = new SizeD();
        }

        public SizeD(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Easy conversion from Point2D to SizeD
        /// </summary>
        /// <param name="pt"></param>
        public SizeD(Point2D pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double Area
        {
            get { return width * height; }
        }

        public static bool operator ==(SizeD left, SizeD right)
        {
            return left.Width == right.Width && left.Height == right.Height;
        }

        public static bool operator !=(SizeD left, SizeD right)
        {
            return !(left == right);
        }

        public static SizeD operator *(SizeD left, double right)
        {
            return new SizeD(left.width * right, left.height * right);
        }

        public static SizeD operator /(SizeD left, double right)
        {
            return new SizeD(left.width / right, left.height / right);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(SizeD) && this == (SizeD)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            string s = "W=" + width.ToString() + ", H=" + height.ToString();
            return s;
        }
    };
}
