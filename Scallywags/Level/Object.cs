using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Scallywags
{
    abstract class Object
    {

        //private variables
        private double[] position = new double[2];
        private int/*Texture*/ myTexture;
        private bool StillAlive = true;

        //properties
        public double PositionX
        {
            get { return position[0]; }
            set { position[0] = value; }
        }
        public double PositionY
        {
            get { return position[1]; }
            set { position[1] = value; }
        }
        public double[] Position
        {
            get { return position; }
            set { if (value.Length == 2) { position = value; } else { Log.WriteToLog(Log.LogErrorLevel.ERROR_MINOR, "Position value should have only two values, an X and a Y value"); } }
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
        public Object() { }
        public Object(int[] Location)
        {
            position[0] = (double)Location[0];
            position[1] = (double)Location[1];
        }
        public Object(double[] Location)
        {
            if (Location.Length == 2)
            {
                position = Location;
            }
        }
        public Object(ArrayList Location)
        {
            position[0] = (int)Location[0];
            position[1] = (int)Location[1];
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
        abstract public void Draw();

        /// <summary>
        /// Kill this unit.
        /// </summary>
        abstract public void Kill();

    }
}
