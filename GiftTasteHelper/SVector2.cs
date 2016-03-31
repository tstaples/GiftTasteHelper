using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GiftTasteHelper
{
    public class SVector2
    {
        public static SVector2 Zero { get { return new SVector2(); } }

        public float x { get; set; }
        public float y { get; set; }

        public int xi
        {
            get { return (int)x; }
            set { x = (float)value; }
        }
        public int yi
        {
            get { return (int)y; }
            set { y = (float)value; }
        }

        public SVector2()
        {
            x = 0;
            y = 0;
        }

        public SVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public SVector2(int x, int y)
        {
            this.xi = x;
            this.yi = y;
        }

        public SVector2(Vector2 v)
        {
            x = v.X;
            y = v.Y;
        }

        public SVector2(Point p)
        {
            xi = p.X;
            yi = p.Y;
        }

        public Vector2 ToXNAVector2()
        {
            return new Vector2(x, y);
        }

        public Point ToPoint()
        {
            return new Point(xi, yi);
        }

        public static SVector2 Max(SVector2 a, SVector2 b)
        {
            return (a.x > b.x && a.y > b.y) ? a : b;
        }

        public static SVector2 Min(SVector2 a, SVector2 b)
        {
            return (a.x > b.x && a.y > b.y) ? b : a;
        }

        public static SVector2 MeasureString(string s, SpriteFont font)
        {
            return new SVector2(font.MeasureString(s));
        }

        public override bool Equals(object other)
        {
            return (this == (SVector2)other);
        }

        public override string ToString()
        {
            return "{" + x + ", " + y + "}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region operators
        public static SVector2 operator +(SVector2 a, SVector2 b)
        {
            return new SVector2(a.x + b.x, a.y + b.y);
        }

        public static SVector2 operator -(SVector2 value)
        {
            return new SVector2(-value.x, -value.y);
        }

        public static SVector2 operator -(SVector2 a, SVector2 b)
        {
            return new SVector2(a.x - b.x, a.y - b.y);
        }

        public static SVector2 operator *(SVector2 a, SVector2 b)
        {
            return new SVector2(a.x * b.x, a.y * b.y);
        }

        public static SVector2 operator *(SVector2 v, float scaleFactor)
        {
            return new SVector2(v.x * scaleFactor, v.y * scaleFactor);
        }

        public static SVector2 operator *(float scaleFactor, SVector2 v)
        {
            return new SVector2(v.x * scaleFactor, v.y * scaleFactor);
        }

        public static SVector2 operator /(SVector2 v, SVector2 b)
        {
            return new SVector2(v.x / b.x, v.y / b.y);
        }

        public static SVector2 operator /(SVector2 v, float divider)
        {
            return new SVector2(v.x / divider, v.y / divider);
        }

        public static bool operator ==(SVector2 a, SVector2 b)
        {
            return (a.x == b.x && a.y == b.y);
        }

        public static bool operator !=(SVector2 a, SVector2 b)
        {
            return !(a == b);
        }
        #endregion operators
    }
}
