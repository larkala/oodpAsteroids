using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidsApp
{
    public class LaserShot : GameObject
    {
        public LaserShot()
        {
            Radius = 2; 
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity;
            Rotation += 0.08f;

            //if rotation is larger than 360, make it 0
            if (Rotation > MathHelper.TwoPi)
                Rotation = 0;
        }
    }
}
