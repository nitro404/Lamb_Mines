using System.Collections.Generic;

namespace Scallywags
{
    /** @class  GameSettings
     *  @brief  the selected game settings
     */
    public class GameSettings
    {
        #region DATA_MEMBERS

        private int                 m_nNumPlayers;      ///< The number of players in the game
        private CharacterInfo[]     m_vCharacters;      ///< The active character information
        private int                 m_nTurnLimit;       ///< The game's turn limit
        private int                 m_nGoldLimit;       ///< The number of gold pieces required to win the game      
        private bool                m_bTotalPillage;    ///< Does the game end when the island is out of gold?

        #endregion

        #region PROPERTIES

        /** @prop   int NumPlayers
         *  @brief  the number of players in the game
         */
        public int NumPlayers
        {
            get
            {
                return m_nNumPlayers;
            }
            set
            {
                m_nNumPlayers = value;
            }
        }

        /** @prop   Character[] Characters
         *  @brief  the ids of the selected characters
         */
        public CharacterInfo[] Characters
        {
            get
            {
                return m_vCharacters;
            }
        }

        /** @prop   int TurnLimit
         *  @brief  the number of turns before a winner is decided
         */
        public int TurnLimit
        {
            get
            {
                return m_nTurnLimit;
            }
            set
            {
                m_nTurnLimit = value;
            }
        }

        /** @prop   int GoldLimit
         *  @brief  the number of gold pieces required to win
         */
        public int GoldLimit
        {
            get
            {
                return m_nGoldLimit;
            }
            set
            {
                m_nGoldLimit = value;
            }            
        }

        /** @prop   bool EndOnTotalPillage
         *  @brief  game end condition that says the game is over when the island has no more gold
         */
        public bool EndOnTotalPillage
        {
           get
           {
               return m_bTotalPillage;
           }
           set
           {
               m_bTotalPillage = value;
           }
        }

        #endregion

       /** @fn     GameSettings()
         *  @brief  constructor
         */
        public GameSettings()
        {
            m_nNumPlayers   = 0;
            m_vCharacters = new CharacterInfo[4];

            //m_vCharacters[0] = new CharacterInfo(0, false);
            //m_vCharacters[1] = new CharacterInfo(1, false);
            //m_vCharacters[2] = new CharacterInfo(2, false);
            //m_vCharacters[3] = new CharacterInfo(3, false);

            m_nTurnLimit    = 0;
            m_nGoldLimit    = 0;
            m_bTotalPillage = false;
        }

        public void Reset()
        {
            m_nNumPlayers = 0;
            m_vCharacters = new CharacterInfo[4];

            //m_vCharacters[0] = new CharacterInfo(0, false);
            //m_vCharacters[1] = new CharacterInfo(1, false);
            //m_vCharacters[2] = new CharacterInfo(2, false);
            //m_vCharacters[3] = new CharacterInfo(3, false);

            m_nTurnLimit = 0;
            m_nGoldLimit = 0;
            m_bTotalPillage = false;
        }
    }
}
