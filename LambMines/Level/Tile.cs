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

namespace LambMines
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

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb, Microsoft.Xna.Framework.GameTime gameTime, Vector2 Offset)
        {
            sb.Draw(myTexture, GlobalHelpers.getTilePositionOffset(GlobalHelpers.GetScreenCoords(Position + Offset), myTexture.Height), Color.White);
        }

		public override Object onCollision(Object collisionObject, Texture2D[] textureList)
        {
            return null;
        }

        public override void Kill()
        {
            
        }

    }
}
