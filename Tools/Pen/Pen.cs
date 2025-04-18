using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace FriteCollection.Tools.Pen
{
    /// <summary>
    /// Draws basic shapes : Line, Rectangle, Circle, Point
    /// </summary>
    public abstract class Pen
    {
        /// <summary>
        /// Drawing settings
        /// </summary>
        public static float thickness = 1;
        public static float layer;
        public static Color Color = Color.White;
        public static Bounds GridOrigin;
        public static Environment environment;

        /// <summary>
        /// Draws a line from a point to an other
        /// </summary>
        public static void Line(Vector2 v1, Vector2 v2, float? thickness = null, Color? color = null)
        {
            float th = thickness is null ? Pen.thickness : thickness.Value;
            Color co = color.HasValue ? color.Value : Color;
            Vector2 offset = environment.Bounds[(int)GridOrigin];

            GameManager.Instance.SpriteBatch.DrawLine
            (
                    v1.X + offset.X - Camera.Position.X,
                    -v1.Y + offset.Y + Camera.Position.Y,
                    v2.X + offset.X - Camera.Position.X,
                    -v2.Y + offset.Y + Camera.Position.Y,
                    co,
                    th,
                    0
            );
        }

        /// <summary>
        /// Draws a rectangle. (Rectangle center point : TopLeft)
        /// </summary>
        public static void Rectangle(Vector2 v, float width, float height, float? thickness = null, Color? color = null)
        {
            float th = thickness is null ? Pen.thickness : thickness.Value;
            Color co = color.HasValue ? color.Value : Color;
            Vector2 offset = environment.Bounds[(int)GridOrigin];

            GameManager.Instance.SpriteBatch.DrawRectangle
            (
                new RectangleF
                (
                    v.X + offset.X, -v.Y + offset.Y, width, height
                ),
                co,
                th,
                0
            );
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        public static void Circle(Vector2 v, float radius, float? thickness = null, Color? color = null, float alpha = 1f)
        {
            float th = thickness is null ? Pen.thickness : thickness.Value;
            Color co = color.HasValue ? color.Value : Color;
            Vector2 offset = environment.Bounds[(int)GridOrigin];

            GameManager.Instance.SpriteBatch.DrawCircle
            (
                new CircleF
                (
                    new Vector2(
                    v.X + offset.X - Camera.Position.X,
                    -v.Y + offset.Y + Camera.Position.Y),
                    radius
                ),
                (int)radius,
                co,
                th,
                0
            );
        }

        /// <summary>
        /// Draws a point.
        /// </summary>
        public static void Point(Vector2 v, float? thickness = null, Color? color = null, float alpha = 1)
        {
            float th = thickness is null ? Pen.thickness : thickness.Value;
            Color co = color.HasValue ? color.Value : Color;
            Vector2 offset = environment.Bounds[(int)GridOrigin];

            GameManager.Instance.SpriteBatch.DrawPoint
            (
                v.X + offset.X,
                -v.Y + offset.Y,
                co,
                th,
                0
            );
        }
    }
}