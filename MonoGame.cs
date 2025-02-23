using System;
using System.Linq;
using FriteCollection;
using FriteCollection.Entity;
using FriteCollection.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FriteModel;

public class MonoGame : Game
{
    private GraphicsDeviceManager graphics;
    internal SpriteBatch SpriteBatch;

    internal List<FriteCollection.UI.ButtonCore> buttons = new List<FriteCollection.UI.ButtonCore>();

    private bool changingScene = false;
    private Settings S => GameManager.Settings;
    private readonly Type[] _childTypes;
    private SamplerState drawstate;

    public MonoGame(Type[] childTypes)
    {
        _childTypes = childTypes;
        IsMouseVisible = true;
        graphics = new GraphicsDeviceManager(this);
        Window.AllowAltF4 = false;
        Window.AllowUserResizing = false;
        GameManager.SetGameInstance(this);
    }

    internal double _timer = 0d, _delta = 0d, _targetTimer = 0d;

    private static Texture2D CreateTexture(GraphicsDevice device, int w, int h, Color color)
    {
        Texture2D texture = new Texture2D(device, w, h);

        Color[] data = new Color[w * h];
        for (int pixel = 0; pixel < w * h; pixel++)
        {
            data[pixel] = color;
        }

        texture.SetData(data);

        return texture;
    }

    internal List<Executable> CurrentExecutables = new List<Executable>();
    internal FriteCollection.Point
        mouseClickedPosition = FriteCollection.Point.Zero,
        mousePosition = FriteCollection.Point.Zero;

    protected override void Initialize()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Window.AllowUserResizing = false;
        Window.Title = S.WindowName;

        graphics.PreferredBackBufferWidth = S.WindowWidth;
        graphics.PreferredBackBufferHeight = S.WindowHeight;
        FullScreen = S.FullScreen;

        screenBounds = BoundFunc.CreateBounds(S.GameFixeWidth, S.GameFixeHeight);

        Renderer._defaultTexture = CreateTexture(GraphicsDevice, 2, 2, Color.White);

        base.Initialize();

        GameManager.Fps = GameManager.Settings.FPS;
        drawstate = S.PixelArtDrawing ? SamplerState.PointClamp : SamplerState.LinearClamp;

        UpdateScriptToScene();
    }

    internal void UpdateScriptToScene()
    {
        GameManager.SetGameInstance(this);

        changingScene = true;
        FriteCollection.Entity.Hitboxs.Hitbox.ClearHitboxes(); 

        buttons.Clear();

        foreach (Executable exe in CurrentExecutables)
        {
            if (exe is Clone)
                (exe as Clone).Destroy();
            exe.Dispose();
        }

        CurrentExecutables.Clear();

        Camera.Position = new Vector(0, 0);
        Screen.backGround = new FriteCollection.Graphics.Color(0.1f, 0.2f, 0.3f);

        _timer = 0;
        _targetTimer = 0;

        foreach (Type type in _childTypes)
        {
            if (type.IsSubclassOf(typeof(Script)))
            {
                Script instance = (Script)Activator.CreateInstance(type);
                if (instance.AttributedScenes == GameManager.CurrentScene)
                {
                    CurrentExecutables.Add(instance);
                }
                else instance = null;
            }
        }

        Time.SpaceTime = 1f;

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.Load();
        }

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.Start();
        }

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.BeforeUpdate();
        }

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.Update();
        }

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.AfterUpdate();
        }
        GC.Collect();
    }

    private bool _fullScreen;
    internal float aspectRatio;
    DisplayMode display => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

    internal bool FullScreen
    {
        get
        {
            return _fullScreen;
        }
        set
        {
            _fullScreen = value;

            renderTarget = new RenderTarget2D(GraphicsDevice, S.GameFixeWidth, S.GameFixeHeight);

            if (value == false)
            {
                float ratioW = S.WindowWidth / (float)S.GameFixeWidth;
                float ratioH = S.WindowHeight / (float)S.GameFixeHeight;

                aspectRatio = MathF.Min(ratioW, ratioH);

                targetGameRectangle = new Rectangle
                (
                    (int)((S.WindowWidth - (S.GameFixeWidth * aspectRatio)) / 2f),
                    (int)((S.WindowHeight - (S.GameFixeHeight * aspectRatio)) / 2f),
                    (int)(S.GameFixeWidth * aspectRatio),
                    (int)(S.GameFixeHeight * aspectRatio)
                );
            }
            else
            {
                float ratioW = display.Width / (float)S.GameFixeWidth;
                float ratioH = display.Height / (float)S.GameFixeHeight;

                aspectRatio = MathF.Min(ratioW, ratioH);

                targetGameRectangle = new Rectangle
                (
                    (int)((display.Width - S.GameFixeWidth * aspectRatio) / 2f),
                    (int)((display.Height - S.GameFixeHeight * aspectRatio) / 2f),
                    (int)(S.GameFixeWidth * aspectRatio),
                    (int)(S.GameFixeHeight * aspectRatio)
                );
            }

            renderTargetUI = new RenderTarget2D(GraphicsDevice, renderTarget.Width, renderTarget.Height);
            UIscreenBounds = BoundFunc.CreateBounds(renderTarget.Width, renderTarget.Height);
            graphics.HardwareModeSwitch = !value;
            graphics.IsFullScreen = value;
            graphics.ApplyChanges();
        }
    }

    public Rectangle targetGameRectangle;

    internal Vector[] screenBounds, UIscreenBounds;
    internal bool previousMouseLeft;

    protected override void Update(GameTime gameTime)
    {
        _timer += gameTime.ElapsedGameTime.TotalMilliseconds / 1000d;
        _targetTimer += 1d / GameManager.Fps;
        _delta = gameTime.ElapsedGameTime.TotalMilliseconds / 1000d;

        MouseState mstate = Mouse.GetState();
        FriteCollection.Point v = new(
            (mstate.Position.X) * S.GameFixeWidth / (_fullScreen ? display.Width : S.WindowWidth),
            (mstate.Position.Y) * S.GameFixeHeight / (_fullScreen ? display.Height : S.WindowHeight));
        if (Mouse.GetState().LeftButton == ButtonState.Pressed && !previousMouseLeft)
        {
            mouseClickedPosition = v;
        }
        mousePosition = v;
        if (!this.IsActive)
        {
            mouseClickedPosition = new(-10, -10);
            mousePosition = new(-10, -10);
        }

        Input.SetStates(Keyboard.GetState(), mstate);

        if (!changingScene)
        {
            if (!changingScene)
            {
                foreach (Executable script in CurrentExecutables.Copy())
                {
                    script.BeforeUpdate();
                }
            }
            if (!changingScene)
            {
                foreach (Executable script in CurrentExecutables.Copy())
                {
                    script.Update();
                }
            }
            if (!changingScene)
            {
                foreach (FriteCollection.UI.ButtonCore but in buttons)
                {
                    but.Update();
                }
            }
            if (!changingScene)
            {
                foreach (Executable script in CurrentExecutables.Copy())
                {
                    script.AfterUpdate();
                }
            }
        }

        previousMouseLeft = Mouse.GetState().LeftButton == ButtonState.Pressed;
        base.Update(gameTime);
    }

    RenderTarget2D renderTarget, renderTargetUI;


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(
            new Color(Screen.backGround.RGB.R, Screen.backGround.RGB.G, Screen.backGround.RGB.B));
        SpriteBatch.Begin(
            blendState: BlendState.AlphaBlend,
            samplerState: drawstate,
            sortMode: SpriteSortMode.BackToFront
            );
        foreach (Executable script in CurrentExecutables)
        {
            script.Draw();
        }
        SpriteBatch.End();

        foreach (Executable script in CurrentExecutables)
        {
            script.Draw(ref SpriteBatch);
        }

        SpriteBatch.Begin(blendState: BlendState.Additive, samplerState: drawstate);
        foreach (Executable script in CurrentExecutables)
        {
            if (!changingScene)
            {
                script.DrawAdditive();
            }
        }
        SpriteBatch.End();

        GraphicsDevice.SetRenderTarget(renderTargetUI);

        GraphicsDevice.Clear(Color.Black);
        SpriteBatch.Begin(samplerState: drawstate);
        SpriteBatch.Draw(
            renderTarget,
            new Rectangle(0, 0, renderTarget.Width, renderTarget.Height),
            Color.White);

        SpriteBatch.End();
        SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: drawstate);
        foreach (Executable script in CurrentExecutables)
        {
            script.DrawUI();
        }
        SpriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(samplerState: drawstate);
        SpriteBatch.Draw(renderTargetUI, targetGameRectangle, Color.White);
        SpriteBatch.End();

        changingScene = false;
        base.Draw(gameTime);
    }
}