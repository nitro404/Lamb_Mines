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
	class Clutter : Object
	{

		public Clutter(int[] location, Texture2D aTexture)
			: base(location, aTexture)
		{

		}
		public Clutter(Vector2 location, Texture2D aTexture)
			: base(location, aTexture)
		{

		}

		/// <summary>
		/// This is the trigger function that is called when there is a collision on this object.
		/// </summary>
		/// <returns>Returns FALSE only if this object needs to be destroyed.</returns>
		public override bool onCollision(Object collisionObject)
		{
			//TODO: More collision logic must be applied here.
			//collisionObject.Kill();//Kill the other object because this is a mine
			//Kill();//This object will explode when anything collides with it.

			collisionObject.Stopped = true;

			return false;//This object will be destroyed.
		}

		public override void Update(float elapsedTime)
		{

		}
		public override void Draw(SpriteBatch spriteThing, GameTime gameTime)
		{

			Vector2 position = new Vector2((float)Position.X * Settings.SCREEN_TILE_MULTIPLIER_X + 3, (float)Position.Y * Settings.SCREEN_TILE_MULTIPLIER_Y + 3);
			position = GlobalHelpers.GetScreenCoords(position);
			spriteThing.Draw(myTexture, position, Color.White);
		}
		public override void Kill()
		{
			KillMe();
		}
	}
}
