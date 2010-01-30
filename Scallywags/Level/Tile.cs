using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Scallywags
{
    class Tile : Object
    {
        public Tile(Vector2 Location, Texture2D tex) : base(Location, tex)
        {

        }

        public override bool Update(float elapsedTime)
        {
			return true;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Microsoft.Xna.Framework.GameTime gameTime)
        {
            sb.Draw(myTexture, GlobalHelpers.GetScreenCoords(Position), Color.White);
        }

        public override bool onCollision(Object collisionObject)
        {
            return true;
        }

        public override void Kill()
        {
            
        }

    }
}
