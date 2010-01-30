using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Scallywags
{
    /** @class  ScallyWagsApp
     *  @brief  the main application class, a type of modular app
     */  
    public class ScallyWagsApp : XNAModularApp
    {
        public const int NUM_CHARACTERS = 6;

        private GameSettings            m_settings;             ///< The game's current *user selected* settings
                    
        /** @prop   UserSettings
         *  @brief  the current game settings
         */
        public GameSettings TheGameSettings
        {
            get
            {
                return m_settings;
            }
        }
               
        /** @fn     CharacterInfo GetCharacter( int nID )
         *  @brief  get a character based on an ID
         *  @return the information about the requested character
         *  @param  nID [in] the ID of the requested character
         */
        public CharacterInfo GetCharacter( int nID, bool bAI )
        {
            return new CharacterInfo(nID, bAI);
        }

        /** @fn     ScallyWagsApp( XNAModule startModule )
         *  @brief  constructor
         *  @param  startModule [in] the module the application will begin running right away
         */
        public ScallyWagsApp( XNAModule startModule )
            : base( startModule )
        {
            //Set some default game settings.
            m_settings = new GameSettings();

            m_settings.NumPlayers   = 4;
            m_settings.TurnLimit    = -1;
           
            m_settings.Characters[ 0 ] = GetCharacter( 5, false );
            m_settings.Characters[ 1 ] = GetCharacter( 1, false );
            m_settings.Characters[ 2 ] = GetCharacter( 2, false );
            m_settings.Characters[ 3 ] = GetCharacter( 3, false );

            m_settings.GoldLimit            = 3;
            m_settings.EndOnTotalPillage    = false;

            IsFixedTimeStep = false;

            //Temp
            SoundPlayer.SoundEffectVolume = 0.5f;
        }
    }

    public class CharacterInfo
    {
        public int      nCharacterID;
        public string   strName;
        public Color    color;
        public string   strPortrait;

        public bool     bAiControlled;

        public CharacterInfo(int nID, bool bAI)
        {
            nCharacterID    = nID;
            bAiControlled   = bAI;
            
            switch( nID )
            {
                case 0:
                    color       = Color.Blue;
                    strName     = "Duke Wallace";
                    strPortrait = "HUD_ppBlue";
                    break;
                case 1:
                    color = Color.Gray;
                    strName = "Brutus Irongut";
                    strPortrait = "HUD_ppBlack";
                    break;
                case 2:
                    color = Color.Red;
                    strName = "Jane Scarlet";
                    strPortrait = "HUD_ppRed";
                    break;
                case 3:
                    color = Color.Indigo;
                    strName = "Indigo Montoya";
                    strPortrait = "HUD_ppIndigo";
                    break;
                case 4:
                    color = Color.Orange;
                    strName = "Amber Squall";
                    strPortrait = "HUD_ppAmber";
                    break;
                case 5:
                    color = Color.Green;
                    strName = "Moldy O'Malley";
                    strPortrait = "HUD_ppMoldy";
                    break;
            }
        }
    }
}
