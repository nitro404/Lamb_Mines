using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scallywags
{
    class Mine:Object
    {

        public Mine(int[] location)
            : base(location)
        {

        }

        /// <summary>
        /// This is the trigger function that is called when there is a collision on this object.
        /// </summary>
        /// <returns>Returns FALSE only if this object needs to be destroyed.</returns>
        public override bool onCollision(Object collisionObject)
        {
            //TODO: More collision logic must be applied here.
            collisionObject.Kill();//Kill the other object because this is a mine
            Kill();//This object will explode when anything collides with it.
            return false;//This object will be destroyed.
        }

        public override void Update()
        {

        }
        public override void Draw()
        {

        }
        public override void Kill()
        {
            KillMe();
        }
    }
}
