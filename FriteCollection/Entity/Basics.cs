using FriteCollection.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection.Entity
{
    interface IDraw
    {
        /// <summary>
        /// Draws the entity on screen.
        /// </summary>
        public void Draw();
    }

    interface ICopy<T>
    {
        /// <summary>
        /// Makes a copy.
        /// </summary>
        public T Copy();
    }

    /// <summary>
    /// (x, y)
    /// </summary>
    public struct Vector
    {
        public float x;
        public float y;

        /// <summary>
        /// (0, 0)
        /// </summary>
        public static readonly Vector Zero = new(0, 0);

        /// <summary>
        /// (x, y)
        /// </summary>
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// (0, 0)
        /// </summary>
        public Vector()
        {
            x = 0;
            y = 0;
        }

        public static Vector operator +(Vector v, float n) { return new Vector(v.x + n, v.y + n); }
        public static Vector operator -(Vector v, float n) { return new Vector(v.x - n, v.y - n); }
        public static Vector operator *(Vector v, float n) { return new Vector(v.x * n, v.y * n); }
        public static Vector operator /(Vector v, float n) { return new Vector(v.x / n, v.y / n); }

        public static Vector operator +(Vector v, Vector n) { return new Vector(v.x + n.x, v.y + n.y); }
        public static Vector operator -(Vector v, Vector n) { return new Vector(v.x - n.x, v.y - n.y); }
        public static Vector operator *(Vector v, Vector n) { return new Vector(v.x * n.x, v.y * n.y); }
        public static Vector operator /(Vector v, Vector n) { return new Vector(v.x / n.x, v.y / n.y); }

        public override bool Equals(object value)
        {
            if (value is Vector)
            {
                return this.x == ((Vector)value).x && this.y == ((Vector)value).y;
            }
            return false;
        }
        public static bool operator == (Vector v, Vector n) { return v.Equals(n); }
        public static bool operator !=(Vector v, Vector n) { return !v.Equals(n); }

        public readonly override string ToString()
        {
            return "Vector(" + x + ", " + y + ")";
        }

        public readonly Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// Describes a state in space.
    /// </summary>
    public class Space : ICopy<Space>
    {
        public Space()
        {
            _eGridOrigin = Bounds.Center;
            _eCenterPoint = Bounds.Center;
            Position = Vector.Zero;
            Scale = new Vector(50, 50);
            rotation = 0;
        }

        public Space Copy()
        {
            return new Space()
            {
                Position = this.Position,
                Scale = this.Scale,
                _eGridOrigin = this._eGridOrigin,
                _eCenterPoint = this._eCenterPoint,
                rotation = this.rotation,
                _ui = this._ui
            };
        }

        /// <summary>
        /// Returns the TopLeft position of the entity.
        /// </summary>
        /// <param name="includeCamera">if false, the position is not related to the camera.</param>
        public Vector GetScreenPosition(bool includeCamera = true)
        {
            if (this.UI)
                return new Vector(Position.x + FriteModel.MonoGame.instance.UIscreenBounds[(int)GridOrigin].x, Position.y + FriteModel.MonoGame.instance.UIscreenBounds[(int)GridOrigin].y);
            else
                return 
                    GridOrigin == Camera.GridOrigin && includeCamera ?
                    new Vector(((Position.x - Camera.Position.x) * Camera.zoom) + FriteModel.MonoGame.instance.screenBounds[(int)GridOrigin].x, -((Position.y - Camera.Position.y) * Camera.zoom) + FriteModel.MonoGame.instance.screenBounds[(int)GridOrigin].y)
                    :
                    new Vector(Position.x + FriteModel.MonoGame.instance.screenBounds[(int)GridOrigin].x, -Position.y + FriteModel.MonoGame.instance.screenBounds[(int)GridOrigin].y);
        }

        private Bounds _eGridOrigin;

        /// <summary>
        /// Origin position based on the screen.
        /// </summary>
        public Bounds GridOrigin
        {
            get { return _eGridOrigin; }
            set
            {
                _eGridOrigin = value;
            }
        }

        private Bounds _eCenterPoint;

        /// <summary>
        /// Center point of position and rotation.
        /// </summary>
        public Bounds CenterPoint
        {
            get { return _eCenterPoint; }
            set
            {
                _eCenterPoint = value;
            }
        }

        /// <summary>
        /// Entity position.
        /// </summary>
        public Vector Position;

        /// <summary>
        /// Entity scale. texture is flipped if negative.
        /// </summary>
        public Vector Scale;

        /// <summary>
        /// Entity rotation (degree).
        /// </summary>
        public float rotation;

        private bool _ui = false;

        public override bool Equals(object obj)
        {
            if (obj is Space)
            {
                Space sp = obj as Space;
                return (this.Scale == sp.Scale && this.Position == sp.Position
                    && this.rotation == sp.rotation && this._eCenterPoint == sp._eCenterPoint
                    && this._eGridOrigin == sp._eGridOrigin && this._ui == sp._ui);
            }
            return false;
        }

        /// <summary>
        /// if true : the Space is related to the user window.
        /// if false : the space is is related to the game resolution (specified in settings).
        /// </summary>
        public bool UI
        {
            get
            {
                return _ui;
            }

            set
            {
                _ui = value;
            }
        }

        public override string ToString()
        {
            return "Transform (position:" + Position.ToString() + ", scale:" + Scale.ToString() + ", direction:" + rotation + ")";
        }
    }

    class BoundFunc
    {
        public Vector BoundToVector(Bounds b, float width, float height)
        {
            return (b) switch
            {
                Bounds.TopLeft => new Vector(0, 0),
                Bounds.Top => new Vector(width / 2f, 0),
                Bounds.TopRight => new Vector(width, 0),

                Bounds.Left => new Vector(0, height / 2f),
                Bounds.Center => new Vector(width / 2f, height / 2f),
                Bounds.Right => new Vector(width, height / 2f),

                Bounds.BottomLeft => new Vector(0, height),
                Bounds.Bottom => new Vector(width / 2f, height),
                Bounds.BottomRight => new Vector(width, height),

                _ => new Vector(0, 0)
            };
        }

        public Vector[] CreateBounds(float width, float height)
        {
            Vector[] vList = new Vector[9];
            for (int i = 0; i < 9; i++)
            {
                vList[i] = BoundToVector((Bounds)i, width, height);
            }

            return vList;
        }
    }

    /// <summary>
    /// Aesthetic data.
    /// </summary>
    public class Renderer : ICopy<Renderer>
    {
        private static readonly BoundFunc _boundFuncs = new();

        public static readonly Texture2D DefaultTexture = FriteModel.MonoGame.CreateTexture(FriteModel.MonoGame.instance.GraphicsDevice, 2, 2, Graphics.Color.White);
        private byte _a = 255;

        /// <summary>
        /// Transparence (0-1)
        /// </summary>
        public float Alpha
        {
            get
            {
                return _a / 255f;
            }
            set
            {
                _a = (byte)(MathF.Max(MathF.Min(value, 1), 0) * 255);
            }
        }

        public Renderer()
        {
            _bounds = _boundFuncs.CreateBounds(2, 2);
            _texture = DefaultTexture;
        }

        public Renderer Copy()
        {
            Renderer r = new()
            {
                _texture = DefaultTexture,
                Color = this.Color.Copy(),
                hide = this.hide
            };
            return r;
        }

        private Vector[] _bounds;

        /// <summary>
        /// Gets the 9 bounds of the texture
        /// </summary>
        /// <returns>an array of 9 Vector</returns>
        public Vector[] GetTextureBounds()
        {
            return _bounds;
        }

        private Texture2D _texture;

        /// <summary>
        /// Texture.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return this._texture;
            }
            set
            {
                _texture = value;
                _bounds = _boundFuncs.CreateBounds(value.Width, value.Height);
            }
        }

        /// <summary>
        /// Color.
        /// </summary>
        public Graphics.Color Color = new (1, 1, 1);

        /// <summary>
        /// can't be draw if true;
        /// </summary>
        public bool hide = false;

        public override bool Equals(object obj)
        {
            if (obj is Renderer)
            {
                Renderer re = obj as Renderer;
                return this._texture.Equals(re._texture) && this.Color == re.Color
                    && this._a == re._a && this.hide == re.hide;
            }
            return false;
        }

        public override string ToString()
        {
            if (Texture is null)
            {
                return "Renderer (texture: null, color:" + Color.RGB.ToString() + ")";
            }
            else { return "Renderer (texture: true, color:" + Color.RGB.ToString() + ")"; }
        }
    }

    public enum Bounds
    {
        TopLeft, Top, TopRight,
        Left, Center, Right,
        BottomLeft, Bottom, BottomRight,
    }
}
