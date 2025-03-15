using FriteCollection.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection.Entity;

interface ICopy<T>
{
    /// <summary>
    /// Fait une copie.
    /// </summary>
    public T Copy();
}

/// <summary>
/// Permet de décrire une entité dans l'espace.
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
            Position = Position,
            Scale = Scale,
            _eGridOrigin = _eGridOrigin,
            _eCenterPoint = _eCenterPoint,
            rotation = rotation,
            _ui = _ui
        };
    }

    internal Vector GetScreenPosition()
    {
        return
            new Vector(
                (Position.x - Camera.Position.x) * Camera.zoom
                + GameManager.Instance.screenBounds[(int)GridOrigin].x,
                -((Position.y - Camera.Position.y) * Camera.zoom)
                + GameManager.Instance.screenBounds[(int)GridOrigin].y);
    }

    private Bounds _eGridOrigin;

    /// <summary>
    /// Origine du repère.
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
    /// Centre de position.
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
    /// Position.
    /// </summary>
    public Vector Position;

    /// <summary>
    /// Taille.
    /// </summary>
    /// <remarks>Les tailles négatives sont prises en charges.</remarks>
    public Vector Scale;

    /// <summary>
    /// Rotation.
    /// </summary>
    public float rotation;

    private bool _ui = false;

    public override bool Equals(object obj)
    {
        if (obj is Space)
        {
            Space sp = obj as Space;
            return Scale == sp.Scale && Position == sp.Position
                && rotation == sp.rotation && _eCenterPoint == sp._eCenterPoint
                && _eGridOrigin == sp._eGridOrigin && _ui == sp._ui;
        }
        return false;
    }

    public override string ToString()
    {
        return "Transform (position:" + Position.ToString() + ", scale:" + Scale.ToString() + ", direction:" + rotation + ")";
    }
}

internal static class BoundFunc
{
    internal static Vector BoundToVector(Bounds b, float width, float height)
    {
        return b switch
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

    internal static Vector[] CreateBounds(float width, float height)
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
/// Permet de décrire l'apparence d'une entité.
/// </summary>
public class Renderer : ICopy<Renderer>
{
    internal static Texture2D _defaultTexture;
    public static Texture2D DefaultTexture => _defaultTexture;
    private byte _a = 255;

    private float layer;
    internal float GetLayer() => layer;

    public short Layer
    {
        get
        {
            return (short)((layer * 2000) - 1000);
        }

        set
        {
            if (value > 1000) throw new ArgumentOutOfRangeException("value cannot be greater than 1000");
            if (value < -1000) throw new ArgumentOutOfRangeException("value cannot be less than -1000");
            layer = (value + 1000f) / 2000f;
        }
    }

    public static Texture2D CreateCircleTexture(int width)
    {
        Texture2D tex = new Texture2D(GameManager.Instance.GraphicsDevice, width, width);
        Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * width];
        for (int i = 0; i < width; i += 1)
        {
            for (int j = 0; j < width; j += 1)
            {
                if (float.Sqrt(float.Pow(i - (width / 2), 2) + float.Pow(j - (width / 2), 2)) <= width / 2)
                {
                    data[i + (j * width)] = Microsoft.Xna.Framework.Color.White;
                }
                else
                {
                    data[i + (j * width)] = Microsoft.Xna.Framework.Color.Transparent;
                }
            }
        }
        tex.SetData<Microsoft.Xna.Framework.Color>(data);
        return tex;
    }

    public static Texture2D CreateCircleTexture(int width, int holeSize)
    {
        Texture2D tex = new Texture2D(GameManager.Instance.GraphicsDevice, width, width);
        Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * width];
        for (int i = 0; i < width; i += 1)
        {
            float a = float.Pow(i - (width / 2), 2);
            for (int j = 0; j < width; j += 1)
            {
                float d = float.Sqrt(a + float.Pow(j - (width / 2), 2));
                if (d <= width / 2 && d >= holeSize / 2)
                {
                    data[i + (j * width)] = Microsoft.Xna.Framework.Color.White;
                }
                else
                {
                    data[i + (j * width)] = Microsoft.Xna.Framework.Color.Transparent;
                }
            }
        }
        tex.SetData<Microsoft.Xna.Framework.Color>(data);
        return tex;
    }

    public static Texture2D CreateFrameTexture(int width, int height, ushort borderSize)
    {
        Texture2D tex = new Texture2D(GameManager.Instance.GraphicsDevice, width, width);
        Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[width * width];
        for (int i = 0; i < width; i += 1)
        {
            float a = float.Pow(i - (width / 2), 2);
            for (int j = 0; j < height; j += 1)
            {
                if (i < borderSize || j < borderSize || width - i < borderSize + 1 || height - j < borderSize + 1)
                {
                    data[i + (j * width)] = Microsoft.Xna.Framework.Color.White;
                }
                else
                {
                    data[i + (j * width)] = Microsoft.Xna.Framework.Color.Transparent;
                }
            }
        }
        tex.SetData<Microsoft.Xna.Framework.Color>(data);
        return tex;
    }


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
        _bounds = BoundFunc.CreateBounds(2, 2);
        _texture = _defaultTexture;
        shadow = true;
        layer = 0.5f;
    }

    public bool shadow;

    public static RenderTarget2D CreateRenderTarget(int width, int height)
    {
        return new RenderTarget2D(GameManager.Instance.GraphicsDevice, width, height);
    }

    public Renderer Copy()
    {
        Renderer r = new()
        {
            _texture = _defaultTexture,
            Color = Color.Copy(),
            hide = hide
        };
        return r;
    }

    private Vector[] _bounds;

    /// <summary>
    /// Gets the 9 bounds of the texture
    /// </summary>
    /// <returns>an array of 9 Vector</returns>
    internal Vector[] GetTextureBounds()
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
            return _texture;
        }
        set
        {
            _texture = value;
            _bounds = BoundFunc.CreateBounds(value.Width, value.Height);
        }
    }

    /// <summary>
    /// Color.
    /// </summary>
    public Graphics.Color Color = new(255, 255, 255);

    /// <summary>
    /// Masquer.
    /// </summary>
    public bool hide = false;

    public override bool Equals(object obj)
    {
        if (obj is Renderer)
        {
            Renderer re = obj as Renderer;
            return _texture.Equals(re._texture) && Color == re.Color
                && _a == re._a && hide == re.hide;
        }
        return false;
    }

    public override string ToString()
    {
        if (Texture is null)
        {
            return "Renderer (texture: NULL, color:" + Color.RGB.ToString() + ")";
        }
        else { return "Renderer (texture: true, color:" + Color.RGB.ToString() + ")"; }
    }
}
