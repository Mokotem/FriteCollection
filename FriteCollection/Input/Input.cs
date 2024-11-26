using FriteCollection.Entity;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;

namespace FriteCollection.Input
{
    public interface IIsKeyDown<T>
    {
        /// <summary>
        /// Checks if a key is pressed.
        /// </summary>
        /// <param name="key">the key you want to check</param>
        /// <returns>true if the key is hold.</returns>
        /// <exception cref="Exception">The key name must exist.</exception>
        public static bool IsKeyDown(string key)
        {
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                if (info.Name == key) return info.GetValue(null).ToString() == "True";
            }

            throw new Exception("The key `" + key + "` does not exist in `" + typeof(T).Name + "`.");
        }

        /// <summary>
        /// Gets the name of the current pressed key. Does not apply if multiple keys pressed.
        /// </summary>
        /// <returns>The name of the key. "None" if no key pressed or more than one.</returns>
        public static string CurrentPressedKey
        {
            get
            {
                string key = "None";
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    if (info.GetValue(null).ToString() == "True")
                    {
                        if (key == "None")
                        {
                            key = info.Name;
                        }
                        else
                        {
                            return "None";
                        }
                    }
                }

                return key;
            }
        }
    }

    /// <summary>
    /// Données sur les touches pressées.
    /// </summary>
    public abstract class Input
    {
        public abstract class KeyBoard : IIsKeyDown<KeyBoard>
        {
            public static bool A => Keyboard.GetState().IsKeyDown(Keys.A);
            public static bool B => Keyboard.GetState().IsKeyDown(Keys.B);
            public static bool C => Keyboard.GetState().IsKeyDown(Keys.C);
            public static bool D => Keyboard.GetState().IsKeyDown(Keys.D);
            public static bool E => Keyboard.GetState().IsKeyDown(Keys.E);
            public static bool F => Keyboard.GetState().IsKeyDown(Keys.F);
            public static bool G => Keyboard.GetState().IsKeyDown(Keys.G);
            public static bool H => Keyboard.GetState().IsKeyDown(Keys.H);
            public static bool I => Keyboard.GetState().IsKeyDown(Keys.I);
            public static bool J => Keyboard.GetState().IsKeyDown(Keys.J);
            public static bool K => Keyboard.GetState().IsKeyDown(Keys.K);
            public static bool L => Keyboard.GetState().IsKeyDown(Keys.L);
            public static bool M => Keyboard.GetState().IsKeyDown(Keys.M);
            public static bool N => Keyboard.GetState().IsKeyDown(Keys.N);
            public static bool O => Keyboard.GetState().IsKeyDown(Keys.O);
            public static bool P => Keyboard.GetState().IsKeyDown(Keys.P);
            public static bool Q => Keyboard.GetState().IsKeyDown(Keys.Q);
            public static bool R => Keyboard.GetState().IsKeyDown(Keys.R);
            public static bool S => Keyboard.GetState().IsKeyDown(Keys.S);
            public static bool T => Keyboard.GetState().IsKeyDown(Keys.T);
            public static bool U => Keyboard.GetState().IsKeyDown(Keys.U);
            public static bool V => Keyboard.GetState().IsKeyDown(Keys.V);
            public static bool W => Keyboard.GetState().IsKeyDown(Keys.W);
            public static bool X => Keyboard.GetState().IsKeyDown(Keys.X);
            public static bool Y => Keyboard.GetState().IsKeyDown(Keys.Y);
            public static bool Z => Keyboard.GetState().IsKeyDown(Keys.Z);

            public static bool[] Number
            {
                get
                {
                    bool[] numList = new bool[10];
                    for (int i = 0; i < 10; i++)
                    {
                        numList[i] = Keyboard.GetState().IsKeyDown((Keys)48 + i);
                    }
                    return numList;
                }
            }


            public static bool ShiftLeft => Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            public static bool ShiftRight => Keyboard.GetState().IsKeyDown(Keys.RightShift);
            public static bool CtrlLeft => Keyboard.GetState().IsKeyDown(Keys.LeftControl);
            public static bool CtrlRight => Keyboard.GetState().IsKeyDown(Keys.RightControl);
            public static bool AltLeft => Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
            public static bool AltRight => Keyboard.GetState().IsKeyDown(Keys.RightAlt);
            public static bool Tab => Keyboard.GetState().IsKeyDown(Keys.Tab);
            public static bool Enter => Keyboard.GetState().IsKeyDown(Keys.Enter);
            public static bool Delete => Keyboard.GetState().IsKeyDown(Keys.Delete);
            public static bool Escape => Keyboard.GetState().IsKeyDown(Keys.Escape);
            public static bool Space => Keyboard.GetState().IsKeyDown(Keys.Space);

            public static bool Up => Keyboard.GetState().IsKeyDown(Keys.Up);
            public static bool Down => Keyboard.GetState().IsKeyDown(Keys.Down);
            public static bool Left => Keyboard.GetState().IsKeyDown(Keys.Left);
            public static bool Right => Keyboard.GetState().IsKeyDown(Keys.Right);

            public static bool F1 => Keyboard.GetState().IsKeyDown(Keys.F1);
            public static bool F2 => Keyboard.GetState().IsKeyDown(Keys.F2);
            public static bool F3 => Keyboard.GetState().IsKeyDown(Keys.F3);
            public static bool F4 => Keyboard.GetState().IsKeyDown(Keys.F4);
            public static bool F5 => Keyboard.GetState().IsKeyDown(Keys.F5);
            public static bool F6 => Keyboard.GetState().IsKeyDown(Keys.F6);
            public static bool F7 => Keyboard.GetState().IsKeyDown(Keys.F7);
            public static bool F8 => Keyboard.GetState().IsKeyDown(Keys.F8);
            public static bool F9 => Keyboard.GetState().IsKeyDown(Keys.F9);
            public static bool F10 => Keyboard.GetState().IsKeyDown(Keys.F10);
            public static bool F11 => Keyboard.GetState().IsKeyDown(Keys.F11);
            public static bool F12 => Keyboard.GetState().IsKeyDown(Keys.F12);
            public static bool F13 => Keyboard.GetState().IsKeyDown(Keys.F13);
            public static bool F14 => Keyboard.GetState().IsKeyDown(Keys.F14);
            public static bool F15 => Keyboard.GetState().IsKeyDown(Keys.F15);
            public static bool F16 => Keyboard.GetState().IsKeyDown(Keys.F16);
            public static bool F17 => Keyboard.GetState().IsKeyDown(Keys.F17);
            public static bool F18 => Keyboard.GetState().IsKeyDown(Keys.F18);
            public static bool F19 => Keyboard.GetState().IsKeyDown(Keys.F19);
            public static bool F20 => Keyboard.GetState().IsKeyDown(Keys.F20);
            public static bool F21 => Keyboard.GetState().IsKeyDown(Keys.F21);
            public static bool F22 => Keyboard.GetState().IsKeyDown(Keys.F22);
            public static bool F23 => Keyboard.GetState().IsKeyDown(Keys.F23);
            public static bool F24 => Keyboard.GetState().IsKeyDown(Keys.F24);
        }

        public abstract class KeyPad : IIsKeyDown<KeyPad>
        {
            public static bool[] Number
            {
                get
                {
                    bool[] numList = new bool[10];
                    for (int i = 0; i < 10; i++)
                    {
                        numList[i] = Keyboard.GetState().IsKeyDown((Keys)96 + i);
                    }
                    return numList;
                }
            }

            public static bool Divide => Keyboard.GetState().IsKeyDown(Keys.Divide);
            public static bool Multiply => Keyboard.GetState().IsKeyDown(Keys.Multiply);
            public static bool Add => Keyboard.GetState().IsKeyDown(Keys.Add);
            public static bool Subtract => Keyboard.GetState().IsKeyDown(Keys.Subtract);
            public static bool NumLock => Keyboard.GetState().IsKeyDown(Keys.NumLock);
            public static bool Decimal => Keyboard.GetState().IsKeyDown(Keys.Decimal);
        }


        public abstract class Mouse : IIsKeyDown<Mouse>
        {
            public static bool Hide { set { FriteModel.MonoGame.instance.IsMouseVisible = !value; } }
            public static bool Left { get { return Microsoft.Xna.Framework.Input.Mouse.GetState().LeftButton == ButtonState.Pressed; } }
            public static bool Right { get { return Microsoft.Xna.Framework.Input.Mouse.GetState().RightButton == ButtonState.Pressed; } }
            public static bool Middle { get { return Microsoft.Xna.Framework.Input.Mouse.GetState().MiddleButton == ButtonState.Pressed; } }
            public static FriteCollection.Entity.Vector Position(Bounds bound = Bounds.Center)
            {
                return new FriteCollection.Entity.Vector
                (
                    ((Microsoft.Xna.Framework.Input.Mouse.GetState().X - FriteModel.MonoGame.instance.targetGameRectangle.Location.X) / FriteModel.MonoGame.instance.aspectRatio) - FriteModel.MonoGame.instance.screenBounds[(int)bound].x,
                    ((-Microsoft.Xna.Framework.Input.Mouse.GetState().Y + FriteModel.MonoGame.instance.targetGameRectangle.Location.Y) / FriteModel.MonoGame.instance.aspectRatio) + FriteModel.MonoGame.instance.screenBounds[(int)bound].y
                );
            }

            public static FriteCollection.Entity.Vector Scroll { get { return new FriteCollection.Entity.Vector(Microsoft.Xna.Framework.Input.Mouse.GetState().HorizontalScrollWheelValue, Microsoft.Xna.Framework.Input.Mouse.GetState().ScrollWheelValue); } }
        }
    }
}