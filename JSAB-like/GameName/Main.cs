
using FriteCollection.Scripting;

namespace GameName;

public static class Main
{
    public static Settings SetGameSettings()
    {
        return new Settings()
        {
            WindowWidth = 720,
            WindowHeight = 720,
            GameFixeWidth = 720,
            GameFixeHeight = 720,
            StartScene = 0
        };
    }
}

public enum Scenes
{
    Game
}