using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veal
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





        abstract public bool onCollision();
        abstract public void Update();
        abstract public void Draw();
        abstract public void Kill();

    }
}
