using System;
using FriteCollection.Entity;

namespace FriteCollection.Graphics
{
    /// <summary>
    /// Rouge (0-1), Vert (0-1), Bleu (0-1)
    /// </summary>
    public class RGB : ICopy<RGB>
    {
        private byte _r, _g, _b;

        public RGB Copy()
        {
            return new RGB(_r, _g, _b);
        }

        public static RGB operator + (RGB rgb, float v) { return new RGB(rgb.R + v, rgb.G + v, rgb.B + v); }
        public static RGB operator -(RGB rgb, float v) { return new RGB(rgb.R - v, rgb.G - v, rgb.B - v); }
        public static RGB operator *(RGB rgb, float v) { return new RGB(rgb.R * v, rgb.G * v, rgb.B * v); }
        public static RGB operator /(RGB rgb, float v) { return new RGB(rgb.R / v, rgb.G / v, rgb.B / v); }


        public static RGB operator +(RGB rgb, RGB rgb2) { return new RGB(rgb.R + rgb2.R, rgb.G + rgb2.G, rgb.B + rgb2.B); }
        public static RGB operator -(RGB rgb, RGB rgb2) { return new RGB(rgb.R - rgb2.R, rgb.G - rgb2.G, rgb.B - rgb2.B); }
        public static RGB operator *(RGB rgb, RGB rgb2) { return new RGB(rgb.R * rgb2.R, rgb.G * rgb2.G, rgb.B * rgb2.B); }
        public static RGB operator /(RGB rgb, RGB rgb2) { return new RGB(rgb.R / rgb2.R, rgb.G / rgb2.G, rgb.B / rgb2.B); }

        /// <summary>
        /// Rouge (0-1)
        /// </summary>
        public float R
        {
            get { return _r / 255f; }
            set { _r = (byte)(MathF.Max(MathF.Min(value, 1), 0) * 255); }
        }

        /// <summary>
        /// Vert (0-1)
        /// </summary>
        public float G
        {
            get { return _g / 255f; }
            set { _g = (byte)(MathF.Max(MathF.Min(value, 1), 0) * 255); }
        }

        /// <summary>
        /// Bleu (0-1)
        /// </summary>
        public float B
        {
            get { return _b / 255f; }
            set { _b = (byte)(MathF.Max(MathF.Min(value, 1), 0) * 255); }
        }

        /// <summary>
        /// Rouge (0-1), Vert (0-1), Bleu (0-1)
        /// </summary>
        public RGB()
        {
            _r = 0;
            _g = 0;
            _b = 0;
        }

        /// <summary>
        /// Rouge (0-1), Vert (0-1), Bleu (0-1)
        /// </summary>
        public RGB(float red, float green, float blue)
        {
            _r = (byte)(MathF.Max(MathF.Min(red, 1), 0) * 255);
            _g = (byte)(MathF.Max(MathF.Min(green, 1), 0) * 255);
            _b = (byte)(MathF.Max(MathF.Min(blue, 1), 0) * 255);
        }

        /// <summary>
        /// Rouge (0-255), Vert (0-255), Bleu (0-255)
        /// </summary>
        public RGB(byte red, byte green, byte blue)
        {
            _r = red;
            _g = green;
            _b = blue;
        }

        public override string ToString()
        {
            return "RGB(" + _r + ", " + _g + ", " + _b + ")";
        }
    }

    /// <summary>
    /// Couleur (0-360), Saturation (0-1), Luminosité (0-1)
    /// </summary>
    public class HSV : ICopy<HSV>
    {
        private float _h;
        private byte _s, _v;

        public static HSV operator +(HSV rgb, float v) { return new HSV(rgb.H + v, rgb.S + v, rgb.V + v); }
        public static HSV operator -(HSV rgb, float v) { return new HSV(rgb.H - v, rgb.S - v, rgb.V - v); }
        public static HSV operator *(HSV rgb, float v) { return new HSV(rgb.H * v, rgb.S * v, rgb.V * v); }
        public static HSV operator /(HSV rgb, float v) { return new HSV(rgb.H / v, rgb.S / v, rgb.V / v); }


        public static HSV operator +(HSV rgb, HSV rgb2) { return new HSV(rgb.H + rgb2.H, rgb.S + rgb2.S, rgb.V + rgb2.V); }
        public static HSV operator -(HSV rgb, HSV rgb2) { return new HSV(rgb.H - rgb2.H, rgb.S - rgb2.S, rgb.V - rgb2.V); }
        public static HSV operator *(HSV rgb, HSV rgb2) { return new HSV(rgb.H * rgb2.H, rgb.S * rgb2.S, rgb.V * rgb2.V); }
        public static HSV operator /(HSV rgb, HSV rgb2) { return new HSV(rgb.H / rgb2.H, rgb.S / rgb2.S, rgb.V / rgb2.V); }

        public HSV Copy()
        {
            return new HSV(_h, _s, _v);
        }

        /// <summary>
        /// Couleur (0-360)
        /// </summary>
        public float H
        {
            get { return _h; }
            set
            { 
                _h = (value % 360);
            }
        }

        /// <summary>
        /// Saturation (0-1)
        /// </summary>
        public float S
        {
            get { return _s / 255f; }
            set
            {
                _s = (byte)(MathF.Max(MathF.Min(value, 1), 0) * 255);
            }
        }

        /// <summary>
        /// Luminosité (0-1)
        /// </summary>
        public float V
        {
            get { return _v / 255f; }
            set {
                _v = (byte)(MathF.Max(MathF.Min(value, 1), 0) * 255);}
        }

        /// <summary>
        /// Couleur (0-360), Saturation (0-1), Luminosité (0-1)
        /// </summary>
        public HSV()
        {
            _h = 0;
            _s = 0;
            _v = 0;
        }

        /// <summary>
        /// Couleur (0-360), Saturation (0-1), Luminosité (0-1)
        /// </summary>
        public HSV(float hue, float saturation, float value)
        {
            _h = hue % 360;
            _s = (byte)(MathF.Max(MathF.Min(saturation, 1), 0) * 255);
            _v = (byte)(MathF.Max(MathF.Min(value, 1), 0) * 255);
        }

        public override string ToString()
        {
            return "HSV(" + _h + ", " + _s + ", " + _v + ")";
        }
    }

    /// <summary>
    /// Couleur
    /// </summary>
    public class Color : ICopy<Color>
    {
        private RGB _rgb = new(0, 0, 0);
        private HSV _hsv = new(0, 0, 0);
        private string _lastEdit = "rgb";

        public static readonly Color White = new Color(1, 1, 1);
        public static readonly Color Black = new Color(0, 0, 0);

        public static Color operator * (Color c1, float m)
        { return new Color(c1.RGB.R * m, c1.RGB.G * m, c1.RGB.B * m); }
        public static Color operator / (Color c1, float m)
        { return new Color(c1.RGB.R / m, c1.RGB.G / m, c1.RGB.B / m); }

        public Color Copy()
        {
            Color c = new Color(_rgb.R, _rgb.G, _rgb.B);
            return c;
        }

        /// <summary>
        /// Noire
        /// </summary>
        public Color()
        {

        }

        /// <summary>
        /// Rouge (0-1), Vert (0-1), Bleu (0-1)
        /// </summary>
        public Color(float r, float g, float b)
        {
            _rgb.R = r;
            _rgb.G = g;
            _rgb.B = b;
        }

        /// <summary>
        /// Rouge (0-1), Vert (0-1), Bleu (0-1)
        /// </summary>
        public RGB RGB
        {
            get
            {
                if (_lastEdit == "hsv")
                {
                    float H = _hsv.H;
                    float S = _hsv.S;
                    float V = _hsv.V;

                    float C = V * S;
                    float X = C * (1 - MathF.Abs(H / 60f % 2 - 1));
                    float m = V - C;

                    float Rp = 0;
                    float Gp = 0;
                    float Bp = 0;

                    if (0 <= H && H < 60) { Rp = C; Gp = X; Bp = 0; }
                    else if (60 <= H && H < 120) { Rp = X; Gp = C; Bp = 0; }
                    else if (120 <= H && H < 180) { Rp = 0; Gp = C; Bp = X; }
                    else if (180 <= H && H < 240) { Rp = 0; Gp = X; Bp = C; }
                    else if (240 <= H && H < 300) { Rp = X; Gp = 0; Bp = C; }
                    else if (300 <= H && H < 360) { Rp = C; Gp = 0; Bp = X; }
                    else { Rp = 0; Gp = 0; Bp = 0; }

                    _rgb.R = Rp + m;
                    _rgb.G = Gp + m;
                    _rgb.B = Bp + m;

                    _lastEdit = "rgb";
                }

                return _rgb;
            }
            set
            {
                _rgb = value;
                _lastEdit = "rgb";
            }
        }

        /// <summary>
        /// Couleur (0-360), Saturation (0-1), Luminosité (0-1)
        /// </summary>
        public HSV HSV
        {
            get
            {
                if (_lastEdit == "rgb")
                {
                    float Rp = _rgb.R;
                    float Gp = _rgb.G;
                    float Bp = _rgb.B;

                    float Cmax = MathF.Max(MathF.Max(Rp, Gp), Bp);
                    float Cmin = MathF.Min(MathF.Min(Rp, Gp), Bp);
                    float delta = Cmax - Cmin;

                    float H = 0;
                    if (delta == 0) { H = 0; }
                    else if (Cmax == Rp) { H = 60 * ((Gp - Bp) / delta % 6); }
                    else if (Cmax == Gp) { H = 60 * ((Bp - Rp) / delta + 2); }
                    else if (Cmax == Bp) { H = 60 * ((Rp - Gp) / delta + 4); }

                    float S = 0;
                    if (Cmax == 0) { S = 0; }
                    else { S = delta / Cmax; }

                    float V = 0;
                    V = Cmax;

                    _hsv = new HSV(H, S, V);
                    _lastEdit = "hsv";
                }

                return _hsv;
            }

            set
            {
                _hsv = value;
                _lastEdit = "hsv";
            }
        }
    }
}