using FriteCollection.Entity;
using FriteCollection.Scripting;
using FriteCollection.Input;
using FriteCollection.Tools.Shaders;

namespace GameName
{
    public class FriteScript1 : Script
    {
        public FriteScript1() : base(Scenes.Scene1) { }

        Object player = new Object()
        {
            Space = new Space()
            {
                Scale = new Vector(10, 10)
            }
        };
        HitBox.Rectangle phit;

        Shader.Wave shader = new();

        public override void Start()
        {
            shader.Length = 20;
            shader.Strength = 0.02f;
            shader.Speed = 1f;
            shader.Volume = 1f;

            phit = new HitBox.Rectangle(player.Space);
            new HitBox.Rectangle(new Space()
            {
                Scale = new Vector(40, 50)
            });
        }

        public override void Update()
        {
            player.Space.Position = Input.Mouse.Position();
            if (phit.Check(out HitBox.Sides side, out HitBox collider, out ushort count))
            {
                GameManager.Print(side);
            }

            player.Space.rotation = Time.Timer * 90;
        }

        public override void Draw()
        {
            player.Draw();
        }
    }
}