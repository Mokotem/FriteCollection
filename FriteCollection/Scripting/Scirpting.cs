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

public class List<T> : IEnumerable, ICopy<List<T>>
{
    public List<T> Copy()
    {
        if (_values is null)
        {
            return new List<T>();
        }
        List<T> list = new List<T>();
        list._values = (T[])this._values.Clone();
        return list;
    }

    public static List<T> operator + (List<T> a, List<T> b)
    {
        List<T> r = a.Copy();
        foreach (T t in b)
            r.Add(t);
        return r;
    }

    private T[] _values = new T[0];

    public T Last => _values[this.Count - 1];

    public List(params T[] elements)
    {
        if (elements.Length <= 0)
        {
            _values = new T[0];
        }
        else
        {
            _values = elements;
        }
    }

    public bool Contains(T element)
    {
        foreach (T value in _values)
        {
            if (element.Equals(value))
                return true;
        }
        return false;
    }

    public uint Index(T element)
    {
        for (uint i = 0; i < Count; i ++)
        {
            if (_values[i].Equals(element))
                return i;
        }
        
        throw  new IndexOutOfRangeException();
    }

    private void Resize(long scale)
    {
        T[] _destination = new T[Count + scale];
        if (scale >= 0)
        {
            Array.Copy(_values, _destination, Count);
            _values = _destination;
        }
        else
        {
            for (uint i = 0; i < Count - 1; i ++)
                _destination[i] = _values[i];
            _values = _destination;
        }
    }

    public void Add(T value)
    {
        if (Count <= 0)
        {
            _values = new T[1] { value };
        }
        else
        {
            Resize(1);
            _values[Count - 1] = value;
        }
    }

    public void Add(T value, uint index)
    {
        if (index >= Count - 1)
        {
            this.Add(value);
        }
        else
        {
            Resize(1);
            for (uint i = Count - 2; i > index && i >= 0; i--)
            {
                _values[i + 1] = _values[i];
            }
            _values[index + 1] = _values[index];
            _values[index] = value;
        }
    }

    public uint Count => _values is null ? 0 : (uint)_values.Length;

    public bool Remove(T value)
    {
        if (_values is not null)
        {
            uint i = 0;
            while (i < _values.Length && !value.Equals(_values[i]))
                i++;
            if (i < Count)
            {
                RemoveIndex(i);
                return true;
            }
        }
        return false;
    }

    public bool Remove()
    {
        if (Count > 0)
        {
            RemoveIndex(Count - 1);
            return true;
        }
        return false;
    }

    public bool RemoveIndex(uint index)
    {
        if (index < Count)
        {
            for (uint j = index; j < Count - 1; j++)
            {
                _values[j] = _values[j + 1];
            }
            Resize(-1);
            return true;
        }
        return false;
    }

    public T this[uint index]
    {
        get
        {
            return _values[index];
        }

        set
        {
            _values[index] = value;
        }
    }

    public IEnumerator GetEnumerator()
    {
        return new ListEnum(_values);
    }

    public void Clear()
    {
        if (_values is not null)
        {
            for (uint i = 0; i < _values.Length; i++)
            {
                _values[i] = default(T);
            }
        }
        _values = null;
    }

    struct ListEnum : IEnumerator
    {
        private T[] _values;
        private int _index;

        public ListEnum(T[] v)
        {
            _values = v;
            _index = -1;
        }

        bool IEnumerator.MoveNext()
        {
            if (_values is null) return false;
            _index++;
            return _values is null ? false : _index < _values.Length;
        }

        void IEnumerator.Reset()
        {
            _index = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return _values[_index];
            }
        }
    }

    public T[] ToArray()
    {
        return _values;
    }

    public override string ToString()
    {
        if (Count > 0)
        {
            string result = "[";
            for (uint i = 0; i < Count - 1; i++)
                result += this[i].ToString() + ", ";
            return result + this[Count - 1].ToString() + "]";
        }
        else
            return "[]";
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
                if (_currentScripts.Count == 0)
                    _currentScripts = new List<Executable>() { this };
                else
                {
                    uint i = 0;
                    while (i < _currentScripts.Count && _currentScripts[i].layer < this.layer)
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
