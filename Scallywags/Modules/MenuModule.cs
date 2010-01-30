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
    
    /** @class  MenuModule
     *  @brief  the opening menu screen, a type of module
     */
    public class MenuModule : XNAModule
    {
        SpriteBatch m_sb;
        Animation anim;
        AnimationPlayer player;
        /** @fn     MenuModule()
         *  @brief  constructor
         */
        public MenuModule()
            : base(MODULE_IDENTIFIER.MID_MENU_MODULE)
        {
            ShowLoadScreen = false;
            RegisterResource(ResourceType.RT_TEXTURE, "Content/Textures/sheep_sheet01");
            player = new AnimationPlayer();
        }

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public override void Initialize()
        {
            m_sb = new SpriteBatch(ParentApp.Device);
            anim = new Animation(Textures["sheep_sheet01"], 0.5f, true, new Vector2(35, 35), 2);
            player.PlayAnimation(anim);
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
                return MODULE_IDENTIFIER.MID_GAME_MODULE;

            return MODULE_IDENTIFIER.MID_THIS; ;   
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void Draw(GraphicsDevice device, GameTime gameTime)
        {
            device.Clear( Color.RosyBrown );//Color.SteelBlue ); //Kind of sky blue
            m_sb.Begin();
            player.Draw(gameTime, m_sb, new Vector2(0, 0), new SpriteEffects());
            m_sb.End();
        }


        /** @fn     void ShutDown()
         *  @brief  clean up the modules resources
         */
        public override void ShutDown()
        {

            base.ShutDown();
        }

    }
}
