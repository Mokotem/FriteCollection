using System;
using System.Linq;
using System.Reflection;
using FriteCollection.Audio;
using FriteCollection.Entity;
using FriteCollection.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FriteModel
{
    public class MonoGame : Game
    {
        private GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch;
        private GameName.Settings onSettings;

        public static MonoGame instance;

        private bool changingScene = false;

        public MonoGame()
        {
            Content.RootDirectory = "Content";
            instance = this;
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
        }

        public float _timer = 0f, _delta = 0f, _targetTimer = 0f;

        public static Texture2D CreateTexture(GraphicsDevice device, int w, int h, FriteCollection.Graphics.Color color)
        {
            Texture2D texture = new Texture2D(device, w, h);

            Color[] data = new Color[w * h];
            for (int pixel = 0; pixel < w * h; pixel++)
            {
                data[pixel] = new Color(color.RGB.R, color.RGB.G, color.RGB.B);
            }

            texture.SetData(data);

            return texture;
        }

        private List<FriteCollection.Entity.Entity> entAlpha = new List<Entity>(), entBlending = new List<Entity>();
        public List<Executable> CurrentExecutables = new List<Executable>();
        public FriteCollection.Entity.Vector mouseClickedPosition = Vector.Zero;

        public FriteCollection.Tools.Shaders.Shader CurrentShader = null;

        protected override void Initialize()
        {
            Music.Volume = 0.5f;
            Window.AllowUserResizing = false;
            onSettings = new GameName.Settings();
            onSettings.SetGameSettings();
            Window.Title = GetSeetings.Settings.WindowName;

            graphics.PreferredBackBufferWidth = GetSeetings.Settings.WindowWidth;
            graphics.PreferredBackBufferHeight = GetSeetings.Settings.WindowHeight;
            FullScreen = GetSeetings.Settings.FullScreen;

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            GameManager.CurrentScene = GetSeetings.Settings.StartScene;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / GetSeetings.Settings.FPS);
            screenBounds = _bf.CreateBounds(GetSeetings.Settings.GameFixeWidth, GetSeetings.Settings.GameFixeHeight);

            base.Initialize();
        }

        public void UpdateScriptToScene()
        {
            changingScene = true;
            CurrentExecutables.Clear();
            HitBox.ClearHitboxes();

            Music.StopAllMusics();

            Camera.Position = new Vector(0, 0);
            Screen.backGround = new FriteCollection.Graphics.Color(0.1f, 0.2f, 0.3f);

            _timer = 0;
            _targetTimer = 0;

            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var childTypesScript = allTypes.Where(t => t.IsSubclassOf(typeof(Script)) && !t.IsAbstract);

            foreach (var type in childTypesScript)
            {
                var instance = (Script)Activator.CreateInstance(type);
                if (instance.AttributedScene == GameManager.CurrentScene)
                {
                    CurrentExecutables.Add(instance);
                }
                else instance = null;
            }

            foreach (Script script in CurrentExecutables)
            {
                script.BeforeStart();
            }

            foreach (Script script in CurrentExecutables)
            {
                script.Start();
            }

            foreach (Script script in CurrentExecutables)
            {
                script.AfterStart();
            }
        }

        private bool _fullScreen;
        public float aspectRatio;

        public bool FullScreen
        {
            get
            {
                return _fullScreen;
            }
            set
            {
                _fullScreen = value;

                renderTarget = new RenderTarget2D(GraphicsDevice, GetSeetings.Settings.GameFixeWidth, GetSeetings.Settings.GameFixeHeight);

                if (value == false)
                {
                    float ratioW = (float)GetSeetings.Settings.WindowWidth / (float)GetSeetings.Settings.GameFixeWidth;
                    float ratioH = (float)GetSeetings.Settings.WindowHeight / (float)GetSeetings.Settings.GameFixeHeight;

                    aspectRatio = MathF.Min(ratioW, ratioH);

                    targetGameRectangle = new Rectangle
                    (
                        (int)((GetSeetings.Settings.WindowWidth - (GetSeetings.Settings.GameFixeWidth * aspectRatio)) / 2f),
                        (int)((GetSeetings.Settings.WindowHeight - (GetSeetings.Settings.GameFixeHeight * aspectRatio)) / 2f),
                        (int)(GetSeetings.Settings.GameFixeWidth * aspectRatio),
                        (int)(GetSeetings.Settings.GameFixeHeight * aspectRatio)
                    );
                }
                else
                {
                    float ratioW = (float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / (float)GetSeetings.Settings.GameFixeWidth;
                    float ratioH = (float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / (float)GetSeetings.Settings.GameFixeHeight;

                    aspectRatio = MathF.Min(ratioW, ratioH);

                    targetGameRectangle = new Rectangle
                    (
                        (int)((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - GetSeetings.Settings.GameFixeWidth * aspectRatio) / 2f),
                        (int)((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - GetSeetings.Settings.GameFixeHeight * aspectRatio) / 2f),
                        (int)(GetSeetings.Settings.GameFixeWidth * aspectRatio),
                        (int)(GetSeetings.Settings.GameFixeHeight * aspectRatio)
                    );
                }

                renderTargetUI = new RenderTarget2D(GraphicsDevice, targetGameRectangle.Width, targetGameRectangle.Height);
                UIscreenBounds = _bf.CreateBounds(targetGameRectangle.Width, targetGameRectangle.Height);
                graphics.HardwareModeSwitch = !value;
                graphics.IsFullScreen = value;
                graphics.ApplyChanges();
            }
        }

        public Rectangle targetGameRectangle;

        private BoundFunc _bf = new BoundFunc();
        public Vector[] screenBounds, UIscreenBounds;
        public bool previousMouseLeft;

        protected override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f;
            _targetTimer += (1f / GetSeetings.Settings.FPS);
            _delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f;

            if (Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && !previousMouseLeft)
                mouseClickedPosition = FriteCollection.Input.Input.Mouse.Position(bound: Bounds.TopLeft);

            if (!changingScene)
            {
                if (!changingScene)
                {
                    foreach (Executable script in CurrentExecutables)
                    {
                        script.BeforeUpdate();
                        if (changingScene) break;
                    }
                }
                if (!changingScene)
                {
                    foreach (Executable script in CurrentExecutables)
                    {
                        script.Update();
                        if (changingScene) break;
                    }
                }
                if (!changingScene)
                {
                    foreach (Executable script in CurrentExecutables)
                    {
                        script.AfterUpdate();
                        if (changingScene) break;
                    }
                }
            }

            previousMouseLeft = Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;

            changingScene = false;
            base.Update(gameTime);
        }

        RenderTarget2D renderTarget, renderTargetUI;

        protected override void Draw(GameTime gameTime)
        {
            if (!changingScene)
            {
                GraphicsDevice.SetRenderTarget(renderTarget);
                GraphicsDevice.Clear(
                    new Color(Screen.backGround.RGB.R, Screen.backGround.RGB.G, Screen.backGround.RGB.B));

                SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                foreach (Executable script in CurrentExecutables) script.Draw();
                SpriteBatch.End();

                SpriteBatch.Begin(blendState: BlendState.Additive, samplerState: SamplerState.PointClamp);
                foreach (Executable script in CurrentExecutables) script.DrawAdditive();
                SpriteBatch.End();

                GraphicsDevice.SetRenderTarget(renderTargetUI);
                GraphicsDevice.Clear(Color.Black);
                if (CurrentShader is null)
                    SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
                else
                {
                    CurrentShader.ApllySettings();
                    SpriteBatch.Begin(effect: CurrentShader.GetEffect, samplerState: SamplerState.PointClamp);
                }
                SpriteBatch.Draw(
                    renderTarget,
                    new Rectangle(0, 0, targetGameRectangle.Width, targetGameRectangle.Height),
                    Color.White);

                SpriteBatch.End();
                SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
                foreach (Executable script in CurrentExecutables) script.DrawUI(); 
                SpriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);
                SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
                SpriteBatch.Draw(renderTargetUI, targetGameRectangle, Color.White);
                SpriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
