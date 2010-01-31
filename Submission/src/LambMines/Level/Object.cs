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
    abstract class Object
    {
		public enum ExplosionType{EX_NONE,EX_POP,EX_FLAME};

        //private variables
        private Vector2 position = new Vector2();
        private Level parentLevel;
        public Texture2D myTexture;
		public Object shadowObject = null;
		public Texture2D shadowTexture = null;
        private bool StillAlive = true;
		private bool AmIStopped = false;
		public bool iIsAShadow = false;

        //properties
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
		}
        public bool isAlive
        {
			get { return StillAlive; }
        }

        public Level parent
        {
            get { return parentLevel; }
            set { parentLevel = value; }
        }

		public bool Stopped
		{
			get { return AmIStopped; }
			set { AmIStopped = value; }
		}
		public Vector2 WorldPosition
		{
			get { return GlobalHelpers.GetScreenCoords((new Vector2(position.X * Settings.SCREEN_TILE_MULTIPLIER_X, position.Y * Settings.SCREEN_TILE_MULTIPLIER_Y))); }
		}

        public virtual Vector2 Centre
        {
            get { return new Vector2( position.X + (myTexture.Width/2),position.Y + (myTexture.Height/2) ); }
        }

		//public methods.
        public void KillMe()
        {
			// I'm writing a note here... huge success.
			// It's hard to overstate my... satisfaction.
            // Aperture Science...
            // We do what we must... because we can
            StillAlive = false;
        }

        //These are some default constructors
        public Object(Texture2D aTexture) { myTexture = aTexture; }
        public Object(int[] Location, Texture2D aTexture)
        {
            position.X = (float)Location[0];
            position.Y = (float)Location[1];
            myTexture = aTexture;
        }
        public Object(double[] Location, Texture2D aTexture)
        {
			position.X = (float)Location[0];
			position.Y = (float)Location[1];
            myTexture = aTexture;
        }
        public Object(ArrayList Location, Texture2D aTexture)
        {
            position.X = (float)Location[0];
            position.Y = (float)Location[1];
            myTexture = aTexture;
        }
		public Object(Vector2 Location, Texture2D aTexture)
		{
			position = Location;
			myTexture = aTexture;
		}

        /// <summary>
        /// This is the trigger function that is called when there is a collision on this object. 
        /// Only an object that has been set with an associated onCollisionEvent can be called.
        /// </summary>
        /// <param name="collisionObject">This is a reference to the object that has collided with this event</param>
        /// <returns>Returns FALSE only if this object needs to be destroyed.</returns>
        abstract public Object[] onCollision(Object collisionObject,Texture2D[] textureList);

        /// <summary>
        /// General update loop
        /// </summary>
        abstract public bool Update(float elapsedTime, ArrayList collisionList);

        /// <summary>
        /// General draw loop
        /// </summary>
        abstract public void Draw(SpriteBatch sb, GameTime gameTime, Vector2 Offset);

        /// <summary>
        /// Kill this unit.
        /// </summary>
        abstract public void Kill();

		virtual public string WhatAmI()
		{
			return "Object";
		}
		virtual public void setExplosionType(ExplosionType nothing){

		}
		virtual public ExplosionType getExplosionType()
		{
			return ExplosionType.EX_NONE;
		}
/*
		virtual public void AddShadow(ref Object shadowObj, Texture2D shadowTex)
		{
			shadowObject = shadowObj;
			shadowTexture = shadowTex;
			iIsAShadow = true;
		}
 */
		virtual public void AddShadow(ref Sheep shadowObj, Texture2D shadowTex)
		{
			shadowObject = (Object)shadowObj;
			shadowTexture = shadowTex;
			iIsAShadow = true;
		}

        public int GetAnimationDirection(Vector2 direction)
        {
            double angle = Math.Atan2(direction.Y, direction.X) + (Math.PI * 6.0 / 8.0);
            double fraction = angle * 0.5 / Math.PI;
            fraction += 1.0 / 16.0;
            if (fraction >= 1.0)
            {
                fraction -= 1.0;
            }
            int index = (int)(fraction * 8.0);
            //float angle = (float)Math.Atan2(direction.Y, direction.X) - (float)Math.Atan2(1.0f, 0.0f);
            //return (int)((angle/(Math.PI/4)) + 8) % 8;
            return index;
        }

		public double getAngleTo(Vector2 origion, Vector2 yourPos)
		{
			//use the sin law to get the angle between thes two vectors
			//The sin law is A/sin(a)=B/sin(b)

			double yDiff = yourPos.Y - origion.Y;
			
			int ySign = Math.Sign(yDiff);
			int xSign = Math.Sign(yourPos.X - origion.X);

			yDiff = Math.Abs(yDiff);

			double dist = (yourPos - origion).Length();
			double result = Math.Asin(yDiff/dist);

			if (xSign == -1 && ySign == 1)
			{
				//Error.Trace("x - y +");
				result = Math.PI-result;
			}
			if (xSign == -1 && ySign == -1)
			{
				//Error.Trace("x - y -");
				result += Math.PI ;
			}
			if (xSign == 1 && ySign == -1)
			{
				//Error.Trace("x + y -");
				result = (Math.PI*2)-result;
			}

			//if (ySign == -1)
			//{
			//	result = (Math.PI * 2) - result;
			//}

			result = MathHelper.ToDegrees((float)result);
			return result;
		}
    }
}
