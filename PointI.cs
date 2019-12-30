using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
namespace Utils
{
	/// <summary>
	/// Summary description for PointI.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	[Serializable]
	//[TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    [TypeConverter(typeof(PointIConverter))]
	public struct PointI
	{
		private int x;
		private int y;

		public PointI(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		
		public int X
		{
			get { return x; }
			set { x = value; }
		}

		public int Y
		{
			get { return y; }
			set { y = value; }
		}
        [Browsable(false)]
        //[DontSerialize]
		public PointI CenterLine
		{
			get { return new PointI((x-1)/2, (y-1)/2); }
		}

		public static bool operator ==(PointI left, PointI right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(PointI left, PointI right)
		{
			return !(left == right);
		}

		public static PointI operator -(PointI target)
		{
			return new PointI(-target.x, -target.y);
		}

		public static PointI operator +(PointI left, PointI right)
		{
			return new PointI(left.X + right.X, left.Y + right.Y);
		}

		public static PointI operator +(PointI left, int right)
		{
			return new PointI(left.X + right, left.Y + right);
		}

		public static PointI operator -(PointI left, PointI right)
		{
			return new PointI(left.X - right.X, left.Y - right.Y);
		}

		public static PointI operator -(PointI left, int right)
		{
			return new PointI(left.X - right, left.Y - right);
		}

		public static PointI operator *(PointI left, PointI right)
		{
			return new PointI(left.X * right.X, left.Y * right.Y);
		}

		public static PointI operator *(PointI left, int right)
		{
			return new PointI(left.X * right, left.Y * right);
		}

		public static Point2D operator *(PointI left, double right)
		{
			return new Point2D(left.X * right, left.Y * right);
		}

		public static PointI operator /(PointI left, PointI right)
		{
			return new PointI(left.X / right.X, left.Y / right.Y);
		}

		public static PointI operator /(PointI left, int right)
		{
			return new PointI(left.X / right, left.Y / right);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == typeof(PointI) && this == (PointI)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			string s = "X=" + x.ToString() + ", Y=" + y.ToString();
			return s;
		}

        public static PointI Parse(string str)
        {
            // parse into tokens
            string[] tokens = str.Split(' ', '\t', '\r', '\n', ',', '=');
            double x = double.NaN;
            double y = double.NaN;
            int ix = 0;
            int iy = 0;
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
                            {
                                x = d;
                                ix = int.Parse(s);
                            }
                            else if (double.IsNaN(y))
                            {
                                y = d;
                                iy = int.Parse(s);
                            }
                            else
                                throw new FormatException("too many tokens");
                            break;
                    }
                }
            }
            if (double.IsNaN(x) || double.IsNaN(y))
                throw new FormatException("too few tokens");
            return new PointI(ix, iy);
        }

	};
}
