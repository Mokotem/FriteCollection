using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Reflection;
using FriteCollection.Entity;
using FriteCollection.Audio;

namespace FriteCollection.Scripting
{
    public abstract class GameManager
    {
        public static void Print(params object[] listText)
        {
            string finalTxt = "";
            foreach (object s in listText) { finalTxt += s.ToString() + "  "; }
            System.Diagnostics.Debug.WriteLine(finalTxt);
        }

        private static Module.Scenes _currentScene;
        /// <summary>
        /// Scène en cour d'execution.
        /// </summary>
        public static Module.Scenes CurrentScene
        {
            get
            {
                return _currentScene;
            }

            set
            {
                _currentScene = value;
                FriteModel.MonoGame.instance.UpdateScriptToScene();
            }
        }

        /// <summary>
        /// Acceder à un autre script de la meme scène.
        /// </summary>
        public static Script GetScript(string name)
        {
            foreach (Script script in FriteModel.MonoGame.instance.CurrentExecutables)
            {
                if (script.GetType().Name == name)
                    return script;
            }

            throw new Exception("Le script '"+name+"' n'existe pas.");
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
            return FriteModel.MonoGame.instance.Content.Load<Texture2D>(path);
        }

        /// <summary>
        /// Ouvrir un son. (mp3, wma, ogg)
        /// </summary>
        public static Music Music(string path)
        {
            return new Music(FriteModel.MonoGame.instance.Content.Load<Microsoft.Xna.Framework.Media.Song>(path));
        }

        /// <summary>
        /// Ouvrir un son. (wav)
        /// </summary>
        public static FriteCollection.Audio.SoundEffect SoundEffect(string path)
        {
            return new FriteCollection.Audio.SoundEffect
                (FriteModel.MonoGame.instance.Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>(path));
        }

        /// <summary>
        /// Ouvrir une police. (.ttf)
        /// </summary>
        public static SpriteFont Font(string path)
        {
            return FriteModel.MonoGame.instance.Content.Load<SpriteFont>(path);
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

    public abstract class SaveManager
    {
        private static readonly string path = Path.Combine
            (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"FriteCollection\Save_" + Assembly.GetAssembly(typeof(Module.Settings)).FullName + ".json");

        public static string SavePath
        {
            get
            {
                return path;
            }
        }

        public static bool FileExist
        {
            get
            {
                return File.Exists(path);
            }
        }

        public static void Save(object _struct)
        {
            string save = JsonConvert.SerializeObject(_struct);
            using (StreamWriter sw = new StreamWriter(path))
                sw.Write(save);
        }

        public static T Load<T>()
        {
            string file;
            using (StreamReader sr = new StreamReader(path))
                file = sr.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(file);
        }
    }

    /// <summary>
    /// Caméra
    /// </summary>
    public abstract class Camera
    {
        /// <summary>
        /// Position de la caméra.
        /// </summary>
        public static Vector Position = Vector.Zero;

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
    public abstract class Screen
    {
        /// <summary>
        /// Couleur d'arrière plan.
        /// </summary>
        public static FriteCollection.Graphics.Color backGround = new(0.1f, 0.2f, 0.3f);
        public static readonly int widht = GetSeetings.Settings.GameFixeWidth, height = GetSeetings.Settings.GameFixeHeight;
        public static Tools.Shaders.Shader Shader
        {
            set
            {

            }
        }
    }
}
