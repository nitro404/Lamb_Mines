using System;
using System.Collections;
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
    class Mine:AnimatedObject
    {
        float time;

		public Mine(Vector2 location,List<Animation> animations, Texture2D aTexture)
			: base(location, animations, aTexture)
		{
            time = 0;
		}

        /// <summary>
        /// This is the trigger function that is called when there is a collision on this object.
        /// </summary>
        /// <returns>Returns FALSE only if this object needs to be destroyed.</returns>
        public override Object[] onCollision(Object collisionObject,Texture2D[] textureList)
        {
			if (time <= 0)
			{
				if (collisionObject.GetType() == typeof(Mine))
				{
					return null;
				}
				if (String.Compare(collisionObject.GetType().FullName, "LambMines.Sheep") == 0 || String.Compare(collisionObject.GetType().FullName, "LambMines.Player") == 0)
				{

					//TODO: More collision logic must be applied here.
					collisionObject.Kill();//Kill the other object because this is a mine
					//Kill();
					Kill();//This object will explode when anything collides with it.
					parent.theOffset.setExplosion(true);
					List<Texture2D> bloodSplatters = new List<Texture2D>();
					for (int i = 22; i < 34; i++)
					{
						bloodSplatters.Add(textureList[i]);
					}

					//create the explosion animation
					List<Animation> animList = new List<Animation>();
					Animation anim = new Animation(textureList[19], 0.1f, false, new Vector2(220, 280), 0);
					animList.Add(anim);
					Explosion tempExp = new Explosion(new Vector2(Position.X - 48 * 5 - 5, Position.Y - 48 * 4 + 5), animList, textureList[19]);
					tempExp.setExplosionType(ExplosionType.EX_POP);
					Error.Trace("Flame Pop explotion at location X: " + Position.X + " Y: " + Position.Y);

					List<Animation> animList2 = new List<Animation>();
					Animation anim2 = new Animation(textureList[19], 0.1f, false, new Vector2(220, 280), 0);
					animList2.Add(anim2);
					Explosion tempExp2 = new Explosion(new Vector2(Position.X - 48 * 5 - 5, Position.Y - 48 * 4 + 5), animList2, textureList[19]);
					tempExp2.setExplosionType(ExplosionType.EX_FLAME);
					Error.Trace("Flame Pop explotion at location X: " + Position.X + " Y: " + Position.Y);

					Object[] objList = new Object[2];
					objList[0] = tempExp;
					objList[1] = tempExp2;

					return objList;
				}
			}
            return null;
        }

        public override bool Update(float elapsedTime, ArrayList collisionList)
        {
            if (time > 0)
            {
                time -= elapsedTime;
                AnimationPlay.Paused = true;
            }
            else
            {
                AnimationPlay.Paused = false;
            }
			return isAlive;
        }
        public override void Draw(SpriteBatch spriteThing, GameTime gameTime, Vector2 Offset)
        {
            base.Draw(spriteThing, gameTime, Offset);
			if (shadowObject != null)
                spriteThing.Draw(shadowTexture, GlobalHelpers.GetScreenCoords(Centre) + Offset, Color.White);

        }
        public void DisarmMine(float duration)
        {
            time = duration;
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
