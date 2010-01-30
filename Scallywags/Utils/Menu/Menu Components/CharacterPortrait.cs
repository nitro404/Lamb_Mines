using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  CharacterPortratit
     *  @brief  the box that will contain the selected character portrait and character name
     */
    public class CharacterPortrait : MenuItem
    {
        Texture2D               m_texBG;        ///< The bg texture
        Texture2D               m_texRing;      ///< The border texture
        List< Texture2D >       m_vPortraits;   ///< The possible character portraits
        List< CharacterInfo >   m_vCharacters;  ///< The character info
        
        int                     m_nID;                  ///< Which portrait is this? 
        int                     m_nCurrentCharacter;   ///< The current character index          
                                                       ///
        private ButtonIndicator m_btnIndicator;

        /** @prop   CurrentCharacter
         *  @brief  the ID of the current character
         */
        public int CurrentCharacter
        {
            get
            {
                return m_nCurrentCharacter;
            }
            set
            {
                m_nCurrentCharacter = value;
            }
        }

        /** @fn     CharacterPortrait()
         *  @brief  constructor
         */
        public CharacterPortrait( int nID )
        {
            m_texBG         = null;
            m_texRing       = null;
            m_vCharacters   = null;
            m_vPortraits    = null;
            m_vCharacters   = null;

            m_nCurrentCharacter = 0;

            m_btnIndicator = null;

            m_nID = nID;
        }

        /** @fn     void Init( Texture2D texBorder, SpriteFont font, List<Texture2D> vPortraits )
         *  @brief  initialize the character portrait menu item
         *  @param  texBG [in] the texture to use as a background
         *  @param  texRing [in] the texture to use for a border
         *  @param  font [in] the sprite font to use for the text
         *  @param  vPortraits [in] the character portraits
         *  @param  vCharacterInfo [in] the information about the characters
         *  @param  nStartingCharacter [in] the character to begin displaying
         */
        public void Init( Dictionary< string, Texture2D > dtTexture, SpriteFont font, List<Texture2D> vPortraits, List< CharacterInfo > vCharacterInfo, int nStartingCharacter )
        {
            m_texBG         = dtTexture[ "CharSelectBG" ];
            m_texRing       = dtTexture[ "CharSelectRing" ];
            Font            = font;
            m_vPortraits    = vPortraits;
            m_vCharacters   = vCharacterInfo;

            m_btnIndicator = new ButtonIndicator( dtTexture );

            Width   = m_texBG.Width;
            Height  = m_texBG.Height;

            m_nCurrentCharacter = nStartingCharacter;
        }

        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the UIobject
         *  @param  sb [in] the active sprite batch
         */
        public override void Draw(SpriteBatch sb)
        {
            //Draw the background
            Color bgColor = Color.White;

            if( m_nCurrentCharacter < m_vCharacters.Count )
                bgColor = m_vCharacters[ m_nCurrentCharacter ].color;

           // sb.Draw( m_texBG, new Vector2( X, Y ), bgColor );

            //Draw the current portrait
            //float fX = X;// +(m_texBG.Width - m_vPortraits[m_nCurrentCharacter].Width) / 2;
            //float fY = Y;

            Vector2 vDrawPos = new Vector2(X, Y);

            sb.Draw( m_texBG, vDrawPos, Color.White);

            sb.Draw( m_vPortraits[m_nCurrentCharacter], vDrawPos, Color.White);
  
            //Draw the border
            sb.Draw( m_texRing, vDrawPos, Color.White );

            string strTop = "Player " + (m_nID + 1);
            string strBot = "Press /A to join";

            if( m_nCurrentCharacter < m_vCharacters.Count ) 
            {
                strTop =  "Captain";
                strBot =  m_vCharacters[ m_nCurrentCharacter ].strName;
            }

            //Draw the captain's name, centered
            Vector2 vTopDimensions = Font.MeasureString( strTop );
            Vector2 vBotDimensions = Font.MeasureString( strBot );

            float fCapnX = X + ( m_texBG.Width - vTopDimensions.X ) / 2;
            float fCapnY = Y + ( m_texBG.Height - vBotDimensions.Y * 2.25f );

            float fX = X + ( m_texBG.Width - vBotDimensions.X ) / 2;
            float fY = fCapnY + vTopDimensions.Y;

            sb.DrawString( Font, strTop, new Vector2( fCapnX, fCapnY ), Color.Black );
            //sb.DrawString( Font, strBot, new Vector2( fX, fY ), Color.Black );
            //Unfortunatly had to hard code the positioning.
            m_btnIndicator.SetText( strBot, Font, new Rectangle( (int)fCapnX - 55, (int)fCapnY - 113, (int)m_texBG.Width, (int)m_texBG.Height ), Color.Black );
            m_btnIndicator.Draw( sb );
            
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
