using FriteCollection.Entity;
using FriteCollection.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FriteCollection.Tools.TileMap
{
    public struct Vector
    {
        public Vector()
        {
            width = 0; height = 0;
        }
        public Vector(int width, int height)
        {
            this.width = width; this.height = height;
        }

        public int width, height;
    }

    public class TileSet : IDisposable
    {
        private HitBox.Rectangle[] _hitReplaces;
        private Entity.Object[] _entReplaces;
        public List<HitBox.Rectangle> hitboxes;
        public readonly List<Entity.Object> entities;

        public HitBox.Rectangle[] ReplaceHitbox
        {
            get
            {
                return _hitReplaces;
            }
        }

        public Entity.Object[] ReplaceEntity
        {
            get
            {
                return _entReplaces;
            }
        }

        public TileSet(Texture2D texture, Vector tileSize, Vector tileSeparation, Vector tileMargin)
        {
            hitboxes = new List<HitBox.Rectangle>();
            entities = new List<Entity.Object>();
            this._texture = texture;
            sheet.width = texture.Width;
            sheet.height = texture.Height;
            _tileSize = tileSize;
            _tileSeparation = tileSeparation;
            _tileMargin = tileMargin;
            Apply();
        }
        private Texture2D _texture;
        public Texture2D Texture
        {
            get { return _texture; }
        }

        private void Apply()
        {
            int _length = ((sheet.width + _tileSeparation.width) / (_tileSize.width + _tileSeparation.width))
                *
                ((sheet.height + _tileSeparation.height) / (_tileSize.height + _tileSeparation.height));

            _hitReplaces = new HitBox.Rectangle[_length];
            _entReplaces = new Entity.Object[_length];
        }
        public readonly Vector sheet;
        private Vector _tileSize;
        public Vector TileSize
        {
            get { return _tileSize; }
            set
            {
                _tileSize = value;
                Apply();
            }
        }

        private Vector _tileSeparation = new(0, 0);
        public Vector TileSeparation
        {
            get { return _tileSeparation; }
            set
            {
                _tileSeparation = value;
                Apply();
            }
        }
        private Vector _tileMargin = new(0, 0);
        public Vector TileMargin
        {
            get { return _tileMargin; }
            set
            {
                _tileMargin = value;
                Apply();
            }
        }

        //Quotient
        public Rectangle GetRectangle(int index)
        {
            Vector positon = new Vector
            (
                (index * (_tileSize.width + _tileSeparation.width)) % (sheet.width + _tileSeparation.width),
                Math.Math.Quotient(index, ((sheet.width + _tileSeparation.width) / (_tileSize.width + _tileSeparation.width))) * (_tileSize.height + _tileSeparation.height)
            );
            return new Rectangle
            (
                positon.width,
                positon.height,
                _tileSize.width,
                _tileSize.height
            );
        }

        public void Dispose()
        {
            _texture.Dispose();
        }
    }

    public class TileMap
    {
        public TileMap(TileSet sheet,
            OgmoFile file,
            float? sizeFactor = null,
            Graphics.Color background = null,
            Texture2D backgroundTexture = null,
            bool mergeHitBoxes = true)
        {
            Graphics.Color bg;
            if (background != null)
            {
                bg = background;
            }
            else { bg = new(0, 0, 0); }
            if (sizeFactor != null)
            {
                _scalefactor = sizeFactor.Value;
            }
            else { _scalefactor = 1; }
            _sheet = sheet;
            _file = file;

            SpriteBatch batch = FriteModel.MonoGame.instance.SpriteBatch;

            _renderTarget = new RenderTarget2D
            (
                FriteModel.MonoGame.instance.GraphicsDevice,
                (file.width / file.layers[0].gridCellWidth) * sheet.TileSize.width,
                (file.height / file.layers[0].gridCellHeight) * sheet.TileSize.height
            );

            FriteModel.MonoGame.instance.GraphicsDevice.SetRenderTarget(_renderTarget);
            FriteModel.MonoGame.instance.GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(bg.RGB.R, bg.RGB.G, bg.RGB.B));
            batch.Begin(samplerState: SamplerState.PointClamp);
            if (backgroundTexture != null)
            {
                batch.Draw
                (
                    backgroundTexture,
                    new Rectangle(0, 0, (file.width / file.layers[0].gridCellWidth) * sheet.TileSize.width, (file.height / file.layers[0].gridCellHeight) * sheet.TileSize.height),
                    null,
                    new Microsoft.Xna.Framework.Color(bg.RGB.R, bg.RGB.G, bg.RGB.B)
                );
            }
            foreach (OgmoLayer layer in file.layers)
            {
                int i = 0;
                foreach (int sheetIndex in layer.data)
                {
                    if (sheetIndex >= 0)
                    {
                        if (sheet.ReplaceHitbox[sheetIndex] is null && sheet.ReplaceEntity[sheetIndex] is null)
                        {
                            batch.Draw
                            (
                                sheet.Texture,
                                new Rectangle
                                (
                                    (i % (file.width / layer.gridCellWidth)) * sheet.TileSize.width,
                                    Math.Math.Quotient(i, file.width / layer.gridCellWidth) * sheet.TileSize.width,
                                    sheet.TileSize.width,
                                    sheet.TileSize.height
                                ),
                                sheet.GetRectangle(sheetIndex),
                                Color.White
                            );
                        }
                        else
                        {
                            if (!(sheet.ReplaceHitbox[sheetIndex] is null))
                            {
                                HitBox.Rectangle hit = sheet.ReplaceHitbox[sheetIndex].Copy();
                                hit.Active = true;
                                hit.Space = this.Space;
                                hit.PositionOffset += new Entity.Vector
                                (
                                    -(file.width / 2f) + (layer.gridCellWidth / 2f),
                                    (file.height / 2f) - (layer.gridCellWidth / 2f)
                                ) * _scalefactor;
                                hit.PositionOffset.x += i % (file.width / layer.gridCellWidth) * layer.gridCellWidth * _scalefactor;
                                hit.PositionOffset.y -= System.Math.DivRem(i * layer.gridCellWidth, file.width).Quotient * layer.gridCellHeight * _scalefactor;
                                sheet.hitboxes.Add(hit);
                            }

                            if (!(sheet.ReplaceEntity[sheetIndex] is null))
                            {
                                Entity.Vector pos = new Entity.Vector();
                                pos = new Entity.Vector
                                (
                                -(file.width / 2f) + (layer.gridCellWidth / 2f),
                                    (file.height / 2f) - (layer.gridCellHeight / 2f)
                                ) * _scalefactor;
                                Entity.Object obj = sheet.ReplaceEntity[sheetIndex].Copy();
                                obj.Space.Position += this.Space.Position;
                                obj.Space.GridOrigin = Bounds.Center;
                                obj.Space.Position += pos;
                                obj.Space.Position.x += i % (file.width / layer.gridCellWidth) * layer.gridCellWidth * _scalefactor;
                                obj.Space.Position.y -= System.Math.DivRem(i * layer.gridCellWidth, file.width).Quotient * sheet.TileSize.height * _scalefactor;
                                obj.Renderer.hide = false;
                                sheet.entities.Add(obj);
                            }
                        }
                    }
                    i++;
                }
            }
            batch.End();

            if (mergeHitBoxes) MergeHitBoxes(sheet.hitboxes);

            _bounds = _boundFunc.CreateBounds(file.width, file.height);
            Space.Scale = new(file.width, file.height);
        }

        private void MergeHitBoxes(List<HitBox.Rectangle> hitList)
        {
            // Merge on X
            uint i = 0;
            uint _length = hitList.Count;
            while (i < _length)
            {
                uint j = 0;
                HitBox.Rectangle hit1 = hitList[i];

                while (j < _length)
                {
                    if (i != j)
                    {
                        HitBox.Rectangle hit2 = hitList[j];

                        if (hit1.PositionOffset.y == hit2.PositionOffset.y && hit1.LockSize.y == hit2.LockSize.y
                            && System.MathF.Abs(hit1.PositionOffset.x - hit2.PositionOffset.x) <= ((hit1.LockSize.x + hit2.LockSize.x) / 2f)
                            && hit1.tag == hit2.tag)
                        {
                            // && hit1.PositionOffset.x < hit2.PositionOffset.x
                            _length--;

                            hitList.Remove(hit2);

                            hit2.Destroy();

                            HitBox.Rectangle newHit = hit1.Copy();
                            newHit.LockSize = new Entity.Vector(hit1.LockSize.x + hit2.LockSize.x, hit1.LockSize.y);
                            if (hit1.PositionOffset.x <= hit2.PositionOffset.x)
                                newHit.PositionOffset.x += hit2.LockSize.x / 2f;
                            else
                                newHit.PositionOffset.x += -hit2.LockSize.x / 2f;

                            hitList.Add(newHit, i);
                            hitList.RemoveIndex(j);
                            hit1.Destroy();

                            i = 0;

                            break;
                        }
                    }

                    j++;
                }

                i++;
            }

            // Merge on Y
            i = 0;
            _length = hitList.Count;
            while (i < _length)
            {
                uint j = 0;
                HitBox.Rectangle hit1 = hitList[i];

                while (j < _length)
                {
                    if (i != j)
                    {
                        HitBox.Rectangle hit2 = hitList[j];

                        if (hit1.PositionOffset.x == hit2.PositionOffset.x && hit1.LockSize.x == hit2.LockSize.x
                            && System.MathF.Abs(hit1.PositionOffset.y - hit2.PositionOffset.y)
                            <= ((hit1.LockSize.y + hit2.LockSize.y) / 2f)
                            && hit1.tag == hit2.tag)
                        {
                            _length--;

                            hitList.Remove(hit2);

                            hit2.Destroy();

                            HitBox.Rectangle newHit = hit1.Copy();
                            newHit.LockSize = new Entity.Vector(hit1.LockSize.x, hit1.LockSize.y + hit2.LockSize.y);
                            
                            if (hit1.PositionOffset.y > hit2.PositionOffset.y)
                                newHit.PositionOffset.y -= hit2.LockSize.y / 2f;
                            else
                                newHit.PositionOffset.y -= -hit2.LockSize.y / 2f;

                            hitList.Add(newHit, hitList.Index(hit1));
                            hitList.Remove(hit1);
                            hit1.Destroy();

                            i = 0;

                            break;
                        }
                    }

                    j++;
                }

                i++;
            }
        }

        private readonly TileSet _sheet;
        private readonly OgmoFile _file;

        private RenderTarget2D _renderTarget;
        public Texture2D Texture
        {
            get
            {
                return _renderTarget;
            }
        }

        private float _scalefactor;

        private Entity.BoundFunc _boundFunc = new();
        private Entity.Vector[] _bounds;

        public Entity.Space Space = new();

        public void Draw()
        {
            Space.CenterPoint = Bounds.Center;
            Entity.Vector entPosi = Space.GetScreenPosition();

            FriteModel.MonoGame.instance.SpriteBatch.Draw
            (
                _renderTarget,
                new Microsoft.Xna.Framework.Rectangle
                (
                    (int)entPosi.x,
                    (int)entPosi.y,
                    (int)(_file.width * _scalefactor * Camera.zoom),
                    (int)(_file.height * _scalefactor * Camera.zoom)
                ),
                null,
                Color.White,
                0,
                _bounds[(int)Space.CenterPoint].ToVector2(),
                SpriteEffects.None,
                0
            );

            foreach (Entity.Object obj in _sheet.entities)
            {
                obj.Draw();
            }
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
}
