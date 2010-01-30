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
		private bool AmIStopped = false;

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
		public bool Stopped
		{
			get { return AmIStopped; }
			set { AmIStopped = value; }
		}

		//public methods.
        public void KillMe()
        {
			// I'm writing a note here... huge success.
			// It's hard to overstate my... satisfaction.
            StillAlive = false;
        }

        //These are some default constructors
<<<<<<< .mine
        public Object(ref Texture2D aTexture) { myTexture = aTexture; }
        public Object(int[] Location, Texture2D aTexture)
=======
        public Object(Texture2D aTexture) { myTexture = aTexture; }
        public Object(int[] Location, Texture2D aTexture)
>>>>>>> .r22
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
        abstract public bool onCollision(Object collisionObject);

        /// <summary>
        /// General update loop
        /// </summary>
        abstract public void Update(float elapsedTime);

        /// <summary>
        /// General draw loop
        /// </summary>
        abstract public void Draw(SpriteBatch sb, GameTime gameTime);

        /// <summary>
        /// Kill this unit.
        /// </summary>
        abstract public void Kill();

    }
}
