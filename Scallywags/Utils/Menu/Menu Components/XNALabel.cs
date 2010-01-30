using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  
     *  @brief  class to contain an XNA label that can glow
     */
    public class XNALabel : MenuItem
    {
        Color       m_colFont;      ///< The font colour
        Color       m_colSelected;  ///< The colour to use when selected
        /** @fn     XNALabel
         *  @brief  constructor
         */
        public XNALabel(  )
        {
            m_colFont           = Color.Black;
            m_colSelected       = Color.DarkRed;
            m_colSelected.A     = 100;

            Selected    = false;
            Enabled     = true;
            Text        = "";
        }

        /** @fn     Init
         *  @brief  initialize the label
         */
        public void Init( SpriteFont font, string strText )
        {
            Selected    = false;
            Enabled     = true;
            Font        = font;
            Text        = strText;

            Vector2 vTextDimensions = font.MeasureString( strText );

            Width   = vTextDimensions.X;
            Height  = vTextDimensions.Y;
        }
        
        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the UIobject
         *  @param  sb [in] the active sprite batch
         */
        public override void Draw( SpriteBatch sb )
        {
            if( Text.Length < 1 )
                return;

            //Unselected settings
            float fScale    = 1.0f;
            float fXOffset  = 0;
            float fYOffset  = 0;

            m_colFont.A = 200;

            //Selected settings
            if( Selected )
            {
                float fMaxScaleUp   = 0.25f;
                fScale              = 1.0f + fMaxScaleUp;
                fXOffset            = ( Width * fScale - Width ) / 2;
                fYOffset            = (Height * fScale - Height) / 2;
                m_colFont.A         = 255;
            }

            byte alpha = m_colFont.A;

            if( Enabled == false )
                m_colFont.A /= 2;

            if (Selected)
            {
                sb.DrawString(Font, Text, new Vector2(X - fXOffset + 1.0f, Y - fYOffset + 1.0f), m_colSelected, 0, Vector2.Zero, fScale, SpriteEffects.None, 0.1f);
                sb.DrawString(Font, Text, new Vector2(X - fXOffset + 2.0f, Y - fYOffset + 2.0f), m_colSelected, 0, Vector2.Zero, fScale, SpriteEffects.None, 0.1f);
            
            }

            //Draw the actual item
            sb.DrawString(Font, Text, new Vector2(X - fXOffset, Y - fYOffset), m_colFont, 0, Vector2.Zero, fScale, SpriteEffects.None, 0.1f);

            
            m_colFont.A = alpha;
        }

        /** @fn     void Update( float fElapsedTime, InputManager inputs  )
         *  @brief  update the UIObject
         *  @param  fElapsedTime [in] the time since the last frame, in seconds.
         *  @param  inputs [in] the state of the input
         */
        public override void Update(float fElapsedTime, InputManager inputs)
        {

        }
    }
}
