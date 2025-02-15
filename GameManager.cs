using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.IO;

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
    public bool FullScreen = false;
    public bool PixelArtDrawing = false;
    public string WindowName = "";
    public byte UICoef = 1;
    public byte StartScene = 0;
    public byte FPS = 60;
    public SpriteFont Font;
}

public static class GameManager
{
    private static FriteModel.MonoGame _nstnc;
    internal static void SetGameInstance(FriteModel.MonoGame _instance)
    {
        _nstnc = _instance;
    }

    internal static FriteModel.MonoGame Instance => _nstnc;
    
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


    internal static Settings Settings => _settings;

    public static SpriteFont Font => _settings.Font;

    public static void SetSettings(Settings settings)
    {
        _settings = settings;
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
            Instance.UpdateScriptToScene();
        }
    }
}

/// <summary>
/// Musique. Les variables 'Music' ne peuvent pas être joué en même temps.
/// </summary>
public static class Open
{
    /// <summary>
    /// Ouvrir une texture. (png, jpg,...)
    /// </summary>
    public static Texture2D Texture(string path)
    {
        return GameManager.Instance.Content.Load<Texture2D>(path);
    }

    /// <summary>
    /// Ouvrir une police. (.ttf)
    /// </summary>
    public static SpriteFont Font(string path)
    {
        return GameManager.Instance.Content.Load<SpriteFont>(path);
    }

    /// <summary>
    /// [pas dans Content] Ouvrir une tileMap (.Json)
    /// </summary>
    public static Tools.TileMap.OgmoFile OgmoTileMap(string path)
    {
        string file;
        using (StreamReader sr = new StreamReader(AppContext.BaseDirectory + path))
            file = sr.ReadToEnd();
        return JsonConvert.DeserializeObject<Tools.TileMap.OgmoFile>(file);
    }
}

public static class SaveManager
{
    private const string foldername = "BallBallGame";
    private static readonly string folder = Path.Combine
        (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        @foldername);
    private static readonly string path = Path.Combine
        (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
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

    public static void Save(object _struct)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        string save = JsonConvert.SerializeObject(_struct);
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
        T data = JsonConvert.DeserializeObject<T>(file);
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
    private static Vector _position = Vector.Zero;

    public static Vector Position
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
    public static Graphics.Color backGround = new(0.1f, 0.2f, 0.3f);
    public static readonly int widht = GameManager.Settings.GameFixeWidth, height = GameManager.Settings.GameFixeHeight;
}
