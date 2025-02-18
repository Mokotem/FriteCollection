using FriteCollection.Graphics;

namespace FriteCollection.Tools.Animation
{
    public class Animation
    {
        public delegate void KeyFrame(float dt);

        private readonly KeyFrame[] frames;
        private readonly float[] durations;
        private readonly float start;

        public Animation(KeyFrame[] frames, float[] durations, float startTime = 0f)
        {
            if (frames.Length < 1 || durations.Length != frames.Length)
                throw new System.Exception("Frame count should be the same as durations");

            this.frames = frames;
            this.durations = durations;
            this.start = startTime;
            currentKey = -1;
            b = 0f;
            a = 0f;
        }

        private int currentKey;
        private float a, b;

        public bool Done => currentKey >= frames.Length;

        public void Animate(float timer)
        {
            if (!Done)
            {
                while (currentKey < frames.Length - 1
                    && timer > start + b)
                {
                    a = b;
                    currentKey += 1;
                    if (durations[currentKey] <= 0)
                    {
                        frames[currentKey](0);
                    }
                    else
                    {
                        b += durations[currentKey];
                    }
                }
                if (currentKey >= 0)
                {
                    frames[currentKey]((timer - a - start) / durations[currentKey]);
                }
            }
        }
    }

    public static class Interpolation
    {
        public static float Linear(float a, float b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            return (a * (1 - t)) + (b * t);
        }
        public static float EaseIn(float a, float b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float k = t * t;
            return (a * (1 - k)) + (b * k);
        }

        public static float EaseOut(float a, float b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float k = t * (2 - t);
            return (a * (1 - k)) + (b * k);
        }

        public static float EaseInOut(float a, float b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float s = float.Sin((float.Pi * t) / 2f);
            float k = s * s;
            return (a * (1 - k)) + (b * k);
        }

        public static float BackIn(float a, float b, float t, float k = 4)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float t2 = t * t;
            float t3 = t2 * t;
            float q = -t3 + (2 * t2) + (k * (t3 - t2));
            return (a * (1 - q)) + (b * q);
        }

        public static float BackOut(float a, float b, float t, float k = 4)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float t2 = t * t;
            float t3 = t2 * t;
            float q = t2 - t3 + t + k * (t3 - (2 * t2) + t);
            return (a * (1 - q)) + (b * q);
        }

        public static float Triangle(float a, float b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return a;
            float q = 1 - (2 *  float.Abs(t - 0.5f));
            return a * (1 - q) + (b * q);
        }

        public static Color Linear(Color a, Color b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            return (a * (1 - t)) + (b * t);
        }
        public static Color EaseIn(Color a, Color b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float k = t * t;
            return (a * (1 - k)) + (b * k);
        }

        public static Color EaseOut(Color a, Color b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float k = t * (2 - t);
            return (a * (1 - k)) + (b * k);
        }

        public static Color EaseInOut(Color a, Color b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float s = float.Sin((float.Pi * t) / 2f);
            float k = s * s;
            return (a * (1 - k)) + (b * k);
        }

        public static Color BackIn(Color a, Color b, float t, float k = 4)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float t2 = t * t;
            float t3 = t2 * t;
            float q = -t3 + (2 * t2) + (k * (t3 - t2));
            return (a * (1 - q)) + (b * q);
        }

        public static Color BackOut(Color a, Color b, float t, float k = 4)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float t2 = t * t;
            float t3 = t2 * t;
            float q = t2 - t3 + t + k * (t3 - (2 * t2) + t);
            return (a * (1 - q)) + (b * q);
        }

        public static Color Triangle(Color a, Color b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return a;
            float q = 1 - (2 * float.Abs(t - 0.5f));
            return a * (1 - q) + (b * q);
        }

        public static Vector Linear(Vector a, Vector b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            return (a * (1 - t)) + (b * t);
        }
        public static Vector EaseIn(Vector a, Vector b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float k = t * t;
            return (a * (1 - k)) + (b * k);
        }

        public static Vector EaseOut(Vector a, Vector b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float k = t * (2 - t);
            return (a * (1 - k)) + (b * k);
        }

        public static Vector EaseInOut(Vector a, Vector b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float s = float.Sin((float.Pi * t) / 2f);
            float k = s * s;
            return (a * (1 - k)) + (b * k);
        }

        public static Vector BackIn(Vector a, Vector b, float t, float k = 4)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float t2 = t * t;
            float t3 = t2 * t;
            float q = -t3 + (2 * t2) + (k * (t3 - t2));
            return (a * (1 - q)) + (b * q);
        }

        public static Vector BackOut(Vector a, Vector b, float t, float k = 4)
        {
            if (t <= 0) return a;
            if (t >= 1) return b;
            float t2 = t * t;
            float t3 = t2 * t;
            float q = t2 - t3 + t + k * (t3 - (2 * t2) + t);
            return (a * (1 - q)) + (b * q);
        }

        public static Vector Triangle(Vector a, Vector b, float t)
        {
            if (t <= 0) return a;
            if (t >= 1) return a;
            float q = 1 - (2 * float.Abs(t - 0.5f));
            return a * (1 - q) + (b * q);
        }
    }
}