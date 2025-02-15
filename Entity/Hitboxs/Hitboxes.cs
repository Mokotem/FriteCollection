using MonoGame.Extended;
using System;
using System.Collections;

namespace FriteCollection.Entity.Hitboxs;

public abstract class Hitbox
{
    private const int _numberOfLayers = 3;

    private readonly static List<Hitbox>[] _hitBoxesList = new List<Hitbox>[_numberOfLayers]
    {
        new(),
        new(),
        new()
    };

    public static void ClearHitboxes()
    {
        _hitBoxesList[0] = new();
        _hitBoxesList[1] = new();
        _hitBoxesList[2] = new();
    }

    private static Graphics.Color[] _color = new Graphics.Color[_numberOfLayers]
    {
        new Graphics.Color(1, 0, 0),
        new Graphics.Color(0, 1, 0),
        new Graphics.Color(0, 0, 1)
    };

    public static void SetLayerColor(int layer, Graphics.Color color)
    {
        _color[layer] = color.Copy();
    }


    private byte _layer = 0;
    public string _tag = "";
    public bool Active = true;

    private protected readonly Space _refSpace;
    public Vector PositionOffset;

    private Hitbox(Space _space, string tag = "", byte layer = 0)
    {
        _refSpace = _space;
        this._tag = tag;
        _hitBoxesList[layer].Add(this);
        _layer = layer;
    }

    /// <summary>
    /// Only Objects in the same layer can collide.
    /// </summary>
    public byte Layer
    {
        get
        {
            return _layer;
        }

        set
        {
            _hitBoxesList[_layer].Remove(this);
            _layer = value;
            _hitBoxesList[value].Add(this);
        }
    }

    public enum Sides
    {
        Up, Down, Left, Right, None
    }

    private Vector lockPosition;
    private bool positionLocked = false;

    public virtual Vector LockPosition
    {
        get => Vector.Zero;
        set { }
    }
    public void UnlockPosition()
    {
        positionLocked = false;
    }


    public struct Collision
    {
        public Hitbox collider;
        public Sides side;
    }

    private interface ICollider
    {
        public bool Check(string tag = null);

        public Collision[] AdvancedCheck(
                out Sides side,
                string tag = null);

        public bool CheckWith(Hitbox collider);

        public Sides AdvancedCheckWith(Hitbox collider);
    }

    public void Destroy()
    {
        _hitBoxesList[_layer].Remove(this);
    }

    public static void Debug()
    {
        foreach (List<Hitbox> list in _hitBoxesList)
            foreach (Rectangle hit in list)
                hit.Draw();
    }

    public class Rectangle : Hitbox, ICollider, IDraw, IEnumerable, ICopy<Rectangle>
    {
        private struct RectangleEnum : IEnumerator
        {
            private readonly Vector _p1, _p2;
            private short index;

            public RectangleEnum(Vector p1, Vector p2)
            {
                _p1 = p1;
                _p2 = p2;
                index = -1;
            }

            public bool MoveNext()
            {
                index += 1;
                if (index > 3)
                    return false;
                return true;
            }

            void IEnumerator.Reset()
            {
                index = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return new Vector(
                        index % 2 == 0 ? _p1.x : _p2.x,
                        index < 2 ? _p1.y : _p2.y
                        );
                }
            }
        }
        public Rectangle(Space _space, string tag = "", byte layer = 0) : base(_space, tag, layer)
        {
            this.UpdatePos();
        }

        private Vector p1, p2;

        private bool sizeLocked = false;
        private Vector lockSize;
        public Vector LockSize
        {
            get
            {
                return lockSize;
            }
            set
            {
                sizeLocked = true;
                lockSize = value;
            }
        }
        public void UnlockSize()
        {
            sizeLocked = false;
        }

        public override Vector LockPosition
        {
            get
            {
                return lockPosition;
            }
            set
            {
                positionLocked = true;
                Vector p;
                Vector q = BoundFunc.BoundToVector(
                        _refSpace.GridOrigin,
                        Screen.widht,
                        Screen.height
                        );
                if (sizeLocked)
                {
                    p = BoundFunc.BoundToVector(
                        _refSpace.CenterPoint,
                        lockSize.x,
                        lockSize.y
                        );
                }
                else
                {
                    p = BoundFunc.BoundToVector(
                        _refSpace.CenterPoint,
                        _refSpace.Scale.x,
                        _refSpace.Scale.y
                        );
                }
                lockPosition = new Vector(
                    value.x + q.x - p.x,
                    -value.y + q.y - p.y
                    );
            }
        }

        private void UpdatePos()
        {
            if (positionLocked)
            {
                p1 = lockPosition;
            }
            else
            {
                p1 = _refSpace.GetScreenPosition() - BoundFunc.BoundToVector(
                        _refSpace.CenterPoint, _refSpace.Scale.x, _refSpace.Scale.y);
            }

            if (sizeLocked)
            {
                p2 = lockPosition + lockSize;
            }
            else
            {
                p2 = new Vector(p1.x + _refSpace.Scale.x, p1.y + _refSpace.Scale.y);
            }
            _centerPoint = new Vector((p1.x + p2.x) / 2f, (p1.y + p2.y) / 2f);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RectangleEnum(p1, p2);
        }


        public Rectangle Copy()
        {
            return new Rectangle(_refSpace, _tag, this._layer)
            {
                p1 = this.p1,
                p2 = this.p2
            };
        }

        private bool InRange(Vector _1, Vector _2)
        {
            if ((p1.x > _1.x && p1.x < _2.x) || (p2.x > _1.x && p2.x < _2.x))
            {
                if (p1.y > _1.y)
                {
                    if (p1.y < _2.y)
                    {
                        return true;
                    }
                }
                else if (p2.y > _1.y)
                {
                    return true;
                }
            }
            else if ((p1.y > _1.y && p1.y < _2.y) || (p2.y > _1.y && p2.y < _2.y))
            {
                if (p1.x > _1.x)
                {
                    if (p1.x < _2.x)
                    {
                        return true;
                    }
                }
                else if (p2.x > _1.x)
                {
                    return true;
                }
            }

            return false;
        }

        private bool PointInRange(Vector p, Vector _1, Vector _2, out float distX, out float distY)
        {
            if (p.x > _1.x && p.x < _2.x && p.y > _1.y && p.y < _2.y)
            {
                distX = float.Min(p.x - _1.x, _2.x - p.x);
                distY = float.Min(p.y - _1.y, _2.y - p.y);
                return true;
            }

            distX = float.PositiveInfinity;
            distY = float.PositiveInfinity;
            return false;
        }

        private bool PointInRange(Vector p, Vector _1, Vector _2)
        {
            return (p.x > _1.x && p.x < _2.x && p.y > _1.y && p.y < _2.y);
        }

        public Vector CenterPoint => _centerPoint;
        private Vector _centerPoint;

        private byte _CountBools(bool[] b)
        {
            byte n = 0;
            foreach (bool b2 in b)
            {
                if (b2) n++;
            }
            return n;
        }

        private bool _MakeCollisionWith(
            Hitbox.Rectangle hit,
            out Collision col,
            ref readonly bool[] global,
            ref Sides side)
        {
            hit.UpdatePos();
            col = new Collision();
            bool[] bools = new bool[4] { false, false, false, false };
            bool colided = false;

            Vector closePoint = new Vector(-1, -1);

            byte i = 0;
            foreach (Vector p in this)
            {
                if (PointInRange(p, hit.p1, hit.p2, out float dx, out float dy))
                {
                    bools[i] = true;
                    global[i] = true;
                    colided = true;

                    if (closePoint.x < 0 || closePoint.y < 0)
                        closePoint = new Vector(dx, dy);
                    else
                        closePoint = new Vector(
                            MathF.Min(closePoint.x, dx),
                            MathF.Min(closePoint.y, dy));
                }

                i += 1;
            }
            i = 0;
            foreach (Vector p in hit)
            {
                if (PointInRange(p, p1, p2))
                {
                    bools[3 - i] = true;
                    global[3 - i] = true;
                    colided = true;
                }

                i += 1;
            }

            if (colided)
            {
                byte n = _CountBools(bools);
                if (n == 2)
                {
                    switch (bools[0], bools[1], bools[2], bools[3])
                    {
                        case (true, true, false, false):
                            col.side = Sides.Up;
                            side = Sides.Up;
                            break;

                        case (false, false, true, true):
                            col.side = Sides.Down;
                            side = Sides.Down;
                            break;

                        case (true, false, true, false):
                            col.side = Sides.Left;
                            side = Sides.Left;
                            break;

                        case (false, true, false, true):
                            col.side = Sides.Right;
                            side = Sides.Right;
                            break;
                    }
                }
                else
                {
                    if (closePoint.x < closePoint.y)
                    {
                        if (_centerPoint.x > hit.CenterPoint.x)
                        {
                            col.side = Sides.Left;
                            side = Sides.Left;
                        }
                        else
                        {
                            col.side = Sides.Right;
                            side = Sides.Right;
                        }
                    }
                    else
                    {
                        if (_centerPoint.y > hit.CenterPoint.y)
                        {
                            col.side = Sides.Up;
                            side = Sides.Up;
                        }
                        else
                        {
                            col.side = Sides.Down;
                            side = Sides.Down;
                        }
                    }
                }

                col.collider = hit;
                return true;
            }

            return false;
        }

        private Sides _MakeCollisionWith(Hitbox.Rectangle hit)
        {
            hit.UpdatePos();
            bool[] bools = new bool[4] { false, false, false, false };
            bool colided = false;

            Vector closePoint = new Vector(-1, -1);

            byte i = 0;
            foreach (Vector p in this)
            {
                if (PointInRange(p, hit.p1, hit.p2, out float dx, out float dy))
                {
                    bools[i] = true;
                    colided = true;

                    if (closePoint.x < 0 || closePoint.y < 0)
                        closePoint = new Vector(dx, dy);
                    else
                        closePoint = new Vector(
                            MathF.Min(closePoint.x, dx),
                            MathF.Min(closePoint.y, dy));
                }

                i += 1;
            }
            i = 0;
            foreach (Vector p in hit)
            {
                if (PointInRange(p, p1, p2))
                {
                    bools[3 - i] = true;
                    colided = true;
                }

                i += 1;
            }

            if (colided)
            {
                byte n = _CountBools(bools);
                if (n == 2)
                {
                    return (bools[0], bools[1], bools[2], bools[3]) switch
                    {
                        (true, true, false, false) => Sides.Up,
                        (true, false, true, false) => Sides.Left,
                        (false, true, false, true) => Sides.Right,
                        _ => Sides.Down
                    };
                }
                else
                {
                    if (closePoint.x < closePoint.y)
                    {
                        if (_centerPoint.x > hit.CenterPoint.x)
                        {
                            return Sides.Left;
                        }
                        else
                        {
                            return Sides.Right;
                        }
                    }
                    else
                    {
                        if (_centerPoint.y > hit.CenterPoint.y)
                        {
                            return Sides.Up;
                        }
                        else
                        {
                            return Sides.Down;
                        }
                    }
                }
            }

            return Sides.None;
        }

        public bool Check(string tag = null)
        {
            this.UpdatePos();
            foreach (Hitbox col in _hitBoxesList[_layer])
            {
                if (col is Rectangle && col.Active && (tag is null ? true : col._tag == tag) && col != this)
                {
                    Rectangle hit = col as Rectangle;
                    hit.UpdatePos();

                    if (InRange(hit.p1, hit.p2))
                        return true;
                }
            }

            return false;
        }

        public bool CheckWith(Hitbox col)
        {
            if (col is Rectangle && col.Active && col != this)
            {
                this.UpdatePos();
                Rectangle hit = col as Rectangle;
                hit.UpdatePos();
                return InRange(hit.p1, hit.p2);
            }
            return false;
        }

        public Collision[] AdvancedCheck(
            out Sides side,
            string tag = null)
        {
            this.UpdatePos();
            List<Collision> cols = new List<Collision>();
            side = Sides.None;

            bool[] global = new bool[4] { false, false, false, false };

            foreach (Hitbox col in _hitBoxesList[_layer])
            {
                if (col is Rectangle && col.Active && (tag is null ? true : col._tag == tag)
                    && col != this)
                {
                    Rectangle hit = col as Rectangle;
                    if (_MakeCollisionWith(hit, out Collision c, ref global, ref side))
                    {
                        cols.Add(c);
                    }
                }
            }

            if (_CountBools(global) == 2)
            {
                switch (global[0], global[1], global[2], global[3])
                {
                    case (true, true, false, false):
                        side = Sides.Up;
                        break;

                    case (false, false, true, true):
                        side = Sides.Down;
                        break;

                    case (true, false, true, false):
                        side = Sides.Left;
                        break;

                    case (false, true, false, true):
                        side = Sides.Right;
                        break;
                }
            }

            return cols.ToArray();
        }

        public Sides AdvancedCheckWith(Hitbox col)
        {
            this.UpdatePos();
            Vector centerPoint = this.CenterPoint;
            if (col is Rectangle && col.Active && col != this)
            {
                return _MakeCollisionWith(col as Rectangle);
            }

            return Sides.None;
        }

        public void Draw()
        {
            if (Active)
            {
                UpdatePos();
                GameManager.Instance.SpriteBatch.DrawRectangle
                (
                    new Microsoft.Xna.Framework.Rectangle
                    (
                        (int)p1.x, (int)p1.y, (int)(p2.x - p1.x), (int)(p2.y - p1.y)
                    ),
                    new Microsoft.Xna.Framework.Color
                    (
                        _color[_layer].RGB.R, _color[_layer].RGB.G, _color[_layer].RGB.B
                    ),
                    1
                );
            }
        }
    }
}