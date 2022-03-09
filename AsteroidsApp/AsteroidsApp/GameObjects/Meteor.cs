using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidsApp
{
    enum MeteorType
    {
        Big,
        Medium,
        Small
    }

    class Meteor : GameObject
    {
        public MeteorType Type { get; private set; }
        public float ExplosionScale { get; private set; }
        public int PointWorth { get; private set; }

        public Meteor(MeteorType type)
        {
            Type = type;

            switch (Type)
            {
                case MeteorType.Big:
                    Radius = 80;
                    ExplosionScale = 1.0f;
                    PointWorth = 20;
                    break;
                case MeteorType.Medium:
                    Radius = 37;
                    ExplosionScale = 0.5f;
                    PointWorth = 50;
                    break;
                case MeteorType.Small:
                    Radius = 20;
                    ExplosionScale = 0.3f;
                    PointWorth = 200;
                    break;
                default:
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity;

            #region Meteor inside bounds
            if (Position.X < Globals.GameArea.Left)
                Position = new Vector2(Globals.GameArea.Right, Position.Y);
            if (Position.X > Globals.GameArea.Right)
                Position = new Vector2(Globals.GameArea.Left, Position.Y);
            if (Position.Y < Globals.GameArea.Top)
                Position = new Vector2(Position.X, Globals.GameArea.Bottom);
            if (Position.Y > Globals.GameArea.Bottom)
                Position = new Vector2(Position.X, Globals.GameArea.Top);
            #endregion

            Rotation += 0.02f;
            if (Rotation > MathHelper.TwoPi)
                Rotation = 0;
        }
        
        public static IEnumerable<Meteor> BreakMeteor(Meteor meteor)
        {
            //adding the points
            StatsSingleton.Instance.AddPoints(meteor.PointWorth);

            List<Meteor> meteors = new List<Meteor>();

            if (meteor.Type == MeteorType.Small)
                return meteors;

            for(int i=0; i<2; i++)
            {
                //decides which angle the new meteors will drift away in, this specifically drifts away 45 degrees 
                var angle = (float) Math.Atan2(meteor.Velocity.Y, meteor.Velocity.X) - MathHelper.PiOver4 + MathHelper.PiOver4 * i;

                //enums have values, big will return 2 small, medium will return small.
                meteors.Add(new Meteor(meteor.Type + 1)
                {
                    Position = meteor.Position,
                    Rotation = angle,
                    //the speed will become less for each meteor split
                    Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * meteor.Velocity.Length() * 1.4f
                }) ;
            }

            return meteors; 
        }
    }
}
