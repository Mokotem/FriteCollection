using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FriteCollection.Scripting;

public static class Input
{
    private static KeyboardState _kbstate, _prekbstate;
    private static MouseState _mouseState, _pMouseState;

    public static KeyboardState KB => _kbstate;
    public static KeyboardState KBP => _prekbstate;

    public static class Mouse
    {
        public static MouseState State => _mouseState;
        public static MouseState StateP => _pMouseState;

        private static Bounds _origin = Bounds.Center;
        public static Bounds GridOrigin
        {
            get => _origin;
            set
            {
                _origin = value;
            }
        }

        public static Vector2 GetVectorPosition(in Environment envi)
        {
            Vector2 offset = new Vector2(-envi.Rect.X, -envi.Rect.Y);
            offset += new Vector2(State.Position.X, -State.Position.Y);
            return new Vector2(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height)) + envi.Bounds[(int)_origin];
        }
        public static Vector2 GetVectorPosition(in Environment envi, Vector2 mouse)
        {
            Vector2 offset = new Vector2(-envi.Rect.X, -envi.Rect.Y);
            offset += mouse;
            return new Vector2(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height));
        }

        public static Point GetPointPosition(in Environment envi)
        {
            Point offset = new Point(-envi.Rect.X, -envi.Rect.Y);
            offset += new Point(State.Position.X, State.Position.Y);
            return new Point(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height));
        }


        public static Point GetPointPosition(in Environment envi, Point mouse)
        {
            Point offset = new Point(-envi.Rect.X, -envi.Rect.Y);
            offset += mouse;
            return new Point(offset.X / (envi.Rect.Width / envi.Target.Width),
                offset.Y / (envi.Rect.Height / envi.Target.Height));
        }
    }

    public static void SetStates(KeyboardState kbs, MouseState mss)
    {
        _prekbstate = _kbstate;
        _pMouseState = _mouseState;
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

/// <summary>
/// Données temporels de la scène.
/// </summary>
public static class Time
{
    private static float _sp = 1f;
    private static float _frameTime = 1f / GameManager.Fps;
    private static double reset = 0d;

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

    internal static void UpdateGameTime(in GameTime gt)
    {
        timer = (gt.TotalGameTime.TotalMicroseconds / 1000000d) - reset;
        dt = gt.ElapsedGameTime.TotalMicroseconds / 1000000d;
        dtf = (float)gt.ElapsedGameTime.TotalMilliseconds / 1000f;
    }

    internal static void Reset(in GameTime gt)
    {
        reset = gt.TotalGameTime.TotalMicroseconds / 1000000d;
    }

    private static double timer, dt;
    private static float dtf;

    /// <summary>
    /// Temps écoulé depuis l'execution.
    /// </summary>
    public static double Timer => timer;

    /// <summary>
    /// Temps écoulé depuis la dernière frame.
    /// </summary>
    public static double Delta => dt * _sp;
    public static float DeltaF => dtf * _sp;
}

public abstract class Executable : IDisposable
{
    public virtual bool Active { get; }
    public virtual void BeforeStart() { }
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
                if (_currentScripts.Count == 0)
                    _currentScripts = new List<Executable>() { this };
                else
                {
                    int i = 0;
                    while (i < _currentScripts.Count && _currentScripts[i].layer < this.layer)
                    {
                        i++;
                    }

                    _currentScripts.Insert(i, this);
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
    public virtual void OnWindowResize() { }

    public Script(object scene, bool active = true)
    {
        _attributedScenes = (int)scene;
        _active = active;
        GameManager.Instance.Window.ClientSizeChanged += (object sender, EventArgs args) => { OnWindowResize(); };
    }

    public static T GetScript<T>() where T: Script
    {
        foreach(Script s in GameManager.Instance.CurrentExecutables)
        {
            if (s.GetType().Name == typeof(T).Name)
                return s as T;
        }
        throw new Exception("'" + typeof(T).Name + "' scripte n'existe pas dans cette scène.");
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
        foreach(Executable exe in GameManager.Instance.CurrentExecutables.ToArray())
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
