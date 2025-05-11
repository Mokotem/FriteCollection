using FriteCollection.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Outils de base de programmation utilisés par les sous-modules de FriteCollection.
/// </summary>
namespace FriteCollection;

public interface IDraw
{
    /// <summary>
    /// Dessine l'entité à l'écran.
    /// </summary>
    public void Draw();
}

/// <summary>
/// Environement de dessin.
/// </summary>
public class Environment : IDraw
{
    public Rectangle Rect { get; private set; }
    public RenderTarget2D Target { get; private set; }
    public Vector2[] Bounds { get; private set; }
    public Environment(Rectangle t, RenderTarget2D r)
    {
        Rect = t;
        Target = r;
        Bounds = BoundFunc.CreateBounds(r.Width, r.Height);
    }

    public void Edit(Rectangle t, RenderTarget2D r)
    {
        Rect = t;
        Target = r;
        Bounds = BoundFunc.CreateBounds(r.Width, r.Height);
    }

    public void Draw()
    {
        GameManager.Instance.SpriteBatch.Draw(Target, Rect, Color.White);
    }

    public void Draw(float depth)
    {
        GameManager.Instance.SpriteBatch.Draw(Target, Rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, depth);
    }
}
