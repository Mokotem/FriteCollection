using MonoGame.Extended;
using System;
using FriteCollection.Scripting;

namespace FriteCollection.Entity
{
    public abstract class HitBox
    {
        private const int _numberOfLayers = 4;
        private readonly BoundFunc _boundFunc = new();

        private readonly static List<HitBox>[] _hitBoxesList = new List<HitBox>[_numberOfLayers]
        {
            new(),
            new(),
            new(),
            new()
        };

        public static List<HitBox> GetLayer(byte layer)
        {
            return _hitBoxesList[layer];
        }

        public static void ClearHitboxes()
        {
            _hitBoxesList[0] = new();
            _hitBoxesList[1] = new();
            _hitBoxesList[2] = new();
            _hitBoxesList[3] = new();
        }

        private static Graphics.Color[] _color = new Graphics.Color[_numberOfLayers]
        {
            new Graphics.Color(0, 1, 0),
            new Graphics.Color(0, 0, 1),
            new Graphics.Color(1, 0, 0),
            new Graphics.Color(1, 1, 0)
        };

        public static void SetLayerColor(int layer, Graphics.Color color)
        {
            _color[layer] = color.Copy();
        }


        private byte _layer = 0;
        public string tag = "";
        public bool Active = true;

        public Space Space;
        public Vector PositionOffset;

        public HitBox(Space _space, string tag = "", byte layer = 0)
        {
            Space = _space;
            this.tag = tag;
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

        private bool positionLocked = false;
        private Vector lockPosition;

        public Vector LockPosition
        {
            get
            {
                return lockPosition;
            }
            set
            {
                positionLocked = true;
                lockPosition = value;
            }
        }

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
                Space.Scale = value;
            }
        }

        public struct Collision
        {
            public Rectangle collider;
            public Sides side;
        }

        /// <summary>
        /// Vérifie si il y a collision.
        /// </summary>
        public virtual Collision[] Check(
                out Sides side,
                string tag = null,
                bool collidersEquals = false)
        {
            side = Sides.None;
            return new Collision[0];
        }

        public virtual bool CheckWith(
                HitBox collider,
                out Sides side)
        {
            side = Sides.None;
            collider = null;
            return false;
        }

        public void Destroy()
        {
            _hitBoxesList[_layer].Remove(this);
        }

        public static void Debug()
        {
            foreach (List<HitBox> list in _hitBoxesList)
                foreach (Rectangle hit in list)
                    hit.Draw();
        }

        public class Rectangle : HitBox, IDraw, ICopy<Rectangle>
        {
            public Rectangle(Space _space, string tag = "", byte layer = 0) : base(_space, tag, layer) { }
            public Vector ScaleOffset;

            public override bool Equals(object obj)
            {
                if (obj is Rectangle)
                {
                    Rectangle r = (Rectangle)obj;
                    return lockSize.Equals(r.lockSize) && lockPosition.Equals(r.lockPosition)
                        && Active == r.Active && PositionOffset.Equals(r.PositionOffset);
                }
                return false;
            }

            public Rectangle Copy()
            {
                return new Rectangle(Space.Copy())
                {
                    ScaleOffset = ScaleOffset,
                    PositionOffset = PositionOffset,
                    _layer = _layer,
                    tag = tag,
                    Active = Active,
                    lockPosition = lockPosition,
                    lockSize = lockSize,
                    sizeLocked = sizeLocked,
                    positionLocked = positionLocked
                };
            }

            private Space GetHitSpace(Rectangle r)
            {
                Space sp1 = r.Space.Copy();
                sp1.Position = r.Space.Position + r.PositionOffset;
                sp1.Scale = r.Space.Scale + r.ScaleOffset;
                if (r.sizeLocked) { sp1.Scale = r.lockSize; }
                if (r.positionLocked) { sp1.Position = r.lockPosition; }
                Vector v = _boundFunc.BoundToVector(r.Space.CenterPoint, sp1.Scale.x, sp1.Scale.y);
                sp1.Position.x -= v.x;
                sp1.Position.y += v.y;
                return sp1;
            }

            private struct PointData
            {
                public PointData(bool value, Vector distance)
                {
                    colValue = value;
                    this.distance = distance;
                }
                public bool colValue;
                public Vector distance;
            }

            private PointData PointInRange(Vector point, Space col, bool collidersEquals)
            {
                if (collidersEquals == false)
                    return new PointData
                    (
                        col.Position.x < point.x && col.Position.x + col.Scale.x > point.x
                        &&
                        col.Position.y < point.y && col.Position.y + col.Scale.y > point.y,
                        new Vector
                        (
                            MathF.Min(MathF.Abs(point.x - col.Position.x), MathF.Abs(point.x - (col.Position.x + col.Scale.x))),
                            MathF.Min(MathF.Abs(col.Position.y - point.y), MathF.Abs(point.y - (col.Position.y + col.Scale.y)))
                        )
                    );
                else return new PointData
                    (
                        col.Position.x <= point.x && col.Position.x + col.Scale.x >= point.x
                        &&
                        col.Position.y <= point.y && col.Position.y + col.Scale.y >= point.y,
                        new Vector
                        (
                            MathF.Min(MathF.Abs(point.x - col.Position.x), MathF.Abs(point.x - (col.Position.x + col.Scale.x))),
                            MathF.Min(MathF.Abs(col.Position.y - point.y), MathF.Abs(point.y - (col.Position.y + col.Scale.y)))
                        )
                    );
            }

            private Vector CenterPoint(Space s)
            {
                return new Vector(s.Position.x + s.Scale.x / 2f, s.Position.y + s.Scale.y / 2f);
            }

            private byte CountBools(bool[] b)
            {
                byte n = 0;
                foreach (bool b2 in b)
                {
                    if (b2) n++;
                }
                return n;
            }

            public override Collision[] Check(
                out Sides side,
                string tag = null,
                bool collidersEquals = false)
            {
                List<Collision> cols = new List<Collision>();
                side = Sides.None;

                Space sp1 = GetHitSpace(this);
                Vector posi1 = sp1.GetScreenPosition(includeCamera: false);
                sp1.Position = posi1;

                Vector[] points = new Vector[4]
                {
                    posi1,
                    new Vector(posi1.x + sp1.Scale.x, posi1.y),
                    new Vector(posi1.x, posi1.y + sp1.Scale.y),
                    posi1 + sp1.Scale
                };

                bool[] global = new bool[4] { false, false, false, false };

                Vector centerPoint = CenterPoint(sp1);

                foreach (HitBox col in _hitBoxesList[_layer])
                {
                    if (col is Rectangle && col.Active && (tag is null ? true : col.tag == tag) && col != this)
                    {
                        Collision c = new Collision();
                        bool[] bools = new bool[4] { false, false, false, false };
                        bool colided = false;

                        Vector closePoint = new Vector(-1, -1);

                        Rectangle hit = col as Rectangle;
                        Space sp2 = GetHitSpace(hit);
                        sp2.Position = sp2.GetScreenPosition(includeCamera: false);

                        for (int i = 0; i < 4; i++)
                        {
                            PointData pointData;
                            pointData = PointInRange(points[i], sp2, collidersEquals);
                            if (pointData.colValue == true)
                            {
                                bools[i] = true;
                                global[i] = true;
                                colided = true;

                                if (closePoint.x < 0 || closePoint.y < 0)
                                    closePoint = new Vector(MathF.Abs(pointData.distance.x), MathF.Abs(pointData.distance.y));

                                closePoint = new Vector(
                                    MathF.Min(closePoint.x, MathF.Abs(pointData.distance.x)),
                                    MathF.Min(closePoint.y, MathF.Abs(pointData.distance.y)));
                            }
                        }

                        if (colided)
                        {
                            byte n = CountBools(bools);
                            if (n == 2)
                            {
                                switch (bools[0], bools[1], bools[2], bools[3])
                                {
                                    case (true, true, false, false):
                                        c.side = Sides.Up;
                                        break;

                                    case (false, false, true, true):
                                        c.side = Sides.Down;
                                        break;

                                    case (true, false, true, false):
                                        c.side = Sides.Left;
                                        break;

                                    case (false, true, false, true):
                                        c.side = Sides.Right;
                                        break;
                                }
                            }
                            else
                            {
                                centerPoint = CenterPoint(sp1);
                                Vector centerPoint2 = CenterPoint(sp2);

                                if (closePoint.x < closePoint.y)
                                {
                                    if (centerPoint.x > centerPoint2.x)
                                        c.side = Sides.Left;
                                    else
                                        c.side = Sides.Right;
                                }
                                else
                                {
                                    if (centerPoint.y > centerPoint2.y)
                                        c.side = Sides.Up;
                                    else
                                        c.side = Sides.Down;
                                }
                            }

                            c.collider = hit;
                            cols.Add(c);
                        }
                    }
                }

                if (CountBools(global) == 2)
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

            public override bool CheckWith(
                    HitBox collider,
                    out Sides side)
            {
                Space sp1 = GetHitSpace(this);
                Vector posi1 = sp1.GetScreenPosition(includeCamera: false);
                sp1.Position = posi1;
                bool value = false;

                Vector[] points = new Vector[4]
                {
                    posi1,
                    new Vector(posi1.x + sp1.Scale.x, posi1.y),
                    new Vector(posi1.x, posi1.y + sp1.Scale.y),
                    posi1 + sp1.Scale,
                };

                Vector closePoint = new Vector(-1, -1);
                bool[] bools = new bool[4] { false, false, false, false };

                side = Sides.None;

                Rectangle hit = collider as Rectangle;
                Space sp2 = GetHitSpace(hit);
                bool colided = false;
                sp2.Position = sp2.GetScreenPosition(includeCamera: false);

                for (int i = 0; i < 4; i++)
                {
                    PointData pointData;
                    pointData = PointInRange(points[i], sp2, true);
                    if (pointData.colValue)
                    {
                        bools[i] = true;
                        colided = true;

                        if (closePoint.x < 0 || closePoint.y < 0)
                            closePoint = pointData.distance;
                        else
                            closePoint = new Vector
                            (
                                MathF.Min(closePoint.x, pointData.distance.x),
                                MathF.Min(closePoint.y, pointData.distance.y)
                            );
                    }
                }

                if (colided)
                {
                    value = true;
                    int n = CountBools(bools);
                    if (n == 2)
                    {
                        switch (bools[0], bools[1], bools[2], bools[3])
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
                    else
                    {
                        Vector centerPoint = CenterPoint(sp1);
                        Vector centerPoint2 = CenterPoint(sp2);

                        if (closePoint.x < closePoint.y)
                        {
                            if (centerPoint.x > centerPoint2.x)
                                side = Sides.Left;
                            else
                                side = Sides.Right;
                        }
                        else
                        {
                            if (centerPoint.y > centerPoint2.y)
                                side = Sides.Up;
                            else
                                side = Sides.Down;
                        }
                    }
                }


                if (points[0].x > sp2.Position.x && points[0].x <= sp2.Position.x + sp2.Scale.x
                    && points[0].y < sp2.Position.y && points[2].y > sp2.Position.y + sp2.Scale.y)
                {
                    colided = true;
                    bools = new bool[4] { true, false, true, false };
                    value = true;
                    side = Sides.Left;
                };

                if (points[1].x > sp2.Position.x && points[1].x <= sp2.Position.x + sp2.Scale.x
                    && points[1].y < sp2.Position.y && points[3].y > sp2.Position.y + sp2.Scale.y)
                {
                    colided = true;
                    bools = new bool[4] { false, true, false, true };
                    value = true;
                    side = Sides.Right;
                };

                byte numberBools = CountBools(bools);
                if (numberBools < 3)
                {
                    if (bools[0] == true && bools[2] == true)
                    {
                        side = Sides.Left;
                    }
                    else if (bools[1] == true && bools[3] == true)
                    {
                        side = Sides.Right;
                    }
                    else if (bools[0] == true && bools[1] == true)
                    {
                        side = Sides.Up;
                    }
                    else if (bools[2] == true && bools[3] == true)
                    {
                        side = Sides.Down;
                    }
                }

                return value;
            }

            // this is to draw the hitbox
            public void Draw()
            {
                if (Active)
                {
                    Space spaceCol1 = GetHitSpace(this);
                    Vector posiCol1 = spaceCol1.GetScreenPosition();
                    Vector v1 = posiCol1;
                    Vector v2 = posiCol1 + spaceCol1.Scale;

                    GameManager.Instance.SpriteBatch.DrawRectangle
                    (
                        new Microsoft.Xna.Framework.Rectangle
                        (
                            (int)v1.x, (int)v1.y, (int)(v2.x - v1.x), (int)(v2.y - v1.y)
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
}