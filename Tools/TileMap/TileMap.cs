using FriteCollection.Entity.Hitboxs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using FriteCollection.Entity;

namespace FriteCollection.Tools.TileMap;

public class TileSet : IDisposable
{
    public int Xlenght { get; private set; }
    public int Ylenght { get; private set; }

    public TileSet(Texture2D texture, Point tileSize, Point tileSeparation, Point tileMargin)
    {
        sheet = new Point(texture.Width, texture.Height);
        _tileSize = tileSize;
        _tileSeparation = tileSeparation;
        _tileMargin = tileMargin;
        Apply();
        _texture = texture;
    }
    private Texture2D _texture;
    public Texture2D Texture
    {
        get { return _texture; }
    }

    private void Apply()
    {
        Xlenght =
        (sheet.X + _tileSeparation.X) / (_tileSize.X + _tileSeparation.X);

        Ylenght =
            (sheet.Y + _tileSeparation.Y) / (_tileSize.Y + _tileSeparation.Y);
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
            index * (_tileSize.X + _tileSeparation.X) % (sheet.X + _tileSeparation.X),
            (index / ((sheet.X + _tileSeparation.X) / (_tileSize.X + _tileSeparation.X))) * (_tileSize.Y + _tileSeparation.Y)
        );
        return new Rectangle
        (
            positon.X,
            positon.Y,
            _tileSize.X,
            _tileSize.Y
        );
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}

public class TileMap : IDisposable, ILayer
{
    private int xCount => _file.layers[0].gridCellsX;
    private int yCount => _file.layers[0].gridCellsY;

    public delegate void DoAt(Point pos);
    public delegate void Entity(Point pos);

    private float layer = 0.5f;
    public short Layer
    {
        get
        {
            return (short)((layer * 2000) - 1000);
        }

        set
        {
            if (value > 1000) throw new ArgumentOutOfRangeException("value cannot be greater than 1000");
            if (value < -1000) throw new ArgumentOutOfRangeException("value cannot be less than -1000");
            layer = (value + 1000f) / 2000f;
        }
    }

    public TileMap(TileSet sheet,
        OgmoFile file,
        in Hitbox.Rectangle[] hitboxesreplaces,
        Color? background = null,
        Texture2D backgroundTexture = null,
        bool mergeHitBoxes = true)
    {
        Hitbox.Rectangle[,] _hitboxData;
        Color bg;
        if (background is not null && background.HasValue)
        {
            bg = background.Value;
        }
        else { bg = new(0, 0, 0); }
        _sheet = sheet;
        _file = file;

        SpriteBatch batch = GameManager.Instance.SpriteBatch;

        _renderTarget = new RenderTarget2D
        (
            GameManager.Instance.GraphicsDevice,
            xCount * sheet.TileSize.X,
            yCount * sheet.TileSize.Y
        );

        GameManager.Instance.GraphicsDevice.SetRenderTarget(_renderTarget);
        GameManager.Instance.GraphicsDevice.Clear(Color.Transparent);
        batch.Begin(samplerState: SamplerState.PointClamp);
        if (backgroundTexture is not null)
        {
            batch.Draw
            (
                backgroundTexture,
                new Rectangle(0, 0,
                xCount * sheet.TileSize.X,
                yCount * sheet.TileSize.Y),
                null,
                bg
            );
        }

        _hitboxData = new Hitbox.Rectangle[xCount, yCount];

        foreach (OgmoLayer layer in _file.layers)
        {
            if (layer.name.Equals("_Hitboxs"))
            {
                for (int i = 0; i < layer.grid.Length; i++)
                {
                    if (!layer.grid[i].Equals('0'))
                    {
                        int x = i % xCount;
                        int y = i / xCount;
                        _hitboxData[x, y] = hitboxesreplaces[0];
                    }
                }
            }
            else if (layer.name[0] != '_')
            {
                for (int i = 0; i < layer.data.Length; i++)
                {
                    if (layer.data[i] >= 0)
                    {
                        int x = i % xCount;
                        int y = i / xCount;

                        int sx = layer.data[i] % _sheet.Xlenght;
                        int sy = layer.data[i] / _sheet.Xlenght;

                        batch.Draw
                        (
                            sheet.Texture,
                            new Rectangle
                            (
                                x * sheet.TileSize.X,
                                y * sheet.TileSize.Y,
                                sheet.TileSize.X,
                                sheet.TileSize.Y
                            ),
                            sheet.GetRectangle(layer.data[i]),
                            Color.White
                        );
                    }
                }
            }
        }
        batch.End();

        MakeHitboxes(mergeHitBoxes, ref _hitboxData);

        Color = Color.White;
    }

    private void MakeHitboxes(bool merge, ref Hitbox.Rectangle[,] _hitboxData)
    {
        if (merge)
            MergeHitBoxes(ref _hitboxData);
        else
            PlaceHitboxes(ref _hitboxData, _sheet.TileSize);
    }

    private void MakeHitboxes(bool merge, ref Hitbox.Rectangle[,] _hitboxData, Point size)
    {
        if (merge)
            MergeHitBoxes(ref _hitboxData);
        else
            PlaceHitboxes(ref _hitboxData, size);
    }

    private void PlaceHitboxes(ref Hitbox.Rectangle[,] _hitboxData, Point tileSize)
    {
        for (int x = 0; x < xCount; ++x)
        {
            for (int y = 0; y < yCount; ++y)
            {
                if (_hitboxData[x, y] is not null)
                {
                    Hitbox.Rectangle hit = _hitboxData[x, y].Copy();
                    hit.Active = true;
                    hit.PositionOffset.X += x * tileSize.X;
                    hit.PositionOffset.Y -= y * tileSize.Y;
                    hit.LockSize(new Vector2(
                        _hitboxData[x, y].Size.X,
                        _hitboxData[x, y].Size.Y));
                }
            }
        }
    }

    public Vector2[] GetPos(ushort i, ushort j)
    {
        List<Vector2> result = new List<Vector2>();
        int target = i + j * _sheet.Xlenght;
        foreach (OgmoLayer layer in _file.layers)
        {
            for (int k = 0; k < layer.data.Length; k++)
            {
                if (layer.data[k] == target)
                {
                    result.Add(new Vector2
                        (
                           k % xCount * _sheet.TileSize.X- (_file.width - _sheet.TileSize.X) / 2f,
                           -(k / xCount) * _sheet.TileSize.Y + (_file.height + _sheet.TileSize.Y) / 2f - _sheet.TileSize.Y
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
            int x = i % xCount;
            int y = i / xCount;

            Hitbox.Rectangle hit1 = lst[x, y];

            if (hit1 is not null)
            {
                int width = 1;
                int height = 1;

                while (x + width < xCount
                    && lst[x + width, y] is not null
                    && lst[x + width, y]._tag == hit1._tag
                    && lst[x + width, y].Layer == hit1.Layer
                    && lst[x + width, y].Size.Y == hit1.Size.Y)
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
                           || h1.Size.X != h2.Size.X)
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
                hit.PositionOffset.X += x * _file.layers[0].gridCellWidth + this.Position.X;
                hit.PositionOffset.Y -= y * _file.layers[0].gridCellHeight + this.Position.X;
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
                hit.LockSize(new Vector2(
                    hit1.Size.X * width + a,
                    hit1.Size.Y * height + b));
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

    public Vector2 Position;

    public Color Color { get; set; }

    public void Draw()
    {
        GameManager.Instance.SpriteBatch.Draw
        (
            _renderTarget,
            new Rectangle
            (
                (int)(Position.X - Camera.Position.X),
                (int)(Position.Y + Camera.Position.Y),
                _renderTarget.Width,
                _renderTarget.Height
            ),
            null,
            Color,
            0,
            Vector2.Zero,
            SpriteEffects.None,
            this.layer
        );
    }

    public void Dispose()
    {
        _renderTarget.Dispose();
    }
}

public struct LevelValues
{
    public short ID { get; init; }

    public short opening_A { get; init; }
    public short opening_B { get; init; }
    public short opening_C { get; init; }
    public short opening_D { get; init; }

    public short X { get; init; }
    public short Y { get; init; }
}

public struct OgmoFile
{
    public static OgmoFile Open(string path)
    {
        string file;
        using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + "/" + path))
            file = sr.ReadToEnd();
        return JsonSerializer.Deserialize<OgmoFile>(file);
    }

    public string ogmoVersion { get; init; }
    public int width { get; set; }
    public int height { get; set; }
    public int offsetX { get; init; }
    public int offsetY { get; init; }
    public OgmoLayer[] layers { get; init; }
    public LevelValues values { get; init; }
}

public struct OgmoLayer
{
    public string name { get; init; }
    public string _eid { get; init; }
    public int offsetX { get; init; }
    public int offsetY { get; init; }
    public int gridCellWidth { get; set; }
    public int gridCellHeight { get; set; }
    public int gridCellsX { get; init; }
    public int gridCellsY { get; init; }
    public string tileset { get; init; }
    public int[] data { get; init; }
    public char[] grid { get; init; }
    public int exportMode { get; init; }
    public int arrayMode { get; init; }
}