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
        public override Object onCollision(Object collisionObject,Texture2D[] textureList)
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

                    Vector2 dir = collisionObject.Position - this.Position;
                    int splatter = (int)GetAnimationDirection(dir) / 2;

                    Vector2 LocationHack = collisionObject.Position;
                    switch (splatter)
                    {
                        case 0:
                            //LocationHack -= new Vector2(110, 70);
                            LocationHack -= new Vector2(10, 0);
                            break;
                        case 1:
                            //LocationHack -= new Vector2(80, 45);
                            //OK
                            LocationHack += new Vector2(20, 45);
                            break;
                        case 2:
                            //LocationHack -= new Vector2(70, 45);
                            //OK
                            LocationHack += new Vector2(0, 50);
                            break;
                        case 3:
                            //LocationHack -= new Vector2(175, 70);
                            LocationHack += new Vector2(-45, 10);
                            break;
                    }

                    Random rand = new Random(DateTime.Now.Millisecond);

                    int randCheck = (splatter * 3) + rand.Next(0, 2);
                    Error.Trace(randCheck.ToString());
                    Tile toAdd = new Tile(LocationHack, bloodSplatters[randCheck]);
                    //Tile toAdd = new Tile(LocationHack, textureList[0]);
                    parent.Spawn(Level.RenderLevel.RL_OBJECTS, toAdd);


                    List<Animation> animList = new List<Animation>();
                    Animation anim = new Animation(textureList[19], 0.1f, false, new Vector2(220, 280), 0);
                    animList.Add(anim);
                    Explosion tempExp = new Explosion(new Vector2(Position.X - 48 * 5 - 5, Position.Y - 48 * 4 + 5), animList, textureList[19]);

                    return tempExp;//This object will be destroyed.
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
