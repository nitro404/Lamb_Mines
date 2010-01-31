using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace LambMines
{
    /** @class  ScallyWagsApp
     *  @brief  the main application class, a type of modular app
     */  
    public class ScallyWagsApp : XNAModularApp
    {
               

        /** @fn     ScallyWagsApp( XNAModule startModule )
         *  @brief  constructor
         *  @param  startModule [in] the module the application will begin running right away
         */
        public ScallyWagsApp( XNAModule startModule )
            : base( startModule )
        {
            IsFixedTimeStep = false;

            //Temp
            //SoundPlayer.SoundEffectVolume = 0.5f;
        }
    }
}
