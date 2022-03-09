using Microsoft.Xna.Framework;

namespace AsteroidsApp
{
    class Explosion : GameObject
    {
        public float Scale { get; set; }
        private int timer = 30;

        //Makes the explosion fade
        public Color Color {
            get { return new Color(timer * 8, timer * 8, timer * 8); } 
        }

        public void Update(GameTime gameTime)
        {
            if (timer > 0)
                timer--;
            else
                IsDead = true;

            Rotation += 0.02f;
            if (Rotation > MathHelper.TwoPi)
                Rotation = 0;
        }

    }
}
