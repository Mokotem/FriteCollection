using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection.Tools.SpriteSheet
{
    public class SpriteSheet : IDisposable
    {
        private readonly Texture2D[,] textures;

        public SpriteSheet(Texture2D texture, int width, int height)
        {
            int wCount = texture.Width / width;
            int hCount = texture.Height / height;

            textures = new Texture2D[wCount, hCount];
            
            for (int x = 0; x < wCount; x++)
            {
                for (int y = 0; y < hCount; y++)
                {
                    Texture2D tex = new Texture2D(FriteModel.MonoGame.instance.GraphicsDevice,
                        width, height);
                    Color[] data = new Color[width * height];

                    Rectangle rect = new Rectangle(x * width, y * height, width, height);
                    texture.GetData<Color>(0, rect, data, 0, width * height);

                    tex.SetData(data);
                    textures[x, y] = tex;
                }
            }
        }

        public Texture2D this[int x, int y]
        {
            get
            {
                return textures[x, y];
            }
        }

        public void Dispose()
        {
            foreach (Texture2D tex in textures)
                tex.Dispose();
        }
    }
}