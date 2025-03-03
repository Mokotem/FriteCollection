using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FriteCollection.Tools.TileMap;
using FriteCollection.Scripting;
using System;

namespace FriteCollection.UI;

public enum Extend
{
    Horizontal, Vertical, Full, None
}

public class Rectangle
{
    public Extend Extend = Extend.None;
    public Point Position = Point.Zero;
    public Point Scale = Point.Zero;
    public Bounds Origin;

    public Rectangle(Bounds origin, Extend extend)
    {
        this.Origin = origin;
        this.Extend = extend;
    }

    public Rectangle(Bounds origin, Extend extend, Point scale)
    {
        this.Origin = origin;
        this.Extend = extend;
        this.Scale = scale;
    }

    public Rectangle(Bounds origin, Extend extend, Point scale, Point position)
    {
        this.Origin = origin;
        this.Extend = extend;
        this.Scale = scale;
        this.Position = position;
    }
}

public abstract class UI : IDisposable
{
    public virtual void Dispose()
    {
        if (papa is not null)
            papa.Dispose();
    }

    private protected bool _active = true;
    public delegate void Procedure();
    private protected UI papa;

    public Point Scale => space.Scale;
    public float alpha = 1f;
    private protected Microsoft.Xna.Framework.Color GetColor()
    {
        return new Color(this.Color.RGB.R, this.Color.RGB.G, this.Color.RGB.B) * alpha;
    }

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    private protected Microsoft.Xna.Framework.Rectangle rect;

    public virtual Point Position
    {
        get => space.Position;
        set
        {
            this.space.Position = value;
            ApplyPosition(papa is null ? Screen : papa.Rectangle);
        }
    }

    public virtual int PositionY
    {
        get => rect.Y;
        set
        {
            this.space.Position.j = value;
            ApplyPosition(papa is null ? Screen : papa.Rectangle);
        }
    }

    private protected List<UI> childs = new List<UI>();
    public void Add(UI element)
    {
        childs.Add(element);
    }

    public Microsoft.Xna.Framework.Rectangle Rectangle => rect;
    public static readonly Microsoft.Xna.Framework.Rectangle Screen = new Microsoft.Xna.Framework.Rectangle
(0, 0, GameManager.Settings.GameFixeWidth * GameManager.Settings.UICoef,
       GameManager.Settings.GameFixeHeight * GameManager.Settings.UICoef);

    public Graphics.Color Color = Graphics.Color.White;

    private protected Rectangle space;

    private protected void ApplyScale(Microsoft.Xna.Framework.Rectangle parent)
    {
        switch (space.Extend)
        {
            case Extend.None:
                rect.Width = 0;
                rect.Height = 0;
                break;

            case Extend.Full:
                rect.Width = parent.Width;
                rect.Height = parent.Height;
                break;

            case Extend.Horizontal:
                rect.Width = parent.Width;
                rect.Height = 0;
                break;

            case Extend.Vertical:
                rect.Width = 0;
                rect.Height = parent.Height;
                break;
        }

        rect.Width += space.Scale.i;
        rect.Height += space.Scale.j;
    }

    private protected void ApplyPosition(Microsoft.Xna.Framework.Rectangle parent)
    {
        switch ((int)space.Origin % 3)
        {
            default:
                rect.X = parent.X;
                break;

            case 1:
                rect.X = parent.X + (parent.Width / 2) - (rect.Width / 2);
                break;

            case 2:
                rect.X = parent.X + parent.Width - rect.Width;
                break;
        }

        switch ((int)space.Origin / 3)
        {
            default:
                rect.Y = parent.Y;
                break;

            case 1:
                rect.Y = parent.Y + (parent.Height / 2) - (rect.Height / 2);
                break;

            case 2:
                rect.Y = parent.Y + parent.Height - rect.Height;
                break;
        }

        rect.X += space.Position.i;
        rect.Y += space.Position.j;
    }

    private protected void ApplySpace(Microsoft.Xna.Framework.Rectangle parent)
    {
        ApplyScale(parent);
        ApplyPosition(parent);
    }

    public virtual void Draw() { }
}

public abstract class ButtonCore : UI
{
    private Text titleText;
    private FriteModel.MonoGame I => GameManager.Instance;

    private bool clic => GameManager.Instance.IsActive && _active && Input.Mouse.Sate.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
&& IsInRange(I.mouseClickedPosition)
&& IsInRange(I.mousePosition)
&& Time.TargetTimer >= 0.2f;

    private bool IsInRange(Point pos) =>
        GameManager.Instance.IsActive
     && pos.i >= papa.Rectangle.X - 1 && pos.i < papa.Rectangle.X + papa.Rectangle.Width + 1
     && pos.j >= papa.Rectangle.Y - 1 && pos.j < papa.Rectangle.Y + papa.Rectangle.Height + 1;

    private protected bool selected = false;
    private bool previousClic = false;

    private protected Procedure _fonction;

    public override void Dispose()
    {
        I.buttons.Remove(this);
        _fonction = null;
        papa.Dispose();
        if (titleText is not null)
        {
            titleText.Dispose();
        }
    }

    internal void Update()
    {
        if (_active)
        {
            selected = IsInRange(I.mousePosition);

            if (clic)
            {
                papa.Color = new Graphics.Color(0.7f, 0.7f, 0.7f);
                if (titleText is not null)
                    titleText.Color = new Graphics.Color(0.7f, 0.7f, 0.7f);
            }
            else
            {
                papa.Color = Graphics.Color.White;
                if (titleText is not null)
                    titleText.Color = Graphics.Color.White;

                if (previousClic == true && _fonction is not null && IsInRange(I.mousePosition))
                {
                    I.previousMouseLeft = false;
                    _fonction();
                }
            }
        }

        previousClic = clic;
    }

    public override int PositionY
    {
        get => papa.Position.j;
        set
        {
            papa.PositionY = value;
            titleText.SetPar(papa.Rectangle);
            titleText.PositionY = 0;
        }
    }

    public string EditText
    {
        set
        {
            if (titleText is null)
            {
                titleText = new Text(value, new Rectangle(Bounds.Center, Extend.Full), papa);
                papa.Add(titleText);
            }
            else
                titleText.EditText = value;
        }
    }

    public ButtonCore(TileSet tileset, Rectangle space, UI parent)
    {
        papa = new Panel(tileset, space, parent);
        I.buttons.Add(this);
    }


    public ButtonCore(TileSet tileset, Rectangle space)
    {
        papa = new Panel(tileset, space);
        I.buttons.Add(this);
    }

    public ButtonCore(Texture2D image, Rectangle space, UI parent)
    {
        papa = new Image(image, space, parent);
        I.buttons.Add(this);
    }

    public ButtonCore(string title, TileSet tileset, Rectangle space, UI parent)
    {
        papa = new Panel(tileset, space, parent);
        titleText = new Text(title, new Rectangle(Bounds.Center, Extend.Full), papa);
        papa.Add(titleText);
        I.buttons.Add(this);
    }

    public ButtonCore(string title, Texture2D image, Rectangle space, UI parent)
    {
        papa = new Image(image, space, parent);
        titleText = new Text(title, new Rectangle(Bounds.Center, Extend.Full), papa);
        papa.Add(titleText);
        I.buttons.Add(this);
    }

    public ButtonCore(Texture2D image, Rectangle space)
    {
        papa = new Image(image, space);
        I.buttons.Add(this);
    }

    public ButtonCore(string title, TileSet tileset, Rectangle space)
    {
        papa = new Panel(tileset, space);
        titleText = new Text(title, new Rectangle(Bounds.Center, Extend.Full), papa);
        papa.Add(titleText);
        I.buttons.Add(this);
    }

    public ButtonCore(string title, Texture2D image, Rectangle space)
    {
        papa = new Image(image, space);
        titleText = new Text(title, new Rectangle(Bounds.Center, Extend.Full), papa);
        papa.Add(titleText);
        I.buttons.Add(this);
    }
}

public class Toggle : ButtonCore
{
    public Procedure OnActivate;
    public Procedure OnDeactivate;

    private bool _on = false;

    public void Set(bool value)
    {
        _on = value;
    }

    public bool On => _on;

    public Toggle[] voisins = new Toggle[0];
    public void Deactivate()
    {
        _on = false;
        OnDeactivate();
    }

    public Toggle(TileSet tileset, Rectangle space, UI parent) : base(tileset, space, parent) { _fonction = OnClic; }
    public Toggle(Texture2D image, Rectangle space, UI parent) : base(image, space, parent) { _fonction = OnClic; }
    public Toggle(string title, TileSet tileset, Rectangle space, UI parent) : base(title, tileset, space, parent) { _fonction = OnClic; }
    public Toggle(string title, Texture2D image, Rectangle space, UI parent) : base(title, image, space, parent) { _fonction = OnClic; }
    public Toggle(TileSet tileset, Rectangle space) : base(tileset, space) { _fonction = OnClic; }
    public Toggle(Texture2D image, Rectangle space) : base(image, space) { _fonction = OnClic; }
    public Toggle(string title, TileSet tileset, Rectangle space) : base(title, tileset, space) { _fonction = OnClic; }
    public Toggle(string title, Texture2D image, Rectangle space) : base(title, image, space) { _fonction = OnClic; }


    private void OnClic()
    {
        foreach(Toggle tog in voisins)
            tog.Deactivate();
        _on = !_on;
        if (_on)
            OnActivate();
        else
            OnDeactivate();
    }

    public override void Draw()
    {
        if (_active)
        {
            if (_on)
            {
                GameManager.Instance.SpriteBatch.Draw
                    (Entity.Renderer._defaultTexture, new Microsoft.Xna.Framework.Rectangle(
                        papa.Rectangle.X - 1,
                    papa.Rectangle.Y - 1, papa.Rectangle.Width + 2, papa.Rectangle.Height + 2),
                    Microsoft.Xna.Framework.Color.White);
            }
            papa.Draw();
        }
    }
}

public class Button : ButtonCore
{
    public Button(TileSet tileset, Rectangle space, UI parent) : base(tileset, space, parent) { }
    public Button(Texture2D image, Rectangle space, UI parent) : base(image, space, parent) { }
    public Button(string title, TileSet tileset, Rectangle space, UI parent) : base(title, tileset, space, parent) { }
    public Button(string title, Texture2D image, Rectangle space, UI parent) : base(title, image, space, parent) { }
    public Button(TileSet tileset, Rectangle space) : base(tileset, space) { }
    public Button(Texture2D image, Rectangle space) : base(image, space) { }
    public Button(string title, TileSet tileset, Rectangle space) : base(title, tileset, space) { }
    public Button(string title, Texture2D image, Rectangle space) : base(title, image, space) { }

    public Procedure Fonction
    {
        set
        {
            base._fonction = value;
        }
    }

    public override void Draw()
    {
        if (_active)
        {
            if (selected && Input.Mouse.Sate.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                GameManager.Instance.SpriteBatch.Draw
                    (Entity.Renderer._defaultTexture, new Microsoft.Xna.Framework.Rectangle(
                        papa.Rectangle.X - 1,
                    papa.Rectangle.Y - 1, papa.Rectangle.Width + 2, papa.Rectangle.Height + 2),
                    Microsoft.Xna.Framework.Color.White);
            }
            papa.Draw();
        }
    }

    public override void Dispose()
    {
        if (papa is not null)
        {
            papa.Dispose();
            papa = null;
        }
    }
}

public class Image : UI
{
    private Texture2D image;

    public Texture2D Texture
    {
        get => image;
        set => image = value;
    }

    public Image(Texture2D image, Rectangle space)
    {
        this.image = image;
        this.space = space;
        base.ApplyScale(Screen);
        base.ApplyPosition(Screen);
    }

    public Image(Texture2D image, Rectangle space, UI parent)
    {
        this.image = image;
        this.space = space;
        base.ApplyScale(parent.Rectangle);
        base.ApplyPosition(parent.Rectangle);
    }

    public override void Draw()
    {
        if (_active)
        {
            GameManager.Instance.SpriteBatch.Draw(image, rect,
            new(this.Color.RGB.R, this.Color.RGB.G, this.Color.RGB.B));
            foreach (UI element in childs)
                element.Draw();
        }
    }

    public override void Dispose()
    {
        if (image is not null)
        {
            image.Dispose();
            image = null;
        }
    }
}

public class Text : UI
{
    private Microsoft.Xna.Framework.Rectangle par;
    private string text;
    public bool Outline;

    public float Size { get; set; }

    public string EditText
    {
        get => text;
        set
        {
            if (value != text)
            {
                int posy = rect.Y;
                this.text = value;
                ApplyScale(par);
                ApplyText(value);
                ApplyPosition(par);
                rect.Y = posy;
            }
        }
    }

    public void SetPar(Microsoft.Xna.Framework.Rectangle rect)
    {
        par = rect;
    }

    public override int PositionY
    {
        get => papa.Position.j;
        set
        {
            space.Position.j = value;
            ApplyPosition(par);
        }
    }

    private void ApplyText(string input)
    {
        text = "";
        string[] txt = input.Split(" ");
        int i = 0;
        if (txt.Length < 2 || rect.Width < 2)
        {
            text = input;
        }
        else
        {
            while (i < txt.Length)
            {
                string line = "";
                while (GameManager.Font.MeasureString(line).X < rect.Width)
                {
                    line += txt[i] + " ";
                    i += 1;
                }
                text += line + "\n";
            }
        }

        Vector2 s = GameManager.Font.MeasureString(text);
        rect.Width = (int)s.X;
        rect.Height = (int)s.Y;
    }

    public Text(string txt, Rectangle space)
    {
        this.Size = 1f;
        this.space = space;
        base.ApplyScale(Screen);
        ApplyText(txt);
        base.ApplyPosition(Screen);
        par = Screen;
        Outline = false;
    }

    public Text(string txt, Rectangle space, UI parent)
    {
        this.Size = 1f;
        this.space = space;
        base.ApplyScale(parent.Rectangle);
        ApplyText(txt);
        base.ApplyPosition(parent.Rectangle);
        par = parent.Rectangle;
        Outline = false;
    }

    public override void Draw()
    {
        if (_active)
        {
            if (Outline)
            {
                foreach (Vector2 r in new Vector2[8]
                {
                new(-1, 1),
                new(0, 1),
                new(1, 1),

                new(-1, 0),
                new(1, 0),

                new(-1, -1),
                new(0, -1),
                new(1, -1)
                })
                {
                    GameManager.Instance.SpriteBatch.DrawString
                    (GameManager.Font, text, new Vector2(rect.X + r.X, rect.Y + r.Y),
                    Microsoft.Xna.Framework.Color.Black * alpha, 0, Vector2.Zero, Size,
                    SpriteEffects.None, 0);
                }
            }
            GameManager.Instance.SpriteBatch.DrawString
                (GameManager.Font, text, new Vector2(rect.X, rect.Y),
                this.GetColor(), 0, Vector2.Zero, Size,
                SpriteEffects.None, 0);
        }
    }
}


public class Panel : UI, IDisposable
{
    private TileSet tileSet;
    private Texture2D texture;
    private RenderTarget2D rt;

    private void CreateTexture()
    {
        int sx = tileSet.TileSize.i;
        int sy = tileSet.TileSize.j;
        if (rect.Width < sx * 2)
        {
            sx = rect.Width / 2;
        }
        if (rect.Height < sy * 2)
        {
            sy = rect.Height / 2;
        }

        GraphicsDevice gd = GameManager.Instance.GraphicsDevice;
        SpriteBatch sb = GameManager.Instance.SpriteBatch;
        rt = new RenderTarget2D(gd, rect.Width, rect.Height);

        gd.SetRenderTarget(rt);
        gd.Clear(Microsoft.Xna.Framework.Color.Transparent);
        sb.Begin(samplerState: SamplerState.PointClamp);

        for (int x = 0; x < 3; x++)
        {
            int width;
            if (x == 0 || x == 2)
                width = sx;
            else
                width = rect.Width - (sx * GameManager.Settings.UICoef);

            int posX;
            if (x == 0)
                posX = 0;
            else if (x == 1)
                posX = sx;
            else
                posX = rect.Width - sx;


            for (int y = 0; y < 3; y++)
            {
                int height;
                if (y == 0 || y == 2)
                    height = sy;
                else
                    height = rect.Height - (sy * GameManager.Settings.UICoef);

                int posY;
                if (y == 0)
                    posY = 0;
                else if (y == 1)
                    posY = sy;
                else
                    posY = rect.Height - sy;

                sb.Draw(tileSet.Texture,
                    new Microsoft.Xna.Framework.Rectangle(posX, posY, width, height),
                    tileSet.GetRectangle(x + (y * 3)),
                    new(this.Color.RGB.R, this.Color.RGB.G, this.Color.RGB.B));
            }
        }

        sb.End();
        texture = rt;
    }

    public override int PositionY
    {
        get => papa.Position.j;
        set
        {
            space.Position.j = value;
            ApplyPosition(papa is null ? Screen : papa.Rectangle);
        }
    }

    public void Clear()
    {
        foreach(UI c in childs)
        {
            c.Active = false;
        }
        this.childs.Clear();
    }

    public Panel(Rectangle space)
    {
        this.space = space;
        ApplySpace(Screen);
    }

    public Panel(Rectangle space, UI parent)
    {
        this.space = space;
        ApplySpace(parent.Rectangle);
    }

    public Panel(TileSet tileSet, Rectangle space)
    {
        this.space = space;
        this.tileSet = tileSet;
        ApplySpace(Screen);
        CreateTexture();
    }

    public Panel(TileSet tileSet, Rectangle space, UI parent)
    {
        this.space = space;
        this.tileSet = tileSet;
        ApplySpace(parent.Rectangle);
        CreateTexture();
    }

    public override void Draw()
    {
        if (_active)
        {
            if (texture != null)
                GameManager.Instance.SpriteBatch.Draw
                 (texture, rect, new(this.Color.RGB.R, this.Color.RGB.G, this.Color.RGB.B));
            foreach (UI element in childs)
                element.Draw();
        }
    }

    public override void Dispose()
    {
        if (texture is not null)
        texture.Dispose();
        if (rt is not null)
            rt.Dispose();
        rt = null;
        texture = null;
        foreach (UI ui in childs)
        {
            ui.Dispose();
        }
    }
}
