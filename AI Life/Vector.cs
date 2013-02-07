using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AI_Life
{
    //implementation of a 2-D vector
    public struct Vector2
    {
        public float X, Y;
        public Vector2(float a)
        {
            X = a;
            Y = a;
        }

        public Vector2(float a, float b)
        {
            X = a;
            Y = b;
        }

        public Vector2(PointF input)
        {
            X = input.X;
            Y = input.Y;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public static double Length(Vector2 v1)
        {
            return Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
        }

        public static Vector2 Normalize(Vector2 v1)
        {
            double length = Vector2.Length(v1);
            if (length > 0)
            {
                return new Vector2((float)(v1.X / Vector2.Length(v1)), (float)(v1.Y / Vector2.Length(v1)));
            }
            else
            {
                return new Vector2((float)(v1.X / 1), (float)(v1.Y / 1));
            }
        }
        public void Normalize()
        {
            this.X = (float)(this.X / Vector2.Length(this));
            this.Y = (float)(this.Y / Vector2.Length(this));
        }

        public static Vector2 Add(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 Subtract(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.X + v1.Y * v2.Y);
        }

        public static Vector2 Multiply(Vector2 v1, float v2)
        {
            return new Vector2(v1.X * v2, v1.Y * v2);
        }

        public static Vector2 operator +(Vector2 v1, int a)
        {
            return new Vector2(v1.X + a, v1.Y + a);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 a)
        {
            return new Vector2(v1.X + a.X, v1.Y + a.Y);
        }
        public static Vector2 operator *(Vector2 v1, int a)
        {
            return new Vector2(v1.X * a, v1.Y * a);
        }

        public static Vector2 operator *(Vector2 v1, float a)
        {
            return new Vector2(v1.X * a, v1.Y * a);
        }
        public static Vector2 operator /(Vector2 v1, int a)
        {
            if (a != 0)
            {
                return new Vector2(v1.X / a, v1.Y / a);
            }
            else
                return v1;
        }

        public static Vector2 operator /(Vector2 v1, float a)
        {
            if (a != 0)
                return new Vector2(v1.X / a, v1.Y / a);
            else
                return v1;
        }

        public static Vector2 operator /(Vector2 v1, double a)
        {
            if (a != 0)
                return new Vector2((float)(v1.X / a), (float)(v1.Y / a));
            else
                return v1;
        }

        public override string ToString()
        {
            return (X.ToString() + ", " + Y.ToString());
        }

        internal static Vector2 Truncate(Vector2 vec, float max_value)
        {
            if (vec.Length() > max_value)
            {
                return Vector2.Multiply(Vector2.Normalize(vec), max_value);
            }
            return vec;
        }
    }
}
