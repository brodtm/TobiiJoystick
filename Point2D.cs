
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Design;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Utils  
{
	/// <summary>
	/// Summary description for Point2D.
	/// </summary>

	[StructLayout(LayoutKind.Sequential)]
	[Serializable]
	//[TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    [TypeConverter(typeof(Point2DConverter))]
    public struct Point2D
	{
		public static readonly Point2D Invalid = new Point2D(double.NaN, double.NaN);

		private double x;
		private double y;

         /// <summary>
        /// An empty rectangle
        /// </summary>
        public static readonly Point2D Empty;

        static Point2D()
        {
            Empty = new Point2D();
        }
        public bool IsEmpty
        {
            get { return (x==0)&&(y==0);}
        }

		public Point2D(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Easy conversion from SizeD to Point2D
		/// </summary>
		public Point2D(SizeD sz)
		{
			this.x = sz.Width;
			this.y = sz.Height;
		}
        public Point2D(PointF p)
        {
            this.x = p.X;
            this.y = p.Y;
        }

        public Point2D(Point2D p)
        {
            this.x = p.X;
            this.y = p.Y;
        }
        public Point2D(Point p)
        {
            this.x = p.X;
            this.y = p.Y;
        }
        public bool IsValid
		{
            [DebuggerStepThrough]
            get
			{
				return !double.IsNaN(this.x) && !double.IsNaN(this.y);
			}
		}
		public double X
		{
            [DebuggerStepThrough]
            get { return x; }
            [DebuggerStepThrough]
            set { x = value; }
		}
		public double Y
		{
            [DebuggerStepThrough]
            get { return y; }
            [DebuggerStepThrough]
            set { y = value; }
		}

		public static bool operator ==(Point2D left, Point2D right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(Point2D left, Point2D right)
		{
			return !(left == right);
		}

		public static Point2D operator -(Point2D target)
		{
			return new Point2D(-target.x, -target.y);
		}

		public static Point2D operator +(Point2D left, Point2D right)
		{
			return new Point2D(left.X + right.X, left.Y + right.Y);
		}

		public static Point2D operator +(Point2D left, PointI right)
		{
			return new Point2D(left.X + right.X, left.Y + right.Y);
		}

		public static Point2D operator +(Point2D left, int right)
		{
			return new Point2D(left.X + right, left.Y + right);
		}

		public static Point2D operator +(Point2D left, SizeD right)
		{
			return new Point2D(left.X + right.Width, left.Y + right.Height);
		}

		public static Point2D operator -(Point2D left, Point2D right)
		{
			return new Point2D(left.X - right.X, left.Y - right.Y);
		}

		public static Point2D operator -(Point2D left, SizeD right)
		{
			return new Point2D(left.X - right.Width, left.Y - right.Height);
		}

		public static Point2D operator *(Point2D left, Point2D right)
		{
			return new Point2D(left.X * right.X, left.Y * right.Y);
		}

		public static Point2D operator *(Point2D left, PointI right)
		{
			return new Point2D(left.X * right.X, left.Y * right.Y);
		}

		public static Point2D operator *(Point2D left, int right)
		{
			return new Point2D(left.X * right, left.Y * right);
		}

		public static Point2D operator *(Point2D left, double right)
		{
			return new Point2D(left.X * right, left.Y * right);
		}

		public static Point2D operator /(Point2D left, Point2D right)
		{
			return new Point2D(left.X / right.X, left.Y / right.Y);
		}

		public static Point2D operator /(Point2D left, double right)
		{
			return new Point2D(left.X / right, left.Y / right);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == typeof(Point2D) && this == (Point2D)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		//public Point2D Midpoint(Point2D second)
		//{
		//	return new Point2D(MathUtils.Average(this.X, second.X), 
		//					  MathUtils.Average(this.Y, second.Y));
		//}

		//public double Distance
		//{
		//	get { return MathUtils.Hypotenuse(x, y); }
		//}

		//public double DistanceTo(Point2D second)
		//{
		//	return MathUtils.Hypotenuse(this.X - second.X, this.Y - second.Y);
		//}

		public override string ToString()
		{
            //return string.Format("X={0:F6}, Y={1:F6}", x, y);//F4
            string s = string.Format(CultureInfo.InvariantCulture,"X={0:F6}, Y={1:F6}", x, y);//F4
            return s;
        }

		public static Point2D Parse(string str)
		{
			// parse into tokens
			string[] tokens = str.Split(' ', '\t', '\r', '\n', ',', '=');
			double x = double.NaN;
			double y = double.NaN;
			foreach (string s in tokens)
			{
				if (s != null && s.Length != 0)
				{
					switch (s)
					{
						case "X":
						case "x":
						case "Y":
						case "y":
							break;
						default:
							double d = double.Parse(s, CultureInfo.InvariantCulture);
							if (double.IsNaN(x))
								x = d;
							else if (double.IsNaN(y))
								y = d;
							else
								throw new FormatException("too many tokens");
							break;
					}
				}
			}
			if (double.IsNaN(x) || double.IsNaN(y))
				throw new FormatException("too few tokens");
			return new Point2D(x, y);
		}

		/// <summary>
		/// Returns a new Point2D with the X and Y values swapped.
		/// </summary>
		public Point2D Swapped
		{
			get { return new Point2D(this.y, this.x); }
		}

		/// <summary>
		/// Swaps the X and Y values.
		/// </summary>
		public void Swap()
		{
			double temp = this.x;
			this.x = this.y;
			this.y = temp;
		}
        public PointF ToPointF()
        {
            return new PointF((float)this.x, (float)this.y);
        }
        public Point ToPoint()
        {
            return new Point((int)this.x, (int)this.y);
        }

    };

    /// <summary>
    /// Type converter for DLLComponent, allows saving a DLLComponent to/from a string
    /// </summary>
    public class Point2DConverter : TypeConverter
    {

        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
            {
                return Point2D.Parse((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((Point2D)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Type converter for DLLComponent, allows saving a DLLComponent to/from a string
    /// </summary>
    public class PointIConverter : TypeConverter
    {

        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is string)
            {
                return PointI.Parse((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((PointI)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    ///// <summary>
    ///// Type converter for DLLComponent, allows saving a DLLComponent to/from a string
    ///// </summary>
    //public class PointI3DConverter : TypeConverter
    //{

    //    // Overrides the CanConvertFrom method of TypeConverter.
    //    // The ITypeDescriptorContext interface provides the context for the
    //    // conversion. Typically, this interface is used at design time to 
    //    // provide information about the design-time container.
    //    public override bool CanConvertFrom(ITypeDescriptorContext context,
    //        Type sourceType)
    //    {

    //        if (sourceType == typeof(string))
    //        {
    //            return true;
    //        }
    //        return base.CanConvertFrom(context, sourceType);
    //    }
    //    // Overrides the ConvertFrom method of TypeConverter.
    //    public override object ConvertFrom(ITypeDescriptorContext context,
    //        CultureInfo culture, object value)
    //    {
    //        if (value is string)
    //        {
    //            return PointI3D.Parse((string)value);
    //        }
    //        return base.ConvertFrom(context, culture, value);
    //    }
    //    // Overrides the ConvertTo method of TypeConverter.
    //    public override object ConvertTo(ITypeDescriptorContext context,
    //        CultureInfo culture, object value, Type destinationType)
    //    {
    //        if (destinationType == typeof(string))
    //        {
    //            return ((PointI3D)value).ToString();
    //        }
    //        return base.ConvertTo(context, culture, value, destinationType);
    //    }
    //}
}
