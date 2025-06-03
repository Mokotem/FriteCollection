using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;

namespace FriteCollection;

public enum Bounds
{
    TopLeft, Top, TopRight,
    Left, Center, Right,
    BottomLeft, Bottom, BottomRight,
}

public class Settings
{
    public int WindowWidth = 800;
    public int WindowHeight = 600;
    public int GameFixeWidth = 800;
    public int GameFixeHeight = 600;
    public int ReferenceWidth = 800;
    public int ReferenceHeight = 600;
    public bool FullScreen = false;
    public bool PixelArtDrawing = false;
    public string WindowName = "";
    public byte UICoef = 1;
    public byte StartScene = 0;
    public byte FPS = 60;
    public bool AllowUserResizeing = false;
}

public static class GameManager
{
    private static FriteModel.MonoGame _nstnc;

    private static ContentManager content;
    public static void SetContentRef(in ContentManager _content)
    {
        content = _content;
    }

    public static void SetGameInstance(FriteModel.MonoGame _instance)
    {
        _nstnc = _instance;
    }

    public static void Quit()
    {
        _nstnc.Exit();
    }

    public static bool FullScreen
    {
        get => _nstnc.FullScreen;
        set => _nstnc.FullScreen = value;
    }

    public static Environment[] Envi => _nstnc.Environments;

    internal static FriteModel.MonoGame Instance => _nstnc;

    public static void UpdateEnvironments()
    {
        _nstnc.UpdateEnvironments();
    }
    
    private static Settings _settings;
    
    private static ushort _fps;
    internal static ushort Fps
    {
        get => _fps;
        set
        {
            _fps = value;
            Instance.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / value);
        }
    }

    public static GraphicsDevice GraphicsDevice => _nstnc.GraphicsDevice;

    public static Settings Settings => _settings;

    private static SpriteFont _font;
    public static SpriteFont Font => _font;

    public static void SetFont(SpriteFont font)
    {
        _font = font;
    }

    public static void SetSettings(Settings settings)
    {
        _settings = settings;
        _currentScene = settings.StartScene;
    }

    public static void Print(params object[] listText)
    {
        string finalTxt = "";
        foreach (object s in listText) { finalTxt += s.ToString() + "  "; }
        System.Diagnostics.Debug.WriteLine(finalTxt);
    }

    private static byte _currentScene;
    /// <summary>
    /// Scène en cour d'execution.
    /// </summary>
    public static byte CurrentScene
    {
        get
        {
            return _currentScene;
        }

        set
        {
            _currentScene = value;
            content.Unload();
            Instance.UpdateScriptToScene();
        }
    }

    public delegate Texture2D TextureCreator(GraphicsDevice graphic, ref SpriteBatch batch);
    public delegate void TextureModifier(GraphicsDevice graphic, ref SpriteBatch batch, Texture2D texture);
    public delegate void TextureMaker(GraphicsDevice graphic, ref SpriteBatch batch);

    public static Texture2D MakeTextureCreator(TextureCreator method)
    {
        return method(Instance.GraphicsDevice, ref Instance.SpriteBatch);
    }

    public static void MakeTextureModifier(TextureModifier method, Texture2D texture)
    {
        method(Instance.GraphicsDevice, ref Instance.SpriteBatch, texture);
    }

    public static void MakeTexture(TextureMaker method)
    {
        method(Instance.GraphicsDevice, ref Instance.SpriteBatch);
    }
}

public static class SaveManager
{
    private const string foldername = "BallBallGame";
    private static readonly string folder = Path.Combine
        (System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
        @foldername);
    private static readonly string path = Path.Combine
        (System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
        @"BallBallGame\Save_BallBallGame.json");
    private static FileInfo _info;

    public static string SavePath
    {
        get
        {
            return path;
        }
    }

    public static bool SaveExist
    {
        get
        {
            return Directory.Exists(folder) && File.Exists(path);
        }
    }

    /// <summary>
    /// bytes.
    /// </summary>
    public static long SpaceTaking => _info is null ? 0 : _info.Length;

    public static void Save<T>(T _struct)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string save = JsonSerializer.Serialize<T>(_struct);
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write(save);
        }
        _info = new FileInfo(path);
    }

    public static void Delete()
    {
        try
        {
            File.Delete(path);
            Directory.Delete(folder);
            _info = null;
        }
        catch
        {
            throw new Exception("failed to delete");
        }
    }

    public static T Load<T>()
    {
        string file;
        using (StreamReader sr = new StreamReader(path))
            file = sr.ReadToEnd();
        _info = new FileInfo(path);
        T data = JsonSerializer.Deserialize<T>(file);
        return data;
    }
}

/// <summary>
/// Caméra
/// </summary>
public static class Camera
{
    /// <summary>
    /// Position de la caméra.
    /// </summary>
    private static Vector2 _position = Vector2.Zero;

    public static Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    /// <summary>
    /// Les objets de la meme origine que la caméra bougent.
    /// </summary>
    public static Bounds GridOrigin = Bounds.Center;

    /// <summary>
    /// Facteur de zoom.
    /// </summary>
    public static float zoom = 1f;
}

/// <summary>
/// Données sur la fenêtre du projet.
/// </summary>
public static class Screen
{
    /// <summary>
    /// Couleur d'arrière plan.
    /// </summary>
    public static Color backGround = new(0.1f, 0.2f, 0.3f);

    internal static int rww, rwh;
    public static int WindowWidth => rww;
    public static int WindowHeight => rwh;

    public static readonly int widht = GameManager.Settings.GameFixeWidth, height = GameManager.Settings.GameFixeHeight;
}
