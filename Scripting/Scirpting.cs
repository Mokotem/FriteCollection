using FriteCollection.Entity;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FriteCollection.Scripting;

public static class Input
{
    private static KeyboardState _kbstate, _prekbstate;
    private static MouseState _mouseState;

    public static KeyboardState KB => _kbstate;
    public static KeyboardState KBP => _prekbstate;

    public static class Mouse
    {
        public static MouseState Sate => _mouseState;

        private static Bounds _origin = Bounds.Center;
        public static Bounds GridOrigin
        {
            get => _origin;
            set
            {
                _origin = value;
            }
        }

        public static Vector Position
        {
            get
            {
                Vector v = BoundFunc.BoundToVector(_origin, Screen.widht, Screen.height);
                return new Vector(_mouseState.Position.X - v.x, -_mouseState.Position.Y + v.y);
            }
        }
    }

    internal static void SetStates(KeyboardState kbs, MouseState mss)
    {
        _prekbstate = _kbstate;
        _kbstate = kbs;
        _mouseState = mss;
    }
}

public class Sequence : Clone
{
    public delegate float Task();
    private readonly Task[] _tasks;
    public Sequence(params Task[] tasks)
    {
        _tasks = tasks;
    }

    float timer = -1;
    uint i = 0;

    public override void BeforeUpdate()
    {
        if (i < _tasks.Length)
        {
            if (timer < 0)
            {
                timer = _tasks[i]();
                i++;
            }
            else
                timer += -Time.FixedFrameTime;
        }
        else
            Destroy();
    }
}

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
    public static double TargetTimer => GameManager.Instance._targetTimer;

    /// <summary>
    /// Temps écoulé depuis l'execution.
    /// </summary>
    public static double Timer => GameManager.Instance._timer; 

    /// <summary>
    /// Temps écoulé depuis la dernière frame.
    /// </summary>
    public static double Delta => GameManager.Instance._delta * _sp;
    public static float DeltaF => (float)GameManager.Instance._delta * _sp;
}

public abstract class Executable : IDisposable
{
    public virtual bool Active { get; }
    public virtual void Start() { }

    public virtual void BeforeUpdate() { }
    public virtual void Update() { }
    public virtual void AfterUpdate() { }

    public virtual void BeforeDraw(ref readonly SpriteBatch spriteBatch) { }
    public virtual void Draw() { }
    public virtual void AfterDraw(ref readonly SpriteBatch spriteBatch) { }

    public virtual void DrawAdditive() { }

    public virtual void DrawUI() { }
    public virtual void DrawUI(ref readonly SpriteBatch spriteBatch) { }

    public virtual void Dispose() { }
    public virtual void Load() { }


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
    public Script(object scene, bool active = true)
    {
        _attributedScenes = (int)scene;
        _active = active;
    }

    private readonly bool _active;
    public override bool Active => _active;
    private int _attributedScenes;

    public void Destroy()
    {
        this.Dispose();
        GameManager.Instance.CurrentExecutables.Remove(this);
    }

    internal int AttributedScenes
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

    public static void DestroyAll(params Type[] exepts)
    {
        foreach(Executable exe in GameManager.Instance.CurrentExecutables.Copy())
        {
            if (exe is Clone && !(exepts.Contains(exe.GetType().BaseType) || exepts.Contains(exe.GetType())))
            {
                (exe as Clone).Destroy();
            }
        }
    }

    protected virtual void OnDestroy()
    {

    }

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
        this.OnDestroy();
        this.Dispose();
        GameManager.Instance.CurrentExecutables.Remove(this);
        isdestroyed = true;
    }
}
