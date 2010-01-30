using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class
     *  @brief  particle like spark used by the cannon fuse and gold indicator
     */
    public class Spark
    {
        public float fLife = 0;
        public Vector2 vVelocity = Vector2.Zero;
        public Vector2 vPosition = Vector2.Zero;
        public Color color = Color.Yellow;
    }
}
