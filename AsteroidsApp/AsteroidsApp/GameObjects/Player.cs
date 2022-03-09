using AsteroidsApp.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidsApp
{
    /// <summary>
    /// This class represents the player and is a game object. DarawableGameComponent is a XNA class.
    /// </summary>
    class Player : DrawableGameComponent, IGameObject
    {
        public bool IsDead { get; set; }
        public int Lives { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }

        public bool CanShoot { get { return reloadTimer == 0; } }
        private int reloadTimer = 0;

        private Texture2D playerTexture, lifeTexture;
        private readonly int _maxVelocity = 4;
        private readonly int _velocityLimit = 16;

        //Sends the parameter to the DrawableGameComponent class
        public Player(Game game) : base(game)
        {
            Init();
            Radius = 21;
        }

        public void Init()
        {
            Velocity = Vector2.Zero;
            Position = new Vector2(Globals.ScreenWidth / 2, Globals.ScreenHeight / 2);
            Lives = 3;
        }
        protected override void LoadContent()
        {
            playerTexture = Game.Content.Load<Texture2D>("Player");
            lifeTexture = Game.Content.Load<Texture2D>("Life");
            base.LoadContent();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture,
                Position,
                null,
                Color.White,
                // Rotation + PiOver2 makes the rotation correct according to the default coordinate system of this xna framework.
                Rotation + MathHelper.PiOver2,
                //Sets the center of the player.
                new Vector2(playerTexture.Width / 2, playerTexture.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f);

            for (int i=0; i<Lives; i++)
            {
                spriteBatch.Draw(lifeTexture, new Vector2(40 + i * 20, 40), Color.White);
            }
        }

        /// <summary>
        /// If i have a velocity that point toward a direction, my position will accelerate towards that direction.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Position += Velocity;

            KeyboardState state = Keyboard.GetState();

            #region keyboard control
            if (state.IsKeyDown(Keys.Up))
                Accelerate();
            if (state.IsKeyDown(Keys.Left))
                Rotation -= 0.05f;
            else if (state.IsKeyDown(Keys.Right))
                Rotation += 0.05f;

            //Här skicakr vi ett meddelande till någon som vill lyssna på det.
            if (state.IsKeyDown(Keys.Space) && CanShoot)
                Mediator.Instance.Send(new AddLaserShotMessage() { LaserShot = Shoot() });

            #endregion


            if (reloadTimer > 0)
                reloadTimer--;

            #region Player inside bounds
            //Makes sure that the player is inside the game bounds
            if (Position.X < Globals.GameArea.Left)
                Position = new Vector2(Globals.GameArea.Right, Position.Y);
            if (Position.X > Globals.GameArea.Right)
                Position = new Vector2(Globals.GameArea.Left, Position.Y);
            if (Position.Y < Globals.GameArea.Top)
                Position = new Vector2(Position.X, Globals.GameArea.Bottom);
            if (Position.Y > Globals.GameArea.Bottom)
                Position = new Vector2(Position.X, Globals.GameArea.Top);
            #endregion

            base.Update(gameTime);
        }

        public void Accelerate()
        {
            Velocity += new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * 0.10f;

            if (Velocity.LengthSquared() > _velocityLimit)
                Velocity = Vector2.Normalize(Velocity) * _maxVelocity;

        }


        public LaserShot Shoot()
        {
            if (!CanShoot)
                return null;

            //creates a delay so player can't sprayt
            reloadTimer = 15;

            return new LaserShot()
            {
                //try making it come from the nose of the player
                Position = Position,
                Velocity = Velocity + 10f * new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation))
            };
        }
    }
}
