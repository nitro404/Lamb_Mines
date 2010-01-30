using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scallywags
{
    /** @class  ControlsMenu
     *  @brief  the menu screen that will show the control setup
     */
    public class ControlsMenu : MainMenuScreen
    {
        Texture2D m_texControls;        ///< The controls texture

        Vector2 m_vDrawPos;             ///< The upper left coordinate of the control display

        XNALabel m_lblBack;
        XNALabel m_lblTitle;

        /** @fn     void Init
         *  @param  device [in] the active rendering device
         *  @param  font [in] the font the menu items will use
         *  @param  drawRect [in] the area of the screen this item can draw to
         *  @brief  initialize the menu screen
         */
        public override void Init(GraphicsDevice device, Dictionary<string, SpriteFont> fonts, Rectangle drawRect, Dictionary<String, Texture2D> dtTextures)
        {
            float fHorizontalGap = 100.0f;   ///< Distance from the right of the draw area to draw items
            float fVerticalGap = 100.0f;   ///< Distance from the bottom of the draw area to draw items
            ///
            m_lblBack = new XNALabel();
            m_lblBack.Init(fonts[ "PirateFontLarge" ], "NEXT");
            m_lblBack.X = drawRect.X + drawRect.Width - fHorizontalGap - m_lblBack.Width;
            m_lblBack.Y = drawRect.Y + drawRect.Height - fVerticalGap;// - m_lblBack.Height;
            m_lblBack.Selected = true;

            AddMenuItem(m_lblBack);

            m_texControls = dtTextures[ "ControlsDisplay" ];

            m_vDrawPos = new Vector2(
                drawRect.X + ( drawRect.Width - m_texControls.Width ) / 2.0f,
                drawRect.Y + ( drawRect.Height - m_texControls.Height ) / 2.0f );

            m_lblTitle = new XNALabel();
            m_lblTitle.Init(fonts["PirateFontLarge"], "Controls");
            m_lblTitle.X = drawRect.X + (drawRect.Width - m_lblTitle.Width) / 2.0f;
            m_lblTitle.Y = drawRect.Y + m_lblTitle.Height;// * 2.0f;

            AddMenuItem( m_lblTitle );
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            sb.Draw( m_texControls, m_vDrawPos, Color.White );
        }

        /** @fn     GameMenuID Update( double fTotalTime, float fElapsedTIme, InputManager input )
         *  @brief  the update function
         *  @return the ID of the menu screen to switch to, or GMN_THIS if this screen is to continue to run
         *  @param  fTotalTime [in] the total time the application has been running
         *  @param  fElapsedTime [in] the time, in seconds, since the previous frame
         *  @param  input [in] the application's input manager
         */
        public override GameMenuID Update(double fTotalTime, float fElapsedTime, InputManager input, SoundManager sounds, int Control)
        {
            base.Update(fTotalTime, fElapsedTime, input, sounds, Control);

            if (input.IsKeyPressed(Keys.Enter) || input.IsButtonPressed(Control, Buttons.A))
                return GameMenuID.GMN_HOWTOPLAY;
            if( input.IsKeyPressed(Keys.Back) || input.IsButtonPressed( Control, Buttons.B ) )
                return GameMenuID.GMN_MAINMENU;

            return GameMenuID.GMN_THIS;
        }
    }
}
