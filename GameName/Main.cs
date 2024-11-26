using FriteCollection.Scripting;

namespace GameName
{
    public class Settings : GetSeetings, ISetGameSettings
    {
        public void SetGameSettings()
        {
            Settings.WindowWidth = 1000;
            Settings.WindowHeight = 800;
            Settings.GameFixeWidth = 200;
            Settings.GameFixeHeight = 160;
            Settings.FPS = 165;
        }
    }

    public enum Scenes
    {
        Scene1
    }
}
