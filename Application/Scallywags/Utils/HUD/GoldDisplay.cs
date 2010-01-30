using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class
     *  @brief  the player's gold amount
     */
    public class GoldDisplay : HudItem
    {
        private int         m_nAmount;  ///< The amount of gold
        private Texture2D   m_texBG;    ///< The gold display background                             
        private float       m_fAlpha;   ///< How visible is the gold display?
        private Vector2     m_vParentPos;
        /** @prop   
         *  @brief  the amount of gold to display
         */
        public int CoinAmount
        {
            set
            {
                m_nAmount = value;
            }
            get
            {
                return m_nAmount;
            }
        }

        public Vector2 ParentPos
        {
            set
            {
                m_vParentPos = value;
            }
            get
            {
                return m_vParentPos;
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

        /** @fn 
         *  @brief   constructor
         */
        public GoldDisplay()
        {
            m_nAmount   = 0;
            m_fAlpha    = 1.0f;
            m_vParentPos  = Vector2.Zero;
        }

        /** @fn
         *  @void Initialize the gold display
         */
        public void Init( Texture2D bgTex, Vector2 vPos )
        {
            m_fAlpha    = 1.0f;
            m_texBG     = bgTex;

            ScreenPosition = vPos;
            base.Init( m_texBG.Width, m_texBG.Height );
        }

        /** @fn     
         *  @brief  draw the display
         */
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Color colBG = Color.White;
            colBG.A = (byte)( 255 * m_fAlpha );

            //Draw the GoldDisplay
            spriteBatch.Draw(m_texBG,
                              m_vParentPos + ScreenPosition,
                              null,
                              colBG,
                              0,
                              new Vector2(Width * 0.5f, Height * 0.5f),
                              1.0f,
                              SpriteEffects.None,
                              0.1f);

            //Offset for pristina font
            Vector2 vPosition = ScreenPosition;

            string strCoins = m_nAmount.ToString();
            Vector2 vDimensions = font.MeasureString( strCoins );
            vPosition.X += vDimensions.X * 0.25f;
            vPosition.Y -= vDimensions.Y * 0.45f;

            Color colFont = Color.Yellow;
            colFont.A = (byte)(255 * m_fAlpha);

            Color colShadow = Color.Black;
            colShadow.A = colFont.A;

            Vector2 vDrawPos = m_vParentPos + vPosition;

            spriteBatch.DrawString(font, strCoins, new Vector2( vDrawPos.X + 2.0f, vDrawPos.Y + 2.0f ), colShadow);
            spriteBatch.DrawString(font, strCoins, vDrawPos, colFont);
        }

        /** @fn 
         *  @brief  update the gold display
         */
        public override void Update(float fElapsedTime)
        {

        }
    }
}
