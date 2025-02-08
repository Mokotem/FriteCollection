
/// <summary>
/// Module Random fait maison parceque c'est marant.
/// </summary>
namespace FriteCollection.Random;

public class Random
{
    private readonly uint _seed;
    public uint Seed => _seed;

    public Random(uint seed)
    {
        this._seed = seed;
        this._branche = F2(F1(seed));
    }

    private ulong _branche;
    private ulong Branche => ((_branche - (_branche % 10)) / 10);

    private delegate ulong Modifier(ulong n);

    private ulong F1(ulong n)
    {
        return n * (n + 2) + 9 + (n % 100);
    }
    private ulong F2(ulong n)
    {
        return (n + 1) * n * 3 + 4 + (_seed % 12);
    }

    private void _next()
    {
        Modifier f = _branche % 2 == 0 ? F1 : F2;
        ulong r = f(_branche);

        if (r > ulong.MaxValue)
            r = (r - (r % 100)) / 100;
        _branche = r;
    }

    public ulong Next(ulong maxValue)
    {
        _next();
        return Branche % maxValue;
    }

    public float Next(float maxValue)
    {
        _next();
        return ((float)Branche * maxValue) / ((float)ulong.MaxValue);
    }
}