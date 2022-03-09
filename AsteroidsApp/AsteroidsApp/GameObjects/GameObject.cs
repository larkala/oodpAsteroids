using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidsApp
{
    public abstract class GameObject : IGameObject
    {
        public bool IsDead { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }

        /// <summary>
        /// Enkel beräkning beroende på skillnaden i positionsläge. Vi tänker oss alla gameobjects som cirklar (drf har vi en radius på alla). Om vi tar skillnaden i position kommer vi få en vektor.
        /// Vi tar längden på den. Den ena vektorn minus den andra vektorn kommer bli en vektor som besriver skillnaden från den ena positionen på den andra. Vi tar längden på den i kvadrat. Om den är
        /// mindre än radien för det ena objektet plus area för andra objektet multiplicerat med sig själv. ifall längden mellan dem är mindre än deras sammanlagda radie så innebär det att en kollision påbörjats.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWith(IGameObject other)
        {
            return (this.Position - other.Position).LengthSquared() < Math.Pow((Radius + other.Radius), 2);
        }
    }
}
