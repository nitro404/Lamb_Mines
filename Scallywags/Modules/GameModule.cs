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

        Texture2D grass;
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
            m_sb = new SpriteBatch(ParentApp.Device);

            grass = ParentApp.Content.Load<Texture2D>("Content/Textures/grass_base01");
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
            m_sb.Begin();
            for (int x = -1; x < 35; x++)
            {
                for (int y = -20; y < 25; y++)
                {
                    Vector2 position = new Vector2(x * 48, y * 48);
                    position = GetScreenCoords(position);
                    m_sb.Draw(grass, position, Color.White);
                }
            }
            m_sb.End();
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

        //TEMP TESTS
        Vector2 GetScreenCoords(Vector2 iso)
        {
            Vector2 result;
            result.X = GetScreenX(iso);
            result.Y = GetScreenY(iso);
            return result;
        }

        float GetScreenX(Vector2 iso)
        {
            float xBasic = (float)(iso.X - iso.Y) * (float)Math.Cos(0.46365);
            return xBasic;
        }

        float GetScreenY(Vector2 iso)
        {
            float yBasic = (float)(iso.X + iso.Y) * (float)Math.Sin(0.46365);
            return yBasic;
        }

       
    }
}
