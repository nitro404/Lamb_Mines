using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
     /**    @class
        *   @brief  the player's gold amount
     */
    public class TurnDisplay : HudItem
    {
        private const float DISPLAY_TIME = 40.0f;

        private int         m_nCurrentTurn;     ///< The current turn number
        private int         m_nMaxTurn;         ///< The maximum turn number
        private Texture2D   m_texBG;            ///< The background       
        private Texture2D   m_texInfinity;      ///< The infinity symbol                               
        private float       m_fAlpha;           ///< How visible is display?
        private SpriteFont  m_fontLarge;

        private float       m_fDisplaytime;                 ///< The time we will be displaying the button
        private Color       m_colFade;                      ///< to apply a fade to the object     

        #region PROPERTIES

        /** @prop   
         *  @brief  the current turn
         */
        public int CurrentTurn
        {
            set
            {
                m_nCurrentTurn = value;
            }
            get
            {
                return m_nCurrentTurn;
            }
        }

        /** @prop   
         *  @brief  max turns
         */
        public int MaxTurn
        {
            set
            {
                m_nMaxTurn = value;
            }
            get
            {
                return m_nMaxTurn;
            }
        }

        /** @prop   
         *  @brief  the visiblity of the hud item
         */
        public float Alpha
        {
            set
            {
                m_fAlpha = value;
            }
            get
            {
                return m_fAlpha;
            }
        }

        #endregion

        /** @fn 
         *  @brief   constructor
         */
        public TurnDisplay()
        {
            m_nCurrentTurn      = 0;
            m_nMaxTurn          = -1;
            m_fAlpha            = 1.0f;
        }

        /** @fn
         *  @void Initialize the turn display
         */
        public void Init( Texture2D bgTex, Texture2D texInfinity, SpriteFont fontLarge )
        {
            m_fAlpha        = 1.0f;
            m_texBG         = bgTex;
            m_texInfinity   = texInfinity;

            m_colFade       = Color.White;
            m_colFade.A     = 0;
            m_fDisplaytime  = DISPLAY_TIME;

            base.Init( m_texBG.Width, m_texBG.Height );

            m_fontLarge = fontLarge;
        }

        /** @fn     
         *  @brief  draw the display
         */
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Color colBG = Color.White;
            //colBG.A = (byte)( 255 * m_fAlpha );
            colBG.A = m_colFade.A;
            //Draw the background
            spriteBatch.Draw(m_texBG,
                              ScreenPosition,
                              colBG );

            ///////////////////////////
            //Draw the turn numbers
            Color colFont = Color.White;
            //colFont.A = (byte)(255 * m_fAlpha);
            colFont.A = m_colFade.A;
            /////////////////////////////////
            //Draw the heading
            string strHeading           = "Turn";
            Vector2 vStringDimensions   = font.MeasureString( strHeading );
            Vector2 vDrawPos            = ScreenPosition;
            
            vDrawPos.X += ( Width - vStringDimensions.X ) * 0.5f;
            vDrawPos.Y += Height * 0.05f;

            Vector2 vShadowOffset = new Vector2( 2.0f, 2.0f );

            Vector2 vShadowPos = vDrawPos + vShadowOffset;
            Color colShadow = Color.Black;
            colShadow.A = colFont.A;

            spriteBatch.DrawString( font, strHeading, vShadowPos, colShadow );
            spriteBatch.DrawString(font, strHeading, vDrawPos, colFont);

            /////////////////////////////////////
            //Draw the turn
            string strTurn      = m_nCurrentTurn.ToString();
            vDrawPos            = ScreenPosition;
            vDrawPos.Y          += Height * 0.38f;

            if( m_nMaxTurn != -1 )
                strTurn += " of " + m_nMaxTurn.ToString();

            vStringDimensions = m_fontLarge.MeasureString(strTurn);
            
            vDrawPos.X += ( Width - vStringDimensions.X ) * 0.5f;

            vShadowPos = vDrawPos + vShadowOffset;

            spriteBatch.DrawString(m_fontLarge, strTurn, vShadowPos, colShadow);
            spriteBatch.DrawString(m_fontLarge, strTurn, vDrawPos, colFont);

        }

        /** @fn 
         *  @brief  update the turn display
         */
        public override void Update(float fElapsedTime)
        {
            m_fDisplaytime -= 5.0f * fElapsedTime;
            float fFadeOutCutoff = DISPLAY_TIME - 10.0f;
          
            if( m_fDisplaytime <= DISPLAY_TIME && m_fDisplaytime > fFadeOutCutoff )
            {
                float fDiff = DISPLAY_TIME - m_fDisplaytime;

                float fPercentElapsed =  fDiff / ( DISPLAY_TIME - fFadeOutCutoff );

                m_colFade.A         = (byte)( 255 * ( fPercentElapsed ) );
                
            }
        }
    }
}
