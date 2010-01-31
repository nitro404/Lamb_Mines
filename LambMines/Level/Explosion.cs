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
	class Explosion:AnimatedObject
	{
		private ExplosionType expType = ExplosionType.EX_NONE;

		public Explosion(Vector2 Location, List<Animation> animationList, Texture2D tex)
            : base(Location, animationList, tex)
        {

        }

        public override bool Update(float elapsedTime, ArrayList collisionList)
		{

			AnimationPlay.PlayAnimation(Animations[0]);

            if (!AnimationPlay.IsPlaying)
            {
                Kill();
            }

            base.Update(elapsedTime, collisionList);

			if (AnimationPlay.IsPlaying)
				return true;
			return false;

		}

		public override void Draw(SpriteBatch sb, GameTime gameTime, Vector2 Offset)
        {
            base.Draw(sb, gameTime, Offset);
        }

		public override Object[] onCollision(Object collisionObject, Texture2D[] textureList)
		{
			//return base.onCollision(collisionObject, textureList);

			if (String.Compare(collisionObject.GetType().FullName, "LambMines.Sheep") == 0 || String.Compare(collisionObject.GetType().FullName, "LambMines.Player") == 0)
			{
				if (expType == ExplosionType.EX_POP )
				{
					collisionObject.Kill();//Kill the other object because this is a mine

					List<Texture2D> bloodSplatters = new List<Texture2D>();
					for (int i = 22; i < 34; i++)
					{
						bloodSplatters.Add(textureList[i]);
					}

					Vector2 dir = collisionObject.Position - this.Position;
					//int splatter = (int)GetAnimationDirection(dir) / 2;
					Vector2 centerOfExp = new Vector2(Position.X + 245, Position.Y + 191);

                    //Error.Trace("Explosion location X: " + Position.X + " Y: " + Position.Y);
                    //Error.Trace("Explosion Center X: " + centerOfExp.X + " Y: " + centerOfExp.Y);

					double angle = getAngleTo(centerOfExp, collisionObject.Centre);
                    //Error.Trace("Angle is: " + angle.ToString());
					//angle += 45;
					//if (angle > 360) angle -= 360;
					int splatter = (int)(((angle + 45) * 4 / 360) % 4);

					Vector2 LocationHack = collisionObject.Position;
					switch (splatter)
					{
						case 0:
							//LocationHack -= new Vector2(110, 70);
							LocationHack += new Vector2(0, 45);
							break;
						case 1:
							//LocationHack -= new Vector2(80, 45);
							//OK
							LocationHack += new Vector2(0, 65);
							break;
						case 2:
							//LocationHack -= new Vector2(70, 45);
							//OK
							LocationHack += new Vector2(-22.5f, 0.0f);
							break;
						case 3:
							//LocationHack -= new Vector2(175, 70);
							LocationHack += new Vector2(-22.5f, 0.0f);
							break;
					}

					Random rand = new Random(DateTime.Now.Millisecond);

					int randCheck = (splatter * 3) + rand.Next(0, 2);
                    //Error.Trace(randCheck.ToString());
					Tile toAdd = new Tile(LocationHack, bloodSplatters[randCheck]);
					//Tile toAdd = new Tile(LocationHack, textureList[0]);
					parent.Spawn(Level.RenderLevel.RL_OBJECTS, toAdd);

				}
				else if(expType == ExplosionType.EX_FLAME)
				{
					if (!collisionObject.isAlive) { return null; }
					collisionObject.Kill();//Kill the otehr player because this is a mine
					//now spawn a firey sheep in it's place.
					//create the explosion animation

					List<Animation> animList = new List<Animation>();
					Animation anim = new Animation(textureList[35], 0.3f, false, new Vector2(35, 35), 0);
					animList.Add(anim);
					Explosion tempExp = new Explosion(new Vector2(collisionObject.Position.X , collisionObject.Position.Y), animList, textureList[35]);
					//Error.Trace("Spawning flame at location X: " + collisionObject.Position.X + " Y: " + collisionObject.Position.Y);
					tempExp.setExplosionType(ExplosionType.EX_NONE);
					
					return new Object[] {tempExp};
					 
					return null;
				}

			}
			return null;
		}


		public override void Kill()
		{
			KillMe();
		}
		public override string WhatAmI()
		{
			return "Explosion";
		}
		public override void setExplosionType(ExplosionType explosionType)
		{
			expType = explosionType;
		}
		public override Object.ExplosionType getExplosionType()
		{
			return expType;
		}
		public override Vector2 Centre
		{
			get
			{
				return new Vector2(Position.X + 280, Position.Y + 162);
			}
		}
	}
}
