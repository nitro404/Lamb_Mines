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
    class Mine:Object
    {

        public Mine(int[] location, Texture2D aTexture)
            : base(location, aTexture)
        {

        }
		public Mine(Vector2 location, Texture2D aTexture)
			: base(location, aTexture)
		{

		}

        /// <summary>
        /// This is the trigger function that is called when there is a collision on this object.
        /// </summary>
        /// <returns>Returns FALSE only if this object needs to be destroyed.</returns>
        public override Object onCollision(Object collisionObject,Texture2D[] textureList)
        {
			if (collisionObject.GetType() == typeof(Mine))
			{
				return null;
			}
            //TODO: More collision logic must be applied here.
            collisionObject.Kill();//Kill the other object because this is a mine
			Kill();//This object will explode when anything collides with it.

			List<Animation> animList = new List<Animation>();
			Animation anim = new Animation(textureList[19], 0.05f, false, new Vector2(220, 280), 0);
			animList.Add(anim);
			Explosion tempExp = new Explosion(new Vector2(Position.X-48*5+5,Position.Y-48*4+5), animList, textureList[19]);

			return tempExp;//This object will be destroyed.
        }

        public override bool Update(float elapsedTime)
        {
			Vector2 hi = GlobalHelpers.GetScreenCoords(new Vector2(320.0f, 240.0f));
			hi = GlobalHelpers.GetScreenCoords(new Vector2(70.0f, 110.0f));
			hi = GlobalHelpers.GetScreenCoords(new Vector2(10.0f, 140.0f));
			hi = GlobalHelpers.GetScreenCoords(new Vector2(316.0f, 5.0f));
			hi = GlobalHelpers.GetScreenCoords(new Vector2(55.0f, 55.0f));
			hi = GlobalHelpers.GetScreenCoords(new Vector2(32.0f, 40.0f));


			return isAlive;
        }
        public override void Draw(SpriteBatch spriteThing, GameTime gameTime, Vector2 Offset)
        {
            spriteThing.Draw(myTexture, GlobalHelpers.GetScreenCoords(Position + Offset), Color.White);
			if (shadowTexture != null)
				spriteThing.Draw(shadowTexture, GlobalHelpers.GetScreenCoords(Position) + Offset, Color.White);

        }
        public override void Kill()
        {
            KillMe();
        }
		public override string WhatAmI()
		{
			return "Mine";
		}
    }
}
