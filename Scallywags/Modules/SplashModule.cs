using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace LambMines
{
    /** @class  SplashModule
     *  @brief  the opening splash screen, a type of module
     */
    public class SplashModule : XNAModule
    {

        /** @fn     SplashModule()
         *  @brief  constructor, Creates the splash screen with the correct module ID
         */
        public SplashModule()
            : base(MODULE_IDENTIFIER.MID_SPLASH_MODULE)
        {
            //One time module setup here
            ShowLoadScreen = false;

        }

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public override void Initialize()
        {
            Error.Trace("Splash Module Init");
        }

        /** @fn     MODULE_IDENTIFIER Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @return the ID of the module to switch to, or ( MID_THIS {0} ) if this module is to continue executing
         *  @param  fTotalTime [in] the total elapsed time since the app began running, in seconds
         *  @param  fElapsedTime [in] the time elapsed since the last frame
         */
        public override MODULE_IDENTIFIER Update(double fTotalTime, float fElapsedTime)
        {

            if (ParentApp.Inputs.IsKeyDown(Keys.Enter) || ParentApp.Inputs.IsButtonPressed(0, Buttons.A))
                return MODULE_IDENTIFIER.MID_MENU_MODULE;

            return MODULE_IDENTIFIER.MID_THIS;    //Continue running the module.
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void Draw(GraphicsDevice device, GameTime gameTime)
        {
            device.Clear(Color.SeaGreen);
        }

    }
}