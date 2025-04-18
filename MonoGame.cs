using System;
using FriteCollection;
using FriteCollection.Entity;
using FriteCollection.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace FriteModel;

public abstract class MonoGame : Game
{
    protected GraphicsDeviceManager graphics;
    internal protected SpriteBatch SpriteBatch;

    internal List<FriteCollection.UI.ButtonCore> _buttons = new List<FriteCollection.UI.ButtonCore>();

    protected bool changingScene = false;
    private protected readonly Type[] _childTypes;
    protected Settings S => GameManager.Settings;
    
    public virtual FriteCollection.Environment[] Environments
    {
        get;
    }

    public MonoGame(Type[] childTypes)
    {
        _childTypes = childTypes;
        IsMouseVisible = true;
        graphics = new GraphicsDeviceManager(this);
        Window.AllowAltF4 = false;
        Window.AllowUserResizing = S.AllowUserResizeing;
        GameManager.SetGameInstance(this);
    }

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

    protected List<Executable> _currentExecutables = new List<Executable>();
    internal List<Executable> CurrentExecutables => _currentExecutables;

    protected override void Initialize()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Window.Title = GameManager.Settings.WindowName;

        graphics.PreferredBackBufferWidth = GameManager.Settings.WindowWidth;
        graphics.PreferredBackBufferHeight = GameManager.Settings.WindowHeight;
        FullScreen = GameManager.Settings.FullScreen;

        Renderer._defaultTexture = CreateTexture(GraphicsDevice, 2, 2, Color.White);

        base.Initialize();

        GameManager.Fps = GameManager.Settings.FPS;

        UpdateScriptToScene();
    }

    public virtual void UpdateScriptToScene()
    {
        GameManager.SetGameInstance(this);

        changingScene = true;
        FriteCollection.Entity.Hitboxs.Hitbox.ClearHitboxes();
        MediaPlayer.Stop();
        _buttons.Clear();

        foreach (Executable exe in CurrentExecutables)
        {
            if (exe is Clone)
                (exe as Clone).Destroy();
            else
                exe.Dispose();
        }

        CurrentExecutables.Clear();

        Camera.Position = Vector2.Zero;
        Screen.backGround = new Color(0.1f, 0.2f, 0.3f);

        foreach (Type type in _childTypes)
        {
            Script instance = (Script)Activator.CreateInstance(type);
            if (instance.AttributedScenes == GameManager.CurrentScene && instance.Active)
            {
                CurrentExecutables.Add(instance);
            }
            else instance = null;
        }

        Time.SpaceTime = 1f;

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.Load();
        }

        foreach (Executable script in CurrentExecutables.ToArray())
        {
            script.BeforeStart();
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

    protected bool _fullScreen;
    protected DisplayMode Display => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

    public virtual bool FullScreen
    {
        get
        {
            return _fullScreen;
        }
        set
        {
            _fullScreen = value;

            graphics.HardwareModeSwitch = !value;
            graphics.IsFullScreen = value;
            graphics.ApplyChanges();
        }
    }

    public virtual void UpdateEnvironments()
    {

    }

    public static void SetUserCursor(Texture2D tex, int ox, int oy)
    {
        Mouse.SetCursor(MouseCursor.FromTexture2D(tex, ox, oy));
    }

    protected virtual void OnUpdate(GameTime gameTime)
    {

    }

    protected MouseState mstate;
    private Point mcp;
    internal Point MouseClickedPosition => mcp;
    protected override void Update(GameTime gameTime)
    {
        Time.UpdateGameTime(ref gameTime);

        if (this.IsActive)
        {
            mstate = Mouse.GetState();
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                mcp = new Point(mstate.Position.X, mstate.Position.Y);
            }
        }

        Input.SetStates(Keyboard.GetState(), mstate);
        foreach (FriteCollection.UI.ButtonCore but in _buttons)
        {
            but.Update();
        }
        this.OnUpdate(gameTime);
        base.Update(gameTime);
    }

    protected virtual void OnDraw(GameTime gameTime)
    {

    }

    protected override void Draw(GameTime gameTime)
    {
        this.OnDraw(gameTime);
        base.Draw(gameTime);
    }
}

public class MonoGameDefault : MonoGame
{
    public MonoGameDefault(Type[] types) : base(types)
    {
        drawstate = S.PixelArtDrawing ? SamplerState.PointClamp : SamplerState.LinearClamp;
    }

    private SamplerState drawstate;
    private static FriteCollection.Environment game, ui;
    internal float aspectRatio;

    public override FriteCollection.Environment[] Environments => new FriteCollection.Environment[2]
    {
        game, ui
    };

    public override void UpdateEnvironments()
    {
        if (this._fullScreen)
        {
            float ratioW = Display.Width / (float)S.GameFixeWidth;
            float ratioH = Display.Height / (float)S.GameFixeHeight;
            aspectRatio = MathF.Min(ratioW, ratioH);

            game = new FriteCollection.Environment(new Rectangle
            (
                (int)((Display.Width - S.GameFixeWidth * aspectRatio) / 2f),
                (int)((Display.Height - S.GameFixeHeight * aspectRatio) / 2f),
                (int)(S.GameFixeWidth * aspectRatio),
                (int)(S.GameFixeHeight * aspectRatio)
            ), new RenderTarget2D(GraphicsDevice, S.GameFixeWidth, S.GameFixeHeight));
        }
        else
        {
            float ratioW = S.WindowWidth / (float)S.GameFixeWidth;
            float ratioH = S.WindowHeight / (float)S.GameFixeHeight;
            aspectRatio = MathF.Min(ratioW, ratioH);

            game = new FriteCollection.Environment(new Rectangle
            (
                (int)((graphics.PreferredBackBufferWidth - S.GameFixeWidth * aspectRatio) / 2f),
                (int)((graphics.PreferredBackBufferHeight - S.GameFixeHeight * aspectRatio) / 2f),
                (int)(S.GameFixeWidth * aspectRatio),
                (int)(S.GameFixeHeight * aspectRatio)
            ), new RenderTarget2D(GraphicsDevice, S.GameFixeWidth, S.GameFixeHeight));
        }

        ui = new FriteCollection.Environment(game.Rect,
            new RenderTarget2D(GraphicsDevice,
            game.Rect.Width / S.UICoef,
            game.Rect.Height / S.UICoef));
    }

    public override bool FullScreen
    {
        get => this._fullScreen;

        set
        {
            UpdateEnvironments();
            FriteCollection.Entity.Space.SetDefaultEnvironment(ref game);
            FriteCollection.UI.UI.Rectangle.SetDefaultEnvironment(in ui);

            base.FullScreen = value;
        }
    }

    public override void UpdateScriptToScene()
    {
        base.UpdateScriptToScene();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (!changingScene)
        {
            foreach (Executable script in CurrentExecutables)
            {
                script.BeforeUpdate();
            }
            foreach (Executable script in CurrentExecutables)
            {
                script.Update();
            }
            foreach (FriteCollection.UI.ButtonCore but in _buttons)
            {
                but.Update();
            }
            foreach (Executable script in CurrentExecutables)
            {
                script.AfterUpdate();
            }
        }
    }

    protected override void OnDraw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(game.Target);
        GraphicsDevice.Clear(Screen.backGround);

        if (!changingScene)
        {
            foreach (Executable script in CurrentExecutables)
            {
                script.BeforeDraw(ref SpriteBatch);
            }

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
                script.AfterDraw(ref SpriteBatch);
            }

            SpriteBatch.Begin(blendState: BlendState.Additive, samplerState: SamplerState.PointClamp);
            foreach (Executable script in CurrentExecutables)
            {
                script.DrawAdditive();
            }
            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(ui.Target);

            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(samplerState: drawstate);
            SpriteBatch.Draw(
                game.Target,
                new Rectangle(0, 0, game.Rect.Width, game.Rect.Height),
                Color.White);

            SpriteBatch.End();
            SpriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            foreach (Executable script in CurrentExecutables)
            {
                script.DrawUI();
            }
            foreach (Executable script in CurrentExecutables)
            {
                script.DrawUI(ref SpriteBatch);
            }
            SpriteBatch.End();
        }
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(samplerState: drawstate);
        SpriteBatch.Draw(ui.Target, ui.Rect, Color.White);
        SpriteBatch.End();

        changingScene = false;
    }
}