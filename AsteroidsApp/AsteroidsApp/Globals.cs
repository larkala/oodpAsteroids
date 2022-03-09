using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidsApp
{
    class Globals
    {
        public static int ScreenWidth = 1680;
        public static int ScreenHeight = 980;

        public static Rectangle GameArea
        {
            get
            {
                return new Rectangle(-50, -50, ScreenWidth + 100, ScreenHeight + 100);
            }
        }
        
        /// <summary>
        /// This is the area where the player will respawn and thereby where asteroids wont spawn
        /// </summary>
        public static Rectangle RespawnArea
        {
            get
            {
                return new Rectangle((int)ScreenCenter.X - 400, (int)ScreenCenter.Y - 400, 800, 800);
            }
        }

        public static Vector2 ScreenCenter
        {
            get
            {
                return new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            }
        }

    }
}
