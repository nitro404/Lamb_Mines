using System;
using System.Collections;
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
    abstract class Object
    {

        //private variables
        private Vector2 position = new Vector2();
        public Texture2D myTexture;
        private bool StillAlive = true;

        //properties
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
		}

        public bool isAlive
        {
            get { return isAlive; }
        }
        public void KillMe()
        {
            StillAlive = false;
        }

        //These are some default constructors
        public Object(ref Texture2D aTexture) { myTexture = aTexture; }
        public Object(int[] Location, ref Texture2D aTexture)
        {
            position.X = (float)Location[0];
            position.Y = (float)Location[1];
            myTexture = aTexture;
        }
        public Object(double[] Location, ref Texture2D aTexture)
        {
			position.X = (float)Location[0];
			position.Y = (float)Location[1];
            myTexture = aTexture;
        }
        public Object(ArrayList Location, ref Texture2D aTexture)
        {
            position.X = (float)Location[0];
            position.Y = (float)Location[1];
            myTexture = aTexture;
        }
		public Object(Vector2 Location, ref Texture2D aTexture)
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
        abstract public bool onCollision(Object collisionObject);

        /// <summary>
        /// General update loop
        /// </summary>
        abstract public void Update();

        /// <summary>
        /// General draw loop
        /// </summary>
        abstract public void Draw(SpriteBatch spriteThing);

        /// <summary>
        /// Kill this unit.
        /// </summary>
        abstract public void Kill();

    }
}
