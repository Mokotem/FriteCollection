using System;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection.Scripting
{
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
        public Module.Scenes StartScene;

        public string WindowName;
    }

    public abstract class GetSeetings
    {
        public static GameSettings Settings = new();

        public GetSeetings()
        {
            Settings.WindowWidth = 800;
            Settings.WindowHeight = 600;
            Settings.GameFixeWidth = 800;
            Settings.GameFixeHeight = 600;
            Settings.FPS = 60;
            Settings.FullScreen = false;
            Settings.PixelArtDrawing = false;
            Settings.WindowName = "";
        }
    }

    /// <summary>
    /// Modifiez vos paramètres en appelant la variable 'settings'.
    /// </summary>
    public interface ISetGameSettings
    {
        /// <summary>
        /// Modifiez vos paramètres en appelant la variable 'settings'.
        /// </summary>
        void SetGameSettings();
    }

    /// <summary>
    /// Données temporels de la scène.
    /// </summary>
    public abstract class Time
    {
        private static float _frameTime = 1f / GetSeetings.Settings.FPS;

        /// <summary>
        /// Constante : temps d'une frame.
        /// </summary>
        public static float FrameTime
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
            get { return FriteModel.MonoGame.instance._targetTimer; }
        }

        /// <summary>
        /// Temps écoulé depuis l'execution.
        /// </summary>
        public static float Timer
        {
            get { return FriteModel.MonoGame.instance._timer; }
        }

        /// <summary>
        /// Temps écoulé depuis la dernière frame.
        /// </summary>
        public static float Delta
        {
            get { return FriteModel.MonoGame.instance._delta; }
        }
    }

    public class List<T> : IEnumerable
    {
        private T[] _values = new T[0];

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
            int i = 0;
            while (i < _values.Length && !value.Equals(_values[i]))
                i++;
            if (i < Count)
            {
                for (int j = i; j < Count - 1; j++)
                {
                    _values[j] = _values[j + 1];
                }
                Resize(-1);
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
            _values = Array.Empty<T>();
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
                return _index < _values.Length;
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
        public virtual void BeforeStart() { }
        public virtual void Start() { }
        public virtual void AfterStart() { }

        public virtual void BeforeUpdate() { }
        public virtual void Update() { }
        public virtual void AfterUpdate() { }

        public virtual void Draw() { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void DrawAdditive() { }
        public virtual void DrawAdditive(SpriteBatch spriteBatch) { }

        public virtual void DrawUI() { }
        public virtual void DrawUI(SpriteBatch spriteBatch) { }

        public virtual void Dispose() { }


        private byte layer = 0;

        public byte Layer
        {
            set
            {
                if (FriteModel.MonoGame.instance.CurrentExecutables.Contains(this) == true)
                {
                    List<Executable> _currentScripts = FriteModel.MonoGame.instance.CurrentExecutables;
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
        public Script(Module.Scenes scene)
        {
            _attributedScene = scene;
        }

        private Module.Scenes _attributedScene;

        public Module.Scenes AttributedScene
        {
            get
            {
                return _attributedScene;
            }
        }
    }

    public abstract class Clone : Executable
    {
        private static ulong _count = 0;
        private readonly ulong _id;

        public ulong ID => _id;

        public Clone()
        {
            _count++;
            _id = _count;
            FriteModel.MonoGame.instance.CurrentExecutables.Add(this);
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
            FriteModel.MonoGame.instance.CurrentExecutables.Remove(this);
        }
    }
}
