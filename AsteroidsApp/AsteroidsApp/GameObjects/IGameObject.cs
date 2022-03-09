using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidsApp
{
    /// <summary>
    /// This Interface will represent every object in the game
    /// </summary>
    public interface IGameObject
    {
        public bool IsDead { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
    }
}
