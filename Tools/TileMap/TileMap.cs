using FriteCollection.Entity.Hitboxs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection.Tools.TileMap;

public class TileSet : IDisposable
{
    private Hitbox.Rectangle[,] _hitReplaces;
    private Entity.Object[,] _entReplaces;
    public readonly List<Entity.Object> entities;
    public List<int> _dontDraw = new List<int>();
    public int Xlenght { get; private set; }
    public int Ylenght { get; private set; }

    public Hitbox.Rectangle[,] ReplaceHitbox
    {
        get
        {
            return _hitReplaces;
        }
    }

    public Entity.Object[,] ReplaceEntity
    {
        get
        {
            return _entReplaces;
        }
    }

    public void DontDraw(ushort i, ushort j)
    {
        _dontDraw.Add(i + j * Xlenght);
    }

    public TileSet(Texture2D texture, Point tileSize, Point tileSeparation, Point tileMargin)
    {
        _tileSize = tileSize;
        _tileSeparation = tileSeparation;
        _tileMargin = tileMargin;
        Apply();
        entities = new List<Entity.Object>();
        _texture = texture;
        sheet = new Point(texture.Width, texture.Height);
    }
    private Texture2D _texture;
    public Texture2D Texture
    {
        get { return _texture; }
    }

    private void Apply()
    {
        Xlenght =
        (sheet.i + _tileSeparation.i) / (_tileSize.i + _tileSeparation.i);

        Ylenght =
            (sheet.j + _tileSeparation.j) / (_tileSize.j + _tileSeparation.j);

        _hitReplaces = new Hitbox.Rectangle[Xlenght, Ylenght];
        _entReplaces = new Entity.Object[Xlenght, Ylenght];
    }
    public readonly Point sheet;
    private Point _tileSize;
    public Point TileSize
    {
        get { return _tileSize; }
        set
        {
            _tileSize = value;
            Apply();
        }
    }

    private Point _tileSeparation = new(0, 0);
    public Point TileSeparation
    {
        get { return _tileSeparation; }
        set
        {
            _tileSeparation = value;
            Apply();
        }
    }
    private Point _tileMargin = new(0, 0);
    public Point TileMargin
    {
        get { return _tileMargin; }
        set
        {
            _tileMargin = value;
            Apply();
        }
    }

    public Rectangle GetRectangle(int index)
    {
        // PETETRE UN BUG LA !!!
        Point positon = new Point
        (
            index * (_tileSize.i + _tileSeparation.i) % (sheet.i + _tileSeparation.i),
            (index / ((sheet.i + _tileSeparation.i) / (_tileSize.i + _tileSeparation.i))) * (_tileSize.j + _tileSeparation.j)
        );
        return new Rectangle
        (
            positon.i,
            positon.j,
            _tileSize.i,
            _tileSize.j
        );
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}

public class TileMap : IDisposable
{
    readonly int xCount;
    readonly int yCount;

    public delegate void DoAt(Point pos);

    public TileMap(TileSet sheet,
        OgmoFile file,
        Graphics.Color background = null,
        Texture2D backgroundTexture = null,
        bool mergeHitBoxes = true)
    {
        Hitbox.Rectangle[,] _hitboxData;
        Graphics.Color bg;
        if (background != null)
        {
            bg = background;
        }
        else { bg = new(0, 0, 0); }
        _sheet = sheet;
        _file = file;

        SpriteBatch batch = GameManager.Instance.SpriteBatch;

        xCount = file.width / file.layers[0].gridCellWidth;
        yCount = file.height / file.layers[0].gridCellHeight;

        _renderTarget = new RenderTarget2D
        (
            GameManager.Instance.GraphicsDevice,
            xCount * sheet.TileSize.i,
            yCount * sheet.TileSize.j
        );

        GameManager.Instance.GraphicsDevice.SetRenderTarget(_renderTarget);
        GameManager.Instance.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
        batch.Begin(samplerState: SamplerState.PointClamp);
        if (backgroundTexture != null)
        {
            batch.Draw
            (
                backgroundTexture,
                new Rectangle(0, 0,
                xCount * sheet.TileSize.i,
                yCount * sheet.TileSize.j),
                null,
                new Color(bg.RGB.R, bg.RGB.G, bg.RGB.B)
            );
        }

        _hitboxData = new Hitbox.Rectangle[xCount, yCount];

        foreach (OgmoLayer layer in new OgmoLayer[3] { _file.layers[2], _file.layers[1], _file.layers[0] })
        {
            for (int i = 0; i < layer.data.Length; i++)
            {
                if (layer.data[i] >= 0)
                {
                    int x = i % xCount;
                    int y = i / xCount;

                    int sx = layer.data[i] % _sheet.Xlenght;
                    int sy = layer.data[i] / _sheet.Xlenght;

                    if (sheet.ReplaceHitbox[sx, sy] is null
                        && sheet.ReplaceEntity[sx, sy] is null
                        && sheet._dontDraw.Contains(layer.data[i]) == false)
                    {
                        batch.Draw
                        (
                            sheet.Texture,
                            new Rectangle
                            (
                                x * sheet.TileSize.i,
                                y * sheet.TileSize.j,
                                sheet.TileSize.i,
                                sheet.TileSize.j
                            ),
                            sheet.GetRectangle(layer.data[i]),
                            Microsoft.Xna.Framework.Color.White
                        );
                    }
                    else
                    {
                        if (!(sheet.ReplaceHitbox[sx, sy] is null))
                        {
                            _hitboxData[x, y] = sheet.ReplaceHitbox[sx, sy];
                        }

                        if (!(sheet.ReplaceEntity[sx, sy] is null))
                        {
                            Vector pos = new Vector();
                            pos = new Vector
                            (
                            -(file.width / 2f) + layer.gridCellWidth / 2f,
                                file.height / 2f - layer.gridCellHeight / 2f
                            );
                            Entity.Object obj = sheet.ReplaceEntity[x, y].Copy();
                            obj.Space.Position += Position;
                            obj.Space.GridOrigin = Bounds.Center;
                            obj.Space.Position += pos;
                            obj.Space.Position.x += x % (file.width / layer.gridCellWidth) * layer.gridCellWidth;
                            obj.Space.Position.y -= System.Math.DivRem(x * layer.gridCellWidth, file.width).Quotient * sheet.TileSize.j;
                            obj.Renderer.hide = false;
                            sheet.entities.Add(obj);
                        }
                    }
                }
            }
        }
        batch.End();

        if (mergeHitBoxes)
        {
            MergeHitBoxes(ref _hitboxData);
        }
        else
        {
            for (int x = 0; x < xCount; x++)
            {
                for (int y = 0; y < yCount; y++)
                {
                    if (_hitboxData[x, y] is not null)
                    {
                        Hitbox.Rectangle hit = _hitboxData[x, y].Copy();
                        hit.Active = true;
                        hit.PositionOffset += new Vector
                        (
                            -(_file.width / 2f) + _sheet.TileSize.i / 2f,
                            _file.height / 2f - _sheet.TileSize.j / 2f
                        );
                        hit.PositionOffset.x += x * _sheet.TileSize.i;
                        hit.PositionOffset.y -= y * _sheet.TileSize.j;
                        hit.LockSize = new Vector(
                            _hitboxData[x, y].LockSize.x,
                            _hitboxData[x, y].LockSize.y);
                    }
                }
            }
        }
        Color = Graphics.Color.White;
    }

    public Vector[] GetPos(ushort i, ushort j)
    {
        List<Vector> result = new List<Vector>();
        int target = i + j * _sheet.Xlenght;
        foreach (OgmoLayer layer in _file.layers)
        {
            for (int k = 0; k < layer.data.Length; k++)
            {
                if (layer.data[k] == target)
                {
                    result.Add(new Vector
                        (
                           k % xCount * _sheet.TileSize.i - (_file.width - _sheet.TileSize.i) / 2f,
                           -(k / xCount) * _sheet.TileSize.j + (_file.height + _sheet.TileSize.j) / 2f - _sheet.TileSize.j
                        ));
                }
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// algo banger que j'ai fais pour éviter la redondance de hitboxes
    /// </summary>
    private List<Hitbox.Rectangle> MergeHitBoxes(ref Hitbox.Rectangle[,] lst)
    {
        List<Hitbox.Rectangle> result = new List<Hitbox.Rectangle>();
        int i = -1;
        while (i + 1 < xCount * yCount)
        {
            i++;
            long x = i % xCount;
            long y = i / xCount;

            Hitbox.Rectangle hit1 = lst[x, y];

            if (hit1 is not null)
            {
                int width = 1;
                int height = 1;

                while (x + width < xCount
                    && lst[x + width, y] is not null
                    && lst[x + width, y]._tag == hit1._tag
                    && lst[x + width, y].Layer == hit1.Layer
                    && lst[x + width, y].LockSize.y == hit1.LockSize.y)
                {
                    lst[x + width, y] = null;
                    width++;
                }

                bool Cond(ref Hitbox.Rectangle[,] h)
                {
                    if (y + height >= yCount)
                        return false;
                    Hitbox.Rectangle h2 = hit1;
                    for (int k = 0; k < width; k++)
                    {
                        Hitbox.Rectangle h1 = h[x + k, y + height];
                        if (h1 is null
                           || h1._tag != h2._tag
                           || h1.Layer != h2.Layer
                           || h1.LockSize.x != h2.LockSize.x)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                while (Cond(ref lst))
                {
                    for (int k = 0; k < width; k++)
                    {
                        lst[x + k, y + height] = null;
                    }
                    height++;
                }

                Hitbox.Rectangle hit = hit1.Copy();
                hit.Layer = hit1.Layer;
                hit.Active = true;
                hit.PositionOffset += new Vector
                (
                    -(_file.width / 2f) + _sheet.TileSize.i / 2f,
                    _file.height / 2f - _sheet.TileSize.j / 2f
                );
                hit.PositionOffset.x += (x + (width - 1) / 2f) * _sheet.TileSize.i;
                hit.PositionOffset.y -= (y + (height - 1) / 2f) * _sheet.TileSize.j;
                float a = 0f;
                float b = 0f;
                if (hit._tag == "red" || hit._tag == "green")
                {
                    if (width >= height)
                    {
                        a = -6f;
                    }
                    else
                    {
                        b = -6f;
                    }
                }
                hit.LockSize = new Vector(
                    hit1.LockSize.x * width + a,
                    hit1.LockSize.y * height + b);
                lst[x, y] = null;

                i = -1;
            }
        }

        return result;
    }

    private readonly TileSet _sheet;
    private readonly OgmoFile _file;

    private readonly RenderTarget2D _renderTarget;
    public Texture2D Texture
    {
        get
        {
            return _renderTarget;
        }
    }

    public Vector Position;

    public Graphics.Color Color { get; set; }

    public void Draw()
    {
        GameManager.Instance.SpriteBatch.Draw
        (
            _renderTarget,
            new Rectangle
            (
                (int)(Position.x - Camera.Position.x),
                (int)(Position.y + Camera.Position.y),
                _file.width,
                256
            ),
            null,
            new Color(Color.RGB.R, Color.RGB.G, Color.RGB.B),
            0,
            Vector2.Zero,
            SpriteEffects.None,
            0
        );

        foreach (Entity.Object obj in _sheet.entities)
        {
            obj.Draw();
        }
    }

    public void Dispose()
    {
        _renderTarget.Dispose();
    }
}

public struct OgmoFile
{
    public string ogmoVersion;
    public int width;
    public int height;
    public int offsetX;
    public int offsetY;
    public OgmoLayer[] layers;
}

public struct OgmoLayer
{
    public string name;
    public int _eid;
    public int offsetX;
    public int offsetY;
    public int gridCellWidth;
    public int gridCellHeight;
    public int gridCellsX;
    public int gridCellsY;
    public string tileset;
    public int[] data;
    public int arrayMode;
}