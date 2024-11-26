namespace FriteCollection.Random
{
    public abstract class Random
    {
        public static float Float(float b1, float b2)
        {
            return b1 + ((float)(new System.Random().NextDouble()) * (b2 - b1));
        }

        public static int Int(int b1, int b2)
        {
            return (int)new System.Random().NextInt64(b1, b2 + 1);
        }

        public static bool Bool()
        {
            return new System.Random().NextInt64(0, 2) == 1;
        }
    }
}
