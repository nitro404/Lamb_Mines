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

namespace Scallywags
{
    /** @class  SplashModule
     *  @brief  the opening splash screen, a type of module
     */
    public class SplashModule : XNAModule 
    {
        private enum STATE
        {
            ST_FADEIN,
            ST_SPLASH,
            ST_FADEOUT
        };
        
        private float m_fElapsedTime;               ///< Time elapsed since module init
        private STATE m_state;                      ///< Current state of the splash screen

        private Texture2D m_texFade;                ///< A black texture.
        private Texture2D m_texSplash;              ///< The splash image
        private float m_fFadeAmount;                ///< Amount to fade the texutre by ( between 0 and 1.0 )

        /** @fn     SplashModule()
         *  @brief  constructor, Creates the splash screen with the correct module ID
         */
        public SplashModule() : base(MODULE_IDENTIFIER.MID_SPLASH_MODULE)
        {
            //One time module setup here
            ShowLoadScreen = false;

            //Resgister required resources
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Splash" );
            
        }

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public override void Initialize()
        {
            Error.Trace("Splash Module Init");

            m_fElapsedTime  = 0;
            m_fFadeAmount   = 1.0f;
            m_state         = STATE.ST_FADEIN;

            m_texFade = new Texture2D(ParentApp.GraphicsDevice, 4, 
                4, 1, TextureUsage.None, SurfaceFormat.Color);

            m_texSplash = Textures[ "Splash" ];

            SetFadeAmount();
        }

        /** @fn     MODULE_IDENTIFIER Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @return the ID of the module to switch to, or ( MID_THIS {0} ) if this module is to continue executing
         *  @param  fTotalTime [in] the total elapsed time since the app began running, in seconds
         *  @param  fElapsedTime [in] the time elapsed since the last frame
         */
        public override MODULE_IDENTIFIER Update(double fTotalTime, float fElapsedTime)
        {
            m_fElapsedTime += fElapsedTime;

            //Check for enter key press to skip this module
            if (ParentApp.Inputs.IsKeyDown(Keys.Enter) || ParentApp.Inputs.IsButtonPressed( 0, Buttons.A ) )
                return MODULE_IDENTIFIER.MID_MENU_MODULE;


            //////////////////////////////////////
            //Update the fade amount
            switch (m_state)
            {
                //The screen is fading in
                case STATE.ST_FADEIN:
                    if (m_fElapsedTime > Settings.SPLASH_FADE_IN_TIME)
                    {
                        m_fFadeAmount = 0;
                        m_state = STATE.ST_SPLASH;
                    }
                    else
                    {
                        m_fFadeAmount = 1.0f - (m_fElapsedTime / Settings.SPLASH_FADE_IN_TIME);
                    }
                    break;

                //The splash screen is visible
                case STATE.ST_SPLASH:

                    if (m_fElapsedTime > Settings.SPLASH_TIME)
                    {
                        m_state         = STATE.ST_FADEOUT;
                        m_fElapsedTime  = 0;
                    }


                    break;

                //The screen is fading out
                case STATE.ST_FADEOUT:

                    //Switch to the menu once FADE_OUT_TIME seconds have elapsed.
                    if (m_fElapsedTime > Settings.SPLASH_FADE_OUT_TIME)
                        return MODULE_IDENTIFIER.MID_MENU_MODULE;
                    else
                    {
                        m_fFadeAmount = m_fElapsedTime / Settings.SPLASH_FADE_OUT_TIME;
                    }
                    break;
                default:
                    break;
            }

            //Update the fade texture
            SetFadeAmount();

            
            return MODULE_IDENTIFIER.MID_THIS;    //Continue running the module.
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void Draw(GraphicsDevice device, GameTime gameTime)
        {

        }

        /** @fn     void DrawHud( GameTime gameTime )
        *  @brief  Draw the module's scene
        *  @param  device [in] the active graphics device
        *  @param  gameTime [in] information about the time between frames
        */
        public override void DrawNonEdgeDetectedFeatures(GraphicsDevice device, GameTime gameTime)
        {
                device.Clear(Color.Black);

                SpriteBatch sb = new SpriteBatch(device);

                sb.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

                int nYStart = (device.Viewport.Height - m_texSplash.Height) / 2;

                if (nYStart < 0)
                    nYStart = 0;

                //Center horizontally
                sb.Draw(m_texSplash,
                    new Rectangle(
                        (device.Viewport.Width - m_texSplash.Width) / 2,
                        nYStart,
                        m_texSplash.Width,
                        m_texSplash.Height),
                        Color.White);

                sb.Draw(m_texFade, new Rectangle(0, 0,
                       device.Viewport.Width,
                       device.Viewport.Width),
                       Color.Black);
                sb.End();

                //ParentApp.DebugFont.DrawFont(sb, "Splash Module", 0, 0.95f, Color.White);
        }

        /** @fn     void SetFadeAmount( float fAmt )
         *  @brief  set the amount of fade to apply to the background image
         */
        private void SetFadeAmount()
        {
            int nNumPixels = m_texFade.Width * m_texFade.Height;
            Color[] nPixels = new Color[nNumPixels];

            for (int i = 0; i < nNumPixels; ++i)
                nPixels[i] = new Color(0.0f, 0, 0, m_fFadeAmount );

            m_texFade.SetData<Color>(nPixels);  
        }
    }
}