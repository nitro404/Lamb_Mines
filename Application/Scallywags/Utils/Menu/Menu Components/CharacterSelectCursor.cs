using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class CharacterSelectCursor : MenuItem
    {
        private Texture2D m_texSelected;      ///< The cursor's texture when a character's been selected
        private Texture2D m_texUnselected;    ///< The cursor's texture when a ca\haracter hasn't been selected
                                              ///
        public int CharacterIndex;     ///< The character's index
        public int OwnerID;            ///< The player ID of the owner

        public CharacterSelectCursor(Texture2D texSelected, Texture2D texUnselected, int nCurrentIndex, int nOwnerID)
        {
            m_texSelected   = texSelected;
            m_texUnselected = texUnselected;
            CharacterIndex  = nCurrentIndex;
            OwnerID         = nOwnerID;

            Enabled     = false;
            Selectable  = false;
            Selected    = false;

            Width   = texSelected.Width;
            Height  = texSelected.Height;
        }

        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the UIobject
         *  @param  sb [in] the active sprite batch
         */
        public override void Draw(SpriteBatch sb)
        {
            if( Enabled )
            {
                if (Selectable)
                    sb.Draw( m_texSelected, new Vector2( X, Y ), Color.White );
                else
                    sb.Draw( m_texUnselected, new Vector2( X, Y ), Color.White );
            }
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
