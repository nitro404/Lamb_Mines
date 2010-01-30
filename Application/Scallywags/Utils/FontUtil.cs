using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class FontUtil
     *  @brief Font class to handle displaying fonts for testing purposes.
     */
    public class FontUtil
    {
        private SpriteFont m_gameFont;      ///< The font
        private Rectangle m_viewportRect;   ///< The viewport rectangle were we will draw

        public SpriteFont Font
        {
            get
            {
                return m_gameFont;
            }
            set
            {
                m_gameFont = value;
            }
        }

        /** @fn     FontUtil( )
         *  @brief  Class constructor
         */
        public FontUtil()
        {
            m_gameFont = null;
            m_viewportRect = new Rectangle(0, 0, 0, 0);
        }

        /** @fn     void Init()
         *  @brief  Initializes our font by loading it in from file.
         *          Also sets up our viewportRect.
         */
        public void Init(int nViewportWidth, int nViewportHeight, SpriteFont gameFont)
        {
            m_gameFont = gameFont;
            m_viewportRect = new Rectangle(0, 0, nViewportWidth, nViewportHeight);
        }

        /** @fn     void DrawFont( SpriteBatch spriteBatch, String strString, float fX, float fY, Color color )
         *  @brief  Takes the string we pass in and draws it onto the screen.
         *  @param  spriteBatch [in] the spriteBatch that we use to draw with.
         *  @param  strText [in] the text we want to draw.
         *  @param  fX [in] the location we wish to draw the font on the x axsis (0.0f to 0.9f).
         *  @param  fY [in] the location we wish to draw the font on the y axsis (0.0f to 0.9f).
         *  @param  color [in] the color of the font.
         */
        public void DrawFont(SpriteBatch spriteBatch, String strText, float fX, float fY, Color color)
        {
            spriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState );

            spriteBatch.DrawString(m_gameFont, strText, new Vector2((fX+0.002f) * m_viewportRect.Width, (fY+0.002f) * m_viewportRect.Height), Color.Black);
            spriteBatch.DrawString(m_gameFont, strText, new Vector2(fX * m_viewportRect.Width, fY * m_viewportRect.Height), color);

            spriteBatch.End();
        }

        /** @fn     void DrawFont( SpriteBatch spriteBatch, int nNumber, float fX, float fY, Color color )
         *  @brief  Takes the string we pass in and draws it onto the screen.
         *  @param  spriteBatch [in] the spriteBatch that we use to draw with.
         *  @param  nNumber [in] the numbers we want to draw.
         *  @param  fX [in] the location we wish to draw the font on the x axsis (0.0f to 0.9f).
         *  @param  fY [in] the location we wish to draw the font on the y axsis (0.0f to 0.9f).
         *  @param  color [in] the color of the font.
         */
        public void DrawFont(SpriteBatch spriteBatch, int nNumber, float fX, float fY, Color color)
        {
            spriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState );

            spriteBatch.DrawString(m_gameFont, nNumber.ToString(), new Vector2((fX+0.002f) * m_viewportRect.Width, (fY+0.002f) * m_viewportRect.Height), Color.Black);
            spriteBatch.DrawString(m_gameFont, nNumber.ToString(), new Vector2(fX * m_viewportRect.Width, fY * m_viewportRect.Height), color);

            spriteBatch.End();
        }
    }
}