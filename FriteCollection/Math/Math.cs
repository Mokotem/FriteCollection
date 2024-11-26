using System;

namespace FriteCollection.Math
{
    public abstract class Math
    {
        /// <summary>
        /// 3.1415 ...
        /// </summary>
        public static readonly float pi = MathF.PI;

        /// <summary>
        /// rad * rad2deg = deg
        /// </summary>
        public static readonly float rad2deg = 180 / MathF.PI;

        /// <summary>
        /// deg * deg2rad = rad
        /// </summary>
        public static readonly float deg2rad = MathF.PI / 180;

        /// <summary>
        /// En degrés.
        /// </summary>
        public static float Sin(float d)
        {
            return MathF.Sin(d * (MathF.PI / 180));
        }
        /// <summary>
        /// En degrés.
        /// </summary>
        public static float Cos(float d)
        {
            return MathF.Cos(d * (MathF.PI / 180));
        }
        /// <summary>
        /// En degrés.
        /// </summary>
        public static float Tan(float d)
        {
            return MathF.Tan(d * (MathF.PI / 180));
        }

        /// <summary>
        /// Valeur absolue.
        /// </summary>
        public static float Abs(float n)
        {
            return MathF.Abs(n);
        }

        /// <summary>
        /// Arrondi.
        /// </summary>
        public static float Round(float n)
        {
            return MathF.Round(n);
        }

        public static float Min(float a, float b)
        {
            return MathF.Min(a, b);
        }

        public static float Max(float a, float b)
        {
            return MathF.Max(a, b);
        }

        public static int Quotient(int n, int n2)
        {
            return System.Math.DivRem(n, n2).Quotient;
        }

        public static float Sqrt(float n)
        {
            return MathF.Sqrt(n);
        }

        public static float GetDistance(Entity.Vector v1, Entity.Vector v2)
        {
            return Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y));
        }

        public static float? GetAngle(Entity.Vector v1, Entity.Vector v2)
        {
            if (v2.x - v1.x == 0)
                return null;
            else
                return MathF.Atan((v2.y - v1.y) / (v2.x - v1.x)) * rad2deg + (v1.x < v2.x ? 0 : 180);
        }

        public static float Pow(float a, float b)
        {
            return MathF.Pow(a, b);
        }
    }
}
