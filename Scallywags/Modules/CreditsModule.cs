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
using System.IO;

namespace Scallywags
{
    /** @class  CreditsModule
     *  @brief  the team credits, a type of module
     */
    public class CreditsModule : XNAModule
    {
        private List<string>    m_Credits;
        private float           m_CreditsPosition;

        /** @fn     CreditsModule()
         *  @brief  constructor
         */
        public CreditsModule()
            : base(MODULE_IDENTIFIER.MID_CREDITS_MODULE)
        {
            this.ShowLoadScreen = false;

            //Resgister required resources
        }

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public override void Initialize()
        {
            m_CreditsPosition = 0;

            m_Credits = new List<string>();
            StreamReader theCreditsFile = new StreamReader(@"Content\Font\FinalCredits.txt");
            string info = "";
            string goodInfo = "";
            while ((info = theCreditsFile.ReadLine()) != null) {
                goodInfo = "";
                foreach (char c in info) {
                    if (c == '\t') {
                        goodInfo += "    ";
                    }
                    else {
                        goodInfo += c;
                    }
                }
                m_Credits.Add(goodInfo);
            }


            SoundPlayer.PlayCue("menu_cleaned");
        }

        /** @fn     MODULE_IDENTIFIER Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @return the ID of the module to switch to, or ( MID_THIS {0} ) if this module is to continue executing
         *  @param  fTotalTime [in] the total elapsed time since the app began running, in seconds
         *  @param  fElapsedTime [in] the time elapsed since the last frame
         */
        public override MODULE_IDENTIFIER Update(double fTotalTime, float fElapsedTime)
        {
            //TEMP - Switch to the next module on enter key press
            if (ParentApp.Inputs.IsKeyPressed(Keys.Enter))
                return MODULE_IDENTIFIER.MID_SPLASH_MODULE;

            for(int i = 0; i < 4; i++)
            {
                if (ParentApp.Inputs.IsButtonPressed(i, Buttons.A) || ParentApp.Inputs.IsButtonPressed(i, Buttons.Start) || ParentApp.Inputs.IsButtonPressed(i, Buttons.B) || ParentApp.Inputs.IsButtonPressed(i, Buttons.Back))
                return MODULE_IDENTIFIER.MID_SPLASH_MODULE;
            }

            m_CreditsPosition -= fElapsedTime/23;
            if (m_CreditsPosition <= -1.15-m_Credits.Count*0.03f){
                return MODULE_IDENTIFIER.MID_SPLASH_MODULE;
            }

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

        /** @fn     void Draw( GameTime gameTime )
        *  @brief  Draw the module's non-processed bits
        *  @param  device [in] the active graphics device
        *  @param  gameTime [in] information about the time between frames
        */
        public override void DrawNonEdgeDetectedFeatures(GraphicsDevice device, GameTime gameTime)
        {
                device.Clear(Color.Black);

                SpriteBatch sb = new SpriteBatch(device);

                for (int i = 0; i < m_Credits.Count; i++)
                {
                    ParentApp.DebugFont.DrawFont(sb, m_Credits[i], 0.4f, 1.05f + 0.03f * (i + 1) + m_CreditsPosition, Color.Gold);
                }

                /////////////////////////////////////
                //Debug output

                //ParentApp.DebugFont.DrawFont(sb, "Credits Module", 0, 0.95f, Color.White);
        }

        /** @fn     void ShutDown()
         *  @brief  clean up the modules resources
         */
        public override void ShutDown()
        {
            m_Credits.Clear();

            base.ShutDown();
        }
    }
}