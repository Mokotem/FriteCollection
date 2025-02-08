using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using Microsoft.Xna.Framework.Input;
using FriteCollection.Entity;

namespace FriteCollection.Scripting;

public struct GameSettings
{
    /// <summary>
    /// Frames par secondes.
    /// </summary>
    public int FPS;

    /// <summary>
    /// Dimension de la fenêtre de teste.
    /// </summary>
    public int WindowWidth, WindowHeight;

    /// <summary>
    /// Dimension de la fenêtre de jeux.
    /// </summary>
    public int GameFixeWidth, GameFixeHeight;

    /// <summary>
    /// Plein écran
    /// </summary>
    public bool FullScreen;

    public bool PixelArtDrawing;

    /// <summary>
    /// Scène de départ.
    /// </summary>
    public byte StartScene;

    public string WindowName;
    public byte UICoef;
}

/// <summary>
/// Données temporels de la scène.
/// </summary>
public static class Time
{
    private static float _sp = 1f;
    private static float _frameTime = 1f / GameManager.Fps;

    public static float SpaceTime
    {
        get => _sp;
        set
        {
            _sp = value;
        }
    }

    /// <summary>
    /// Constante : temps d'une frame.
    /// </summary>
    public static float FrameTime
    {
        get
        {
            return _frameTime * _sp;
        }
    }

    /// <summary>
    /// Constante : temps d'une frame.
    /// </summary>
    public static float FixedFrameTime
    {
        get
        {
            return _frameTime;
        }
    }

    public static void SetFPS (float fps)
    {
        _frameTime = 1f / fps;
    }

    /// <summary>
    /// Temps écoulé visé depuis l'execution.
    /// </summary>
    public static float TargetTimer
    {
        get { return GameManager.Instance._targetTimer; }
    }

    /// <summary>
    /// Temps écoulé depuis l'execution.
    /// </summary>
    public static float Timer
    {
        get { return GameManager.Instance._timer; }
    }

    /// <summary>
    /// Temps écoulé depuis la dernière frame.
    /// </summary>
    public static float Delta
    {
        get { return GameManager.Instance._delta; }
    }
}

public static class Input
{
    private static KeyboardState _kbstate;
    private static MouseState _mouseState;

    public static KeyboardState Keyboard => _kbstate;
    public static MouseState Mouse => _mouseState;

    internal static void SetStates(KeyboardState kbs, MouseState mss)
    {
        _kbstate = kbs;
        _mouseState = mss;
    }
}

public abstract class Executable : IDisposable
{
    public virtual bool Active { get; }
    public virtual void BeforeStart() { }
    public virtual void Start() { }
    public virtual void AfterStart() { }

    public virtual void BeforeUpdate() { }
    public virtual void Update() { }
    public virtual void AfterUpdate() { }

    public virtual void Draw() { }
    public virtual void Draw(ref SpriteBatch spriteBatch) { }

    public virtual void BeforeDraw() { }
    public virtual void BeforeDraw(ref SpriteBatch spriteBatch) { }

    public virtual void AfterDraw() { }
    public virtual void AfterDraw(ref SpriteBatch spriteBatch) { }

    public virtual void DrawAdditive() { }

    public virtual void DrawUI() { }
    public virtual void DrawUI(ref SpriteBatch spriteBatch) { }

    public virtual void Dispose() { }


    private short layer = 0;

    public short Layer
    {
        set
        {
            if (GameManager.Instance.CurrentExecutables.Contains(this) == true)
            {
                List<Executable> _currentScripts = GameManager.Instance.CurrentExecutables;
                _currentScripts.Remove(this);

                layer = value;
                if (_currentScripts.Lenght == 0)
                    _currentScripts = new List<Executable>() { this };
                else
                {
                    uint i = 0;
                    while (i < _currentScripts.Lenght && _currentScripts[i].layer < this.layer)
                    {
                        i++;
                    }

                    _currentScripts.Add(this, i);
                }
            }
        }

        get
        {
            return layer;
        }
    }
}

public abstract class Script : Executable
{
    public Script(byte scene, bool active = true)
    {
        _attributedScenes = scene;
    }
    public override bool Active => true;
    private byte _attributedScenes;

    public byte AttributedScenes
    {
        get
        {
            return _attributedScenes;
        }
    }
}

public abstract class Clone : Executable
{
    private static ulong _count = 0;
    private readonly ulong _id;
    private bool isdestroyed = false;

    public override bool Active => true;

    public bool IsDestroyed => isdestroyed;

    public ulong ID => _id;

    public Clone()
    {
        _count++;
        _id = _count;
        GameManager.Instance.CurrentExecutables.Add(this);
        this.Start();
    }

    public override bool Equals(object obj)
    {
        if (obj is Clone)
        {
            return _id == ((Clone)obj)._id;
        }
        return false;
    }

    public void Destroy()
    {
        GameManager.Instance.CurrentExecutables.Remove(this);
        isdestroyed = true;
    }
}
