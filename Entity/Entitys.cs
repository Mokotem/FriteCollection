using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection.Entity;

public abstract class Entity
{
    public Space Space = new Space();

    public Renderer Renderer = new Renderer();

    public virtual void Draw() { }
}

/// <summary>
/// Object.
/// </summary>
public class Object : Entity, ICopy<Object>
{
    public Object Copy()
    {
        return new()
        {
            Space = Space.Copy(),
            Renderer = Renderer.Copy()
        };
    }

    public override void Draw()
    {
        if (Renderer.hide == false)
        {
            Vector2 entPosi = Space.GetScreenPosition();
            Vector2 s = Space.Scale;
            float flipFactor = 0f;

            SpriteEffects spriteEffect = SpriteEffects.None;
            if (s.X< 0 && s.Y < 0)
            {
                flipFactor = MathF.PI * 1;
            }
            else
            {
                if (s.X< 0 && s.Y >= 0)
                {
                    spriteEffect = SpriteEffects.FlipVertically;
                    flipFactor = MathF.PI;
                }
                else if (s.Y < 0 && s.X >= 0)
                {
                    spriteEffect = SpriteEffects.FlipHorizontally;
                    flipFactor = MathF.PI;
                }
            }

            if (Renderer.shadow)
            {
                GameManager.Instance.SpriteBatch.Draw
                (
                    Renderer.Texture,
                    new Rectangle
                    (
                        (int)entPosi.X + 4,
                        (int)entPosi.Y + 4,
                        (int)MathF.Abs(s.X),
                        (int)MathF.Abs(s.Y)
                    ),
                    null,
                    Color.Black * Renderer.Alpha,
                    float.DegreesToRadians(Space.rotation) + flipFactor,
                    Renderer.GetTextureBounds()[(int)Space.CenterPoint],
                    SpriteEffects.None,
                    0.75f
                );
            }

            GameManager.Instance.SpriteBatch.Draw
            (
                Renderer.Texture,
                new Rectangle
                (
                    (int)entPosi.X,
                    (int)entPosi.Y,
                    (int)MathF.Abs(s.X),
                    (int)MathF.Abs(s.Y)
                ),
                null,
                this.Renderer.Color,
                float.DegreesToRadians(Space.rotation) + flipFactor,
                Renderer.GetTextureBounds()[(int)Space.CenterPoint],
                spriteEffect,
                Renderer.GetLayer()
            );
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Object)
        {
            return Space.Equals((obj as Object).Space)
                && Renderer.Equals((obj as Object).Renderer);
        }
        return false;
    }

    public override string ToString()
    {
        return "Object (" + Space.ToString() + ", " + Renderer.ToString() + ")";
    }
}

/// <summary>
/// Text.
/// </summary>
public class Text : Entity, ICopy<Text>
{
    private int _spacing;

    public Text Copy()
    {
        Text t = new(font)
        {
            Space = Space.Copy(),
            Renderer = Renderer.Copy(),
            Spacing = _spacing
        };
        return t;
    }

    public override void Draw()
    {
        if (Renderer.hide == false && _text != null)
        {
            Vector2 entPosi = Space.GetScreenPosition();

            foreach (Vector2 pos in new Vector2[8]
            {
                new(-1, 1),
                new(-1, -1),
                new(1, -1),
                new(1, 1),

               new(0, 1),
                new(0, -1),
                new(1, 0),
                new(-1, 0),
            })
            {
                GameManager.Instance.SpriteBatch.DrawString
            (
                font,
                _text,
                new Vector2
                (
                    entPosi.X + pos.X, entPosi.Y + pos.Y
                ),
                Color.Black,
                Space.rotation * (MathF.PI / 180f),
                GetTextBounds()[(int)Space.CenterPoint],
                1,
                SpriteEffects.None,
                0
            );
            }

            GameManager.Instance.SpriteBatch.DrawString
            (
                font,
                _text,
                new Vector2
                (
                    entPosi.X, entPosi.Y
                ),
                this.Renderer.Color,
                Space.rotation * (MathF.PI / 180f),
                GetTextBounds()[(int)Space.CenterPoint],
                1,
                SpriteEffects.None,
                0
            );
        }
    }

    private Vector2[] _bounds;
    /// <summary>
    /// Gets the 9 bounds of the text.
    /// </summary>
    /// <returns> an array of 9 Vector</returns>
    public Vector2[] GetTextBounds()
    {
        return _bounds;
    }

    private string _text = null;

    /// <summary>
    /// Text to show.
    /// </summary>
    public string Write
    {
        get { return _text; }
        set
        {
            _text = value;
            Space.Scale = new Vector2
            (
                font.MeasureString(value).X + _spacing * (value.Length - 1),
                font.MeasureString(value).Y
            );
            _bounds = BoundFunc.CreateBounds(
                font.MeasureString(value).X + _spacing * (value.Length - 1),
                font.MeasureString(value).Y
                );
        }
    }

    private SpriteFont font;

    /// <summary>
    /// Gets the font file of the Text.
    /// </summary>
    public SpriteFont Font
    {
        get
        {
            return font;
        }
    }

    private void Constructor()
    {
        _spacing = 0;
        Space.CenterPoint = Bounds.Center;
    }

    /// <summary>
    /// Creates a Text Entity.
    /// </summary>
    /// <param name="font">font file</param>
    public Text(SpriteFont font)
    {
        this.font = font;
        Constructor();
    }

    /// <summary>
    /// Creates a Text Entity.
    /// </summary>
    /// <param name="font">font file</param>
    /// <param name="text">text to show</param>
    public Text(SpriteFont font, string text)
    {
        this.font = font;
        Constructor();
        Write = text;
    }

    /// <summary>
    /// Spacing between letters.
    /// </summary>
    public float Spacing
    {
        get { return font.Spacing; }
        set
        {
            font.Spacing = value;
        }
    }

    public override string ToString()
    {
        return "Text " + Write + " : (" + Space.ToString() + ", " + Renderer.ToString() + ")";
    }
}