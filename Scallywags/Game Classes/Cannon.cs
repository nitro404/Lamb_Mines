using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  Cannon
     *  @brief  That which shoots cannon balls.  
     *  @note   This may end up as part of the ship...
     */
    public class Cannon : Object3D
    {
        /** @fn     Cannon( Model mdl )
         *  @brief  Constructor
         *  @param  mdl [in] the model to use to display the Cannon
         */
        public Cannon(Model mdl)
            : base(mdl)
        {

        }
    }
}