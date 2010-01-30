using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scallywags
{
    class HowToPlay : MainMenuScreen
    {

        XNALabel m_lblBack;
        XNALabel m_lblTitle;

        SpriteFont m_fInfoFont;
        Texture2D m_tTower;
        Texture2D m_tBoat;
        Texture2D m_tCannon;
        Texture2D m_tInfluence;
        Texture2D m_tCoin;
        Texture2D m_tCompass;

        Rectangle m_drawRect;

        /** @fn     void Init
         *  @param  device [in] the active rendering device
         *  @param  font [in] the font the menu items will use
         *  @param  drawRect [in] the area of the screen this item can draw to
         *  @brief  initialize the menu screen
         */
        public override void Init(GraphicsDevice device, Dictionary<string, SpriteFont> fonts, Rectangle drawRect, Dictionary<String, Texture2D> dtTextures)
        {
            m_drawRect = drawRect;

            float fHorizontalGap = 100.0f;   ///< Distance from the right of the draw area to draw items
            float fVerticalGap = 100.0f;   ///< Distance from the bottom of the draw area to draw items
            ///
            m_lblBack = new XNALabel();
            m_lblBack.Init(fonts["PirateFontLarge"], "OK");
            m_lblBack.X = drawRect.X + drawRect.Width - fHorizontalGap - m_lblBack.Width;
            m_lblBack.Y = drawRect.Y + drawRect.Height - fVerticalGap;// - m_lblBack.Height;
            m_lblBack.Selected = true;

            AddMenuItem(m_lblBack);

            m_lblTitle = new XNALabel();
            m_lblTitle.Init(fonts["PirateFontLarge"], "How to Play");
            m_lblTitle.X = drawRect.X + (drawRect.Width - m_lblTitle.Width) / 2.0f;
            m_lblTitle.Y = drawRect.Y + m_lblTitle.Height;// * 2.0f;

            AddMenuItem(m_lblTitle);

            m_fInfoFont = fonts["PirateFont"];
            m_tTower = dtTextures["Tower"];
            m_tBoat = dtTextures["Boat"];
            m_tCannon = dtTextures["Cannon"];
            m_tInfluence = dtTextures["Influence"];
            m_tCoin = dtTextures["Coin"];
            m_tCompass = dtTextures["Compass"];
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            Vector2 vStringDim  = m_fInfoFont.MeasureString( "TEST" );
            Vector2 vDrawPos    = new Vector2( m_drawRect.X + m_drawRect.Width * 0.1f, m_drawRect.Y + m_drawRect.Height * 0.20f );
            Vector2 vImagePos   = new Vector2( m_drawRect.X + m_drawRect.Width  - (m_drawRect.Width * 0.1f), vDrawPos.Y );
            float fHalfHeight = vStringDim.Y / 2;
            float fStringWidth = 0;
            float fHorizontalGap = 5.0f;

            sb.DrawString(m_fInfoFont, "You are a Pirate.  You want to plunder.", vDrawPos, Color.Black);
            fStringWidth = m_fInfoFont.MeasureString( "You are a Pirate.  You want to plunder." ).X;
            sb.Draw(m_tBoat, vDrawPos + new Vector2(fStringWidth + fHorizontalGap, m_tBoat.Height / -2 + fHalfHeight), Color.White);
            
            vDrawPos.Y  += 2 * vStringDim.Y;
            vImagePos.Y = vDrawPos.Y;

            sb.DrawString(m_fInfoFont, "Go to the center island of Port Pitiful, and pillage their gold.", vDrawPos, Color.Black);
            fStringWidth = m_fInfoFont.MeasureString("Go to the center island of Port Pitiful, and pillage their gold.").X;
            sb.Draw(m_tCoin, vDrawPos + new Vector2(fStringWidth + fHorizontalGap, m_tCoin.Height / -2 + fHalfHeight), Color.White);

            vDrawPos.Y += 2 * vStringDim.Y;
            vImagePos.Y = vDrawPos.Y;

            sb.DrawString( m_fInfoFont, "Follow your ship's compass home to collect your coin.", vDrawPos, Color.Black );
            fStringWidth = m_fInfoFont.MeasureString("Follow your ship's compass home to collect your coin.").X;
            sb.Draw(m_tCompass, vDrawPos + new Vector2(fStringWidth + fHorizontalGap, m_tCompass.Height / -2 + fHalfHeight), Color.White);

            vDrawPos.Y += 2 * vStringDim.Y;
            vImagePos.Y = vDrawPos.Y;

            sb.DrawString(m_fInfoFont, "But watch out for their Towers, they pack a mean punch.", vDrawPos, Color.Black);
            fStringWidth = m_fInfoFont.MeasureString("But watch out for their Towers, they pack a mean punch.").X;
            sb.Draw(m_tTower, vDrawPos + new Vector2(fStringWidth + fHorizontalGap, m_tTower.Height / -2 + fHalfHeight), Color.White);
            
            vDrawPos.Y += 2 * vStringDim.Y;
            vImagePos.Y = vDrawPos.Y;

            sb.DrawString(m_fInfoFont, "Should your fellow pirates take too much gold, fire a cannon at them.", vDrawPos, Color.Black);
            fStringWidth = m_fInfoFont.MeasureString("Should your fellow pirates take too much gold, fire a cannon at them.").X;
            sb.Draw(m_tInfluence, vDrawPos + new Vector2(fStringWidth + fHorizontalGap, m_tInfluence.Height / -2 + fHalfHeight), Color.White);

            vDrawPos.Y += 2 * vStringDim.Y;
            vImagePos.Y = vDrawPos.Y;

            sb.DrawString(m_fInfoFont, "As the fuse burns down, all four players may infuence where the shot lands.", vDrawPos, Color.Black);
            fStringWidth = m_fInfoFont.MeasureString("As the fuse burns down, all four players may infuence where the shot lands.").X;
            sb.Draw(m_tCannon, vDrawPos + new Vector2(fStringWidth + fHorizontalGap, m_tCannon.Height / -2 + fHalfHeight), Color.White);

            vDrawPos.Y += 2 * vStringDim.Y;
            vImagePos.Y = vDrawPos.Y;

            sb.DrawString(m_fInfoFont, "The Richest Pirate when the game ends is the winner.", vDrawPos, Color.Black);

            vDrawPos.Y += 2 * vStringDim.Y;
            vImagePos.Y = vDrawPos.Y;

            /*
            sb.DrawString(m_fInfoFont, "You are a Pirate.  You want to plunder.", new Vector2(325.0f, 200.0f), Color.Black);

            sb.Draw(m_tBoat, new Vector2(600.0f, 175.0f), Color.White);

            sb.DrawString(m_fInfoFont, "Go to the center island of Port Pitiful, and pillage their gold.", new Vector2(325.0f, 250.0f), Color.Black);

            sb.Draw(m_tCoin, new Vector2(735.0f, 240.0f), Color.White);

            sb.DrawString(m_fInfoFont, "But watch out for their Towers, they pack a mean punch.", new Vector2(325.0f, 300.0f), Color.Black);

            sb.Draw(m_tTower, new Vector2(725.0f, 285.0f), Color.White);

            sb.DrawString(m_fInfoFont, "Should your fellow pirates take too much gold, fire a cannon at them.", new Vector2(325.0f, 350.0f), Color.Black);

            sb.Draw(m_tInfluence, new Vector2(825.0f, 390.0f), Color.White);

            sb.DrawString(m_fInfoFont, "As the fuse burns down, all four players may infuence where the shot lands.", new Vector2(325.0f, 400.0f), Color.Black);

            sb.Draw(m_tCannon, new Vector2(795.0f, 340.0f), Color.White);

            sb.DrawString(m_fInfoFont, "The Richest Pirate when the game ends is the winner.", new Vector2(325.0f, 450.0f), Color.Black);
             * 
             */
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
                return GameMenuID.GMN_MAINMENU;
            if (input.IsKeyPressed(Keys.Back) || input.IsButtonPressed(Control, Buttons.B))
                return GameMenuID.GMN_CONTROLS;

            return GameMenuID.GMN_THIS;
        }

    }
}
