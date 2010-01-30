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
using Microsoft.Xna.Framework.GamerServices;    //needed for checking whether or not the guide is visible

namespace Scallywags
{
    /** @class  GameModule
     *  @brief  the main branch of game execution, a type of module
     */
    public class GameModule : XNAModule
    {
        #region DATA_MEMBERS
                              
                private SpriteBatch     m_sb;               ///< The sprite batch for 2D rendering
                

        #endregion


        /** @fn     GameModule()
         *  @brief  constructor
         */
        public GameModule()
            : base(MODULE_IDENTIFIER.MID_GAME_MODULE)
        {
                       
        }

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public override void Initialize()
        {
           
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
                return MODULE_IDENTIFIER.MID_CREDITS_MODULE;

            ///////////////////////////
            //Temp... reset the game
            if( ParentApp.Inputs.IsKeyPressed(Keys.F12) )
                this.Initialize();

            return MODULE_IDENTIFIER.MID_THIS;    //Continue running the module.
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void Draw(GraphicsDevice device, GameTime gameTime)
        {
            device.Clear(Color.Red);

            
        }


        /** @fn     void ShutDown()
         *  @brief  clean up the modules resources
         */
        public override void ShutDown()
        {

            base.ShutDown();
        }

        /** @fn     void TraceGameSettigns()
         *  @brief  debug function to output the current game settigns to the console
         */
        private void TraceGameSettings()
        {
           
        }

       
    }
}
