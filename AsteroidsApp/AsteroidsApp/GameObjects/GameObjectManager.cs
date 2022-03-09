using AsteroidsApp.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsteroidsApp
{
    //All hantering av gameobjects utom playern görs här.
    class GameObjectManager : DrawableGameComponent
    {
        private Random random = new Random();
        List<LaserShot> shots = new List<LaserShot>();
        List<Meteor> meteors = new List<Meteor>();
        List<Explosion> explosions = new List<Explosion>();

        Texture2D laserTexture, meteorBigTexture, meteorMediumTexture, meteorSmallTexture, explosionTexture;


        public GameObjectManager(Game game) : base(game)
        {

        }

        public override void Initialize()
        {
            //om vi får ett meddelande ska callbackmetoden anropas
            Mediator.Instance.Register<AddLaserShotMessage>(this, AddLaserShotMessageCallback);
            base.Initialize();
        }

        private void AddLaserShotMessageCallback(AddLaserShotMessage message)
        {
            shots.Add(message.LaserShot);
        }

        public void Init()
        {
            meteors.Clear();
            shots.Clear();
            explosions.Clear();
            while (meteors.Count < StatsSingleton.Instance.Level+2)
            {
                var angle = random.Next() * MathHelper.TwoPi;
                var m = new Meteor(MeteorType.Big)
                {
                    Position = new Vector2(Globals.GameArea.Left + (float)random.NextDouble() * Globals.GameArea.Width,
                    Globals.GameArea.Top + (float)random.NextDouble() * Globals.GameArea.Height),
                    Rotation = angle,
                    Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle) * random.Next(20, 60) / 30.0f)
                };

                if (!Globals.RespawnArea.Contains(m.Position))
                    meteors.Add(m);
            }
        }

        protected override void LoadContent()
        {
            laserTexture =        Game.Content.Load<Texture2D>("Laser");
            meteorBigTexture =    Game.Content.Load<Texture2D>("BigAsteroid");
            meteorMediumTexture = Game.Content.Load<Texture2D>("MediumAsteroid");
            meteorSmallTexture =  Game.Content.Load<Texture2D>("SmallAsteroid");
            explosionTexture =    Game.Content.Load<Texture2D>("Explosion");

            base.LoadContent();
        }

        public void CheckPlayerCollision(Player playerComponent)
        {
            var collidingMeteor = meteors.FirstOrDefault(m => m.CollidesWith(playerComponent));

            if(collidingMeteor != null)
            {
                playerComponent.Lives--;
                meteors.Remove(collidingMeteor);
                explosions.Add(new Explosion()
                {
                    Position = collidingMeteor.Position,
                    Rotation = collidingMeteor.Rotation,
                    Scale = collidingMeteor.ExplosionScale
                });

                if (playerComponent.Lives <= 0)
                    Mediator.Instance.Send(new GameStateChangedMessage() { NewState = GameState.Dead });
            }
        }

        public override void Update(GameTime gameTime)
        {
            //gör att man kan se skotten röra på sig och kollar även om de träffar en meteor
            foreach (LaserShot l in shots)
            {
                l.Update(gameTime);
                Meteor meteor = meteors.FirstOrDefault(m => m.CollidesWith(l));

                if (meteor != null)
                {
                    meteors.Remove(meteor);
                    meteors.AddRange(Meteor.BreakMeteor(meteor));
                    explosions.Add(new Explosion()
                    {
                        Position = meteor.Position,
                        Scale = meteor.ExplosionScale
                    }); ;
                    l.IsDead = true;
                }
            }

            //if it's dead or outside game area, remove it
            shots.RemoveAll(l => l.IsDead || !Globals.GameArea.Contains(l.Position));
            explosions.RemoveAll(e => e.IsDead);

            #region meteors
            foreach (Meteor m in meteors)
                m.Update(gameTime);
            #endregion

            #region explosions
            foreach (Explosion e in explosions)
                e.Update(gameTime);
            #endregion

            #region won

            if (meteors.Count == 0)
                Mediator.Instance.Send(new GameStateChangedMessage() { NewState = GameState.Win });

            #endregion

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //draws every shot int the shots list
            foreach (LaserShot l in shots)
            {
                spriteBatch.Draw(laserTexture, l.Position, null, Color.White, l.Rotation, new Vector2(laserTexture.Width / 2, laserTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }

            //draws every meteor in the meteor list
            foreach (Meteor m in meteors)
            {
                Texture2D meteorTexture = meteorSmallTexture;

                switch (m.Type)
                {
                    case MeteorType.Big:
                        meteorTexture = meteorBigTexture;
                        break;
                    case MeteorType.Medium:
                        meteorTexture = meteorMediumTexture;
                        break;
                }

                foreach (Explosion e in explosions)
                {
                    spriteBatch.Draw(explosionTexture, e.Position, null, e.Color, e.Rotation, new Vector2(explosionTexture.Width / 2, explosionTexture.Height / 2), e.Scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(meteorTexture, m.Position, null, Color.White, m.Rotation, new Vector2(meteorTexture.Width / 2, meteorTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            }

        }
    }
}
