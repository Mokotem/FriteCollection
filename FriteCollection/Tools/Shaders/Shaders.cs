using System;
using FriteCollection.Graphics;
using FriteCollection.Scripting;
using Microsoft.Xna.Framework.Graphics;

namespace FriteCollection.Tools.Shaders
{
    public abstract class Shader
    {
        private Effect _effect;
        public Effect GetEffect => _effect;

        public Shader()
        {
            _effect = FriteModel.MonoGame.instance.Content.Load<Effect>("Shaders/" + this.GetType().Name);
            FriteModel.MonoGame.instance.CurrentShader = this;
        }

        public bool Active
        {
            set
            {
                if (value)
                    FriteModel.MonoGame.instance.CurrentShader = this;
                else
                    FriteModel.MonoGame.instance.CurrentShader = null;
            }
        }

        public virtual void ApllySettings() { }

        public float Volume
        {
            set
            {
                _effect.Parameters["volume"].SetValue(MathF.Max(MathF.Min(value, 1), 0));
            }
        }

        public class Circle : Shader
        {
            public Circle() : base()
            {
                _effect.Parameters["ratio"].SetValue((float)GetSeetings.Settings.GameFixeHeight / GetSeetings.Settings.GameFixeWidth);
            }

            public float Value
            {
                set
                {
                    _effect.Parameters["value"].SetValue(MathF.Max(MathF.Min(1, value), 0));
                }
            }
        }

        public class Bulge : Shader
        {
            public Bulge() : base()
            {
                _effect = FriteModel.MonoGame.instance.Content.Load<Effect>("Shaders/Bulge");
            }

            public float Value
            {
                set
                {
                    _effect.Parameters["value"].SetValue(value);
                }
            }
        }

        public class Glitch : Shader
        {
            public override void ApllySettings()
            {
                switch (Random.Random.Int(0, 2))
                {
                    case 0:
                        _effect.Parameters["fact1"].SetValue(-1);
                        _effect.Parameters["fact2"].SetValue(0);
                        _effect.Parameters["fact3"].SetValue(1);
                        break;

                    case 1:
                        _effect.Parameters["fact1"].SetValue(0);
                        _effect.Parameters["fact2"].SetValue(1);
                        _effect.Parameters["fact3"].SetValue(-1);
                        break;

                    case 2:
                        _effect.Parameters["fact1"].SetValue(1);
                        _effect.Parameters["fact2"].SetValue(-1);
                        _effect.Parameters["fact3"].SetValue(0);
                        break;
                }
            }

            public Entity.Vector Offset
            {
                set
                {
                    _effect.Parameters["ofx"].SetValue(value.x);
                    _effect.Parameters["ofy"].SetValue(value.y);
                }
            }
        }

        public class Wave : Shader
        {
            private float _offset = 0f;

            public override void ApllySettings()
            {
                _offset += Time.Delta * 10;
                _effect.Parameters["offset"].SetValue(_offset);
            }

            public float Strength
            {
                set
                {
                    _effect.Parameters["strength"].SetValue(value);
                }
            }

            public float Speed
            {
                set
                {
                    _effect.Parameters["speed"].SetValue(value);
                }
            }

            public float Length
            {
                set
                {
                    _effect.Parameters["length"].SetValue(value);
                }
            }
        }

        public class ColorFilter : Shader
        {
            public ColorFilter() : base()
            {
                _effect.Parameters["mr"].SetValue(1f);
                _effect.Parameters["mg"].SetValue(1f);
                _effect.Parameters["mb"].SetValue(1f);
            }

            public Color Color
            {
                set
                {
                    _effect.Parameters["mr"].SetValue(value.RGB.R);
                    _effect.Parameters["mg"].SetValue(value.RGB.G);
                    _effect.Parameters["mb"].SetValue(value.RGB.B);
                }
            }
        }
    }
}
