using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Scallywags
{
    /** @class  CharacterSelector
     *  @brief  class to describe the minibar that will allow players to choose a character
     */
    public class CharacterSelector : MenuItem
    {
        CharacterSelectCursor[] m_vCursors;     ///< The selection cursors
        Texture2D               m_texPortraits; ///< The character portrait bar

        /** @prop   Cursors
         *  @brief  the game cursors
         */
        public CharacterSelectCursor[] Cursors
        {
            get
            {
                return m_vCursors;
            }
        }

        /** @prop   ActiveSelectors
         *  @brief  the number of players actively choosing characters
         */
        public int ActiveSelectors
        {
            get
            {
                int nActive = 0;

                for( int i = 0; i < m_vCursors.Length; ++i )
                {
                    if (m_vCursors[i].Enabled == true && m_vCursors[i].Selectable == true )
                        nActive++;
                }

                return nActive;
            }
        }

        /** @fn     CharacterSelector
         *  @brief  constructor
         */
        public CharacterSelector()
        {
            m_vCursors = null;
            m_texPortraits = null;
        }

        /** @fn     void Init( Texture2D texPortraits, Texture2D []  vCursorTexs )
         *  @brief  initialize the character selector
         *  @param  texPortraits [in] the texture that has all 6 mini portraits on it
         *  @param  vCursorTexs [in] the textures to use for the cursors of all 4 players
         */
        public void Init( Texture2D texPortraits, Texture2D [] vCursorTexs )
        {
            m_texPortraits = texPortraits;

            m_vCursors = new CharacterSelectCursor[ 4 ];

            for( int i = 0; i < 4; ++i )
            {
                m_vCursors[ i ] = new CharacterSelectCursor( vCursorTexs[ i ], vCursorTexs[ i + 4 ], -1, i );
                
                m_vCursors[ i ].Enabled     = false;    //This will be user to check if the cursor is to be drawn or not
                m_vCursors[ i ].Selectable  = true;     //This will be used to control if the user can move the cursor
            }

            //Enable player one by default on the first character
            m_vCursors[ 0 ].CharacterIndex  = 0;
            m_vCursors[ 0 ].Enabled         = true;

            Width   = texPortraits.Width;
            Height  = texPortraits.Height;
        }

        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the UIobject
         *  @param  sb [in] the active sprite batch
         */
        public override void Draw(SpriteBatch sb)
        {
            //Draw the minibar
            sb.Draw( m_texPortraits, new Vector2( X, Y ), Color.White );

            //Draw the active cursors
            float fWidth = m_texPortraits.Width / 6;
            float fY = Y;// + ( m_texPortraits.Height );
            for( int i = 0; i < m_vCursors.Length; ++i )
            {
                if( m_vCursors[i].Enabled )
                {
                    Vector2 vPos = new Vector2( X + ( fWidth / 2.0f ) + m_vCursors[i].CharacterIndex * fWidth - ( m_vCursors[i].Width / 2.0f ), fY );
                    m_vCursors[ i ].X = vPos.X;
                    m_vCursors[ i ].Y = fY;
                    m_vCursors[ i ].Draw( sb );
                }
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

        #region INTERFACE

        /** @fn     bool IsPlayerSelecting( int nPlayerIndex )
         *  @brief  check if a player is selecting a character
         *  @return true if yes, false otherwise
         *  @param  nPlayerIndex [in] the id of the player to check
         */
        public bool IsPlayerSelecting(int nPlayerIndex)
        {
            return (m_vCursors[nPlayerIndex].Enabled == true && m_vCursors[nPlayerIndex].Selectable);
        }

        /** @fn     void MoveSelectionRight( int nPlayerIndex )
         *  @brief  move the provided character's selection cursor right
         *  @param  nPlayerIndex [in] the index of the player to move
         */
        public void MoveSelectionRight( int nPlayerIndex )
        {
            if( m_vCursors[ nPlayerIndex ].Enabled == true && m_vCursors[ nPlayerIndex ].Selectable == true )
                m_vCursors[ nPlayerIndex ].CharacterIndex = FindNextAvailableIndex( m_vCursors[ nPlayerIndex ].CharacterIndex );
        }

        /** @fn     void MoveSelectionLeft( int nPlayerIndex )
         *  @brief  move the provided character's selection cursor left
         *  @param  nPlayerIndex [in] the index of the player to move
         */
        public void MoveSelectionLeft( int nPlayerIndex )
        {
             if( m_vCursors[ nPlayerIndex ].Enabled == true && m_vCursors[ nPlayerIndex ].Selectable == true )
                m_vCursors[ nPlayerIndex ].CharacterIndex = FindPreviousAvailableIndex( m_vCursors[ nPlayerIndex ].CharacterIndex );
        }

        /** @fn     void HandleSelection( int nPlayerIndex )
         *  @brief  handle a selection press by a player
         *  @param  nPlayerIndex [in] the index of the player who hit the A button
         */
        public void HandleSelection( int nPlayerIndex )
        {
            //Activate inactive cursors
            if (m_vCursors[nPlayerIndex].Enabled == false)
            {
                m_vCursors[nPlayerIndex].Enabled = true;
                m_vCursors[nPlayerIndex].Selectable = true;

                m_vCursors[nPlayerIndex].CharacterIndex = FindFirstAvailableIndex();
            }
            else
            {
                //Toggle Selectable status - this will mark a character as selected
                m_vCursors[nPlayerIndex].Selectable = false;
            }
        }

        /** @fn     void HandleUnselection( int nPlayerIndex )
         *  @brief  handle a player hitting the unselect button
         *  @param  nPlayerIndex [in] the player that pressed the button
         */
        public void HandleUnselection( int nPlayerIndex )
        {
            //Ignore players that haven't even been enabeld yet
            if( m_vCursors[ nPlayerIndex ].Enabled  ) 
            {
                if( m_vCursors[nPlayerIndex].Selectable == false )
                {
                    //Reallow players to select characters if they've already selected one
                    m_vCursors[nPlayerIndex].Selectable = true;
                }
                else
                {
                    //Don't allow player one to leave character selection....

                    if( 0 != nPlayerIndex )
                    {
                        //Disable players that are currently selecting
                        m_vCursors[ nPlayerIndex ].Enabled          = false;
                        m_vCursors[ nPlayerIndex ].CharacterIndex   = 6;
                    }
                }
            }

        }

        /** @fn      List< int > GetUnselectedCharacterIDs()
         *  @brief  get an array of character IDs that aren't currently selected
         *  @return the array of unselected character IDs
         */
        public List< int > GetUnselectedCharacterIDs()
        {
            List< int > retVal = new List<int>();

            for( int i = 0; i < 6; ++i )
            {
                if( IsCharacterSelected( i ) )
                    continue;

                retVal.Add( i );
            }

            return retVal;
        }

        /** @fn     int FindFirstAvailableIndex()
         *  @brief  find the first available character selection slot
         *  @return the ID of the first available slot
         */
        private int FindFirstAvailableIndex()
        {
            int nMaxIndex = 6;

            for( int i = 0; i < nMaxIndex; ++i )
            {
                if( IsCharacterSelected( i ) == false )
                    return i;
            }

            //If one isn't found, default to the random face
            return nMaxIndex;
        }

        /** @fn     bool IsCharacterSelected( int nIndex )
         *  @brief  check if a character index is selected by any player
         *  @return true if the index is beign used, false otherwise
         *  @param  nIndex [in] the index of the character to check
         */
        bool IsCharacterSelected( int nIndex )
        {
            for( int i = 0; i < m_vCursors.Length; ++i )
            {
                if( m_vCursors[ i ].CharacterIndex == nIndex )
                    return true;
            }

            return false;
        }

        /** @fn     int FindPreviousAvailableIndex( int nCurrentIndex )
         *  @brief  find the last available index from a given position
         *  @return the index of the first previous character slot that is open
         *  @param  nCurrentIndex [in] the index to begin searching from
         */
        private int FindPreviousAvailableIndex( int nCurrentIndex )
        {
            int nRetVal = nCurrentIndex;

            do
            {
                nRetVal = ( nRetVal + 5 ) % 6; 

            }while( IsCharacterSelected( nRetVal ) );

            return nRetVal;
        }

        /** @fn     int FindNextAvailableIndex( int nCurrentIndex )
         *  @brief  find the next available index from a given position
         *  @return the index of the first previous character slot that is open
         *  @param  nCurrentIndex [in] the index to begin searching from
         */
        private int FindNextAvailableIndex ( int nCurrentIndex )
        {
            int nRetVal = nCurrentIndex;

            do
            {
                nRetVal = (nRetVal + 1) % 6;

            }while (IsCharacterSelected(nRetVal));

            return nRetVal;
        }

        #endregion
    }
}
