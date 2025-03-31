using FriteCollection.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Outils de base de programmation utilisés par les sous-modules de FriteCollection.
/// </summary>
namespace FriteCollection
{
    public interface IDraw
    {
        /// <summary>
        /// Dessine l'entité à l'écran.
        /// </summary>
        public void Draw();
    }

    /// <summary>
    /// Type de base de tous les types du genre tableau, listes, piles, ...
    /// </summary>
    public abstract class ArrayObject<T>
    {
        private protected T[] _values;

        private protected struct ListEnum : IEnumerator
        {
            private readonly T[] _values;
            private long _index;

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

        private protected void Resize(short scale)
        {
            T[] _destination = new T[Lenght + scale];
            if (scale >= 0)
            {
                System.Array.Copy(_values, _destination, Lenght);
                _values = _destination;
            }
            else
            {
                for (uint i = 0; i < Lenght - 1; i++)
                    _destination[i] = _values[i];
                _values = _destination;
            }
        }

        private protected uint Lenght => _values is null ? 0 : (uint)_values.Length;

        /// <summary>
        /// Vide le tableau.
        /// </summary>
        public void Clear()
        {
            _values = null;
        }

        /// <summary>
        /// Converti en tableau.
        /// </summary>
        public T[] ToArray()
        {
            return _values;
        }

        public override string ToString()
        {
            if (Lenght > 0)
            {
                string result = this.GetType().Name + " " + typeof(T).Name + " [";
                for (uint i = 0; i < Lenght - 1; i++)
                    result += _values[i].ToString() + ", ";
                return result + _values[Lenght - 1].ToString() + "]";
            }
            else
                return this.GetType().Name + " " + typeof(T).Name + " []";
        }
    }

    public class Queue<T> : ArrayObject<T>, ICopy<Queue<T>>
    {
        public Queue(params T[] elements)
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

        public uint Lenght => base.Lenght;

        /// <summary>
        /// Lire le haut de la pile.
        /// </summary>
        public T Read
        {
            get
            {
                return _values[0];
            }
        }

        public void Add(T value)
        {
            if (base.Lenght <= 0)
            {
                _values = new T[1] { value };
            }
            else
            {
                Resize(1);
                _values[base.Lenght - 1] = value;
            }
        }

        public bool Remove()
        {
            if (base.Lenght > 0)
            {
                for (uint j = 0; j < base.Lenght - 1; j++)
                {
                    _values[j] = _values[j + 1];
                }
                Resize(-1);
                return true;
            }
            return false;
        }

        public Queue<T> Copy()
        {
            if (_values is null)
            {
                return new Queue<T>();
            }
            Queue<T> list = new Queue<T>();
            list._values = (T[])this._values.Clone();
            return list;
        }
    }


    /// <summary>
    /// Une pile d'éléments.
    /// </summary>
    public class Stack<T> : ArrayObject<T>, ICopy<Stack<T>>
    {
        public Stack(params T[] elements)
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

        /// <summary>
        /// Hauteur de la pile. (longueur de la liste)
        /// </summary>
        public uint Height => Lenght;

        /// <summary>
        /// Lire le haut de la pile.
        /// </summary>
        public T Read
        {
            get
            {
                return _values[Lenght - 1];
            }
        }

        public void Add(T value)
        {
            if (Lenght <= 0)
            {
                _values = new T[1] { value };
            }
            else
            {
                Resize(1);
                _values[Lenght - 1] = value;
            }
        }

        /// <summary>
        /// Supprime le dernier élément de la pile.
        /// </summary>
        /// <returns>Si un élément a bien été enlevé.</returns>
        public bool Remove()
        {
            if (Lenght > 0)
            {
                Resize(-1);
                return true;
            }
            return false;
        }

        public Stack<T> Copy()
        {
            if (_values is null)
            {
                return new Stack<T>();
            }
            Stack<T> list = new Stack<T>();
            list._values = (T[])this._values.Clone();
            return list;
        }
    }

    /// <summary>
    /// Environement de dessin.
    /// </summary>
    public class Environment : IDraw
    {
        public readonly Rectangle rect;
        public readonly RenderTarget2D target;
        public readonly Vector[] bounds;
        public Environment(Rectangle t, RenderTarget2D r)
        {
            rect = t;
            target = r;
            bounds = BoundFunc.CreateBounds(r.Width, r.Height);
        }

        public void Draw()
        {
            GameManager.Instance.SpriteBatch.Draw(target, rect, Color.White);
        }

        public void Draw(float depth)
        {
            GameManager.Instance.SpriteBatch.Draw(target, rect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, depth);
        }
    }

    /// <summary>
    /// Représente un Vecteur, un Point de l'espace.
    /// </summary>
    public struct Vector
    {
        public float x;
        public float y;

        /// <summary>
        /// Retourne la distance entre deux points.
        /// </summary>
        /// <remarks>exemple: (0, 0) | (1, 1) => 1,414</remarks>
        public static float operator |(Vector v1, Vector v2)
        {
            float d1 = v1.x - v2.x;
            float d2 = v1.y - v2.y;
            return float.Sqrt((d1 * d1) + (d2 * d2));
        }

        /// <summary>
        /// Produit scalaire
        /// </summary>
        public static float operator ^(Vector v1, Vector v2)
        {
            return (v1.x * v2.x) + (v1.y * v2.y);
        }

        /// <summary>
        /// Retourne l'angle formé par les deux points
        /// </summary>
        /// <remarks>exemple: (0, 0) et (1, 1) => 45</remarks>
        public static float operator &(Vector v1, Vector v2)
        {
            if (v2.x - v1.x == 0)
                return v1.y < v2.y ? 90f : -90f;
            else
                return float.Atan((v2.y - v1.y) / (v2.x - v1.x)) * (180f / System.MathF.PI) + (v1.x < v2.x ? 0 : 180);
        }

        /// <summary>
        /// Retourne la norme du Vecteur
        /// </summary>
        /// <remarks>exemple: (1, 1) => 1,414</remarks>
        public float Lenght => float.Sqrt((x * x) + (y * y));

        /// <summary>
        /// (0, 0)
        /// </summary>
        public static Vector Zero => new Vector(0, 0);

        /// <summary>
        /// (x, y)
        /// </summary>
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector(float s)
        {
            this.x = s;
            this.y = s;
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
                return x == ((Vector)value).x && y == ((Vector)value).y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
        public static bool operator ==(Vector v, Vector n) { return v.Equals(n); }
        public static bool operator !=(Vector v, Vector n) { return !v.Equals(n); }

        public override string ToString()
        {
            return "Vector(" + x + ", " + y + ")";
        }

        internal Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// Comme un vecteur, mais dans les entiers relatifs.
    /// </summary>
    public struct Point
    {
        public int i;
        public int j;

        /// <summary>
        /// (0, 0)
        /// </summary>
        public static Point Zero => new Point(0, 0);

        public Point(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        public Point(int s)
        {
            this.i = s;
            this.j = s;
        }

        /// <summary>
        /// (0, 0)
        /// </summary>
        public Point()
        {
            i = 0;
            j = 0;
        }

        public float Lenght => float.Sqrt((i * i) + (j * j));

        public static Point operator +(Point p, int n) { return new Point(p.i + n, p.j + n); }
        public static Point operator -(Point p, int n) { return new Point(p.i - n, p.j - n); }
        public static Point operator *(Point p, int n) { return new Point(p.i * n, p.j * n); }

        public static Point operator +(Point p1, Point p2) { return new Point(p1.i + p2.i, p1.j + p2.j); }
        public static Point operator -(Point p1, Point p2) { return new Point(p1.i - p2.i, p1.j - p2.j); }
        public static Point operator *(Point p1, Point p2) { return new Point(p1.i * p2.i, p1.j * p2.j); }

        public override bool Equals(object value)
        {
            if (value is Point)
            {
                return i == ((Point)value).i && j == ((Point)value).j;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return i.GetHashCode() ^ j.GetHashCode();
        }
        public static bool operator ==(Point v, Point n) { return v.Equals(n); }
        public static bool operator !=(Point v, Point n) { return !v.Equals(n); }

        public override string ToString()
        {
            return "Point(" + i + ", " + j + ")";
        }

        internal Vector2 ToVector2()
        {
            return new Vector2(i, j);
        }
    }
}