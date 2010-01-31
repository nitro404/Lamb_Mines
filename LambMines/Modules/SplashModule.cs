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
        SpriteBatch newStarterSB;
        Texture2D splashTexture;
        Rectangle drawnRectangle = new Rectangle(0, 0, Settings.PREFFERED_WINDOW_WIDTH, Settings.PREFFERED_WINDOW_HEIGHT);

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
            newStarterSB = new SpriteBatch(ParentApp.Device);
            //splashTexture = Texture2D.FromFile(this.ParentApp.GraphicsDevice, "Textures\\grass_base01.png");
            splashTexture = new Texture2D(ParentApp.Device, Settings.PREFFERED_WINDOW_WIDTH, Settings.PREFFERED_WINDOW_HEIGHT);// = ParentApp.Content.Load<Texture2D>("\\Textures\\explosion_sheet01.png");
            splashTexture = ParentApp.Content.Load<Texture2D>("Content/Textures/screens_intro01");
            //splashTexture = Texture2D.FromFile(ParentApp.Device, "Textures\\explosion_sheet01.xnb");
                //ParentApp.Content.Load<Texture2D>("\\Textures\\explosion_sheet01.png");
            //splashTexture = Texture2D.FromFile(ParentApp.Device, ParentApp.Content.Load<Texture2D>("explosion_sheet01.xnb"));
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
            
            newStarterSB.Begin();
            
            newStarterSB.Draw(splashTexture, drawnRectangle, Color.White);
            newStarterSB.End();
        }
    }
}