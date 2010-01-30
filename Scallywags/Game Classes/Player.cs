using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System;

namespace Scallywags
{
    /** @class  Player
     *  @brief  a player that controls ships
     */
    public class Player:AI
    {
        string m_Name;
        ChestOpen m_ChestLid;
        Statistics m_PlayerStats = new Statistics();

        /// <summary>
        /// Get/set function to track each individual player's stats
        /// </summary>
        public Statistics PlayerStats
        {
            get { return m_PlayerStats; }
            set { m_PlayerStats = value; }
        }

        /** @prop   float Color
         *  @brief  Gets and sets the Players Color
         */
        public Color Color   
        {
            get
            {
                return m_PlayerColor;
            }
            set
            {
                m_PlayerColor = value;
            }
        }

        public ChestOpen ChestLid
        {
            get
            {
                return m_ChestLid;
            }
            set
            {
                m_ChestLid = value;
            }
        }

        public Coin PortCoin
        {
            get
            {
                return m_PortCoin;
            }
            set
            {
                m_PortCoin = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /** @prop   float InitialMovePoints
         *  @brief  Gets The starting move points of the player
         */
        public float InitialMovePoints
        {
            get
            {
                return Settings.MOVE_POINTS + (Settings.BOAT_BONUS * m_totalAvailableShips);
            }
        }

        #region Functionality
        
        /*  @fn Player
         *  @brief The constructor for the player.
         *  @param playerNum [in] The index number of the selected ship
         */
        public Player(int playerNum, bool AmIAI , Color color, Model TargetModel, Model CoinModel ,World theWorld, Player[] players,Town town,Object3D AITargetModel):base(theWorld,players,town)
        {
            m_playerShips = new Ship[3];
            m_currentShip = 0;
            m_totalAvailableShips = 0;

            m_playerNum = playerNum;
            m_totalGold = 0;
            m_fMovePoints = 100.0f;
            m_nCharacterID = 0;
            m_bActive = AmIAI;
            m_PlayerColor = color;
            m_PortCoin = new Coin(CoinModel);
            m_PortCoin.Y = 5.0f;
            m_PortCoin.Exists = false;
            theWorld.AddGameObject(m_PortCoin, true);
            m_world = theWorld;

            m_targetModel = TargetModel;
            m_AITargetModel = AITargetModel;
            m_ShipTargetters = new List<Targetter>();
            joe = new Random(playerNum);

            switch (playerNum) {
                case 0:                     //Top left
                    m_homeLoc.X = 225;
                    m_homeLoc.Y = 225;
                    m_PortCoin.X = 225;
                    m_PortCoin.Z = 225;
                    break;
                case 1:                     //Top Right
                    m_homeLoc.X = -225;
                    m_homeLoc.Y = 225;
                    m_PortCoin.X = -225;
                    m_PortCoin.Z = 225;
                    break;
                case 2:                     //Bottom Right
                    m_homeLoc.X = -225;
                    m_homeLoc.Y = -225;
                    m_PortCoin.X = -225;
                    m_PortCoin.Z = -225;
                    break;
                case 3:                     //Bottom Left
                    m_homeLoc.X = 225;
                    m_homeLoc.Y = -225;
                    m_PortCoin.X = 225;
                    m_PortCoin.Z = -225;
                    break;
            }

        }
        /*  @fn addShipRef
         *  @brief stores a refrence to the ship. Should be limited to three later, just in case
         *  @param aShip [in] The ship that will br referenced.
         */
        public void addShipRef(Ship aShip, World theWorld){
            m_playerShips[m_totalAvailableShips] = aShip;
            m_GoldModel = aShip.Gold.TheModel;
            m_playerShips[m_totalAvailableShips].moveTo(m_homeLoc.X, m_homeLoc.Y);
            //m_playerShips[m_totalAvailableShips].Yaw = (float)(Math.PI*5/4) - (float)(Math.PI/2) *playerNumber;

            ForceShipToFacePoint(m_playerShips[m_totalAvailableShips], Vector2.Zero);

            m_playerShips[m_totalAvailableShips].ControllingPlayer = playerNumber;

            Targetter Target = new Targetter(m_targetModel, theWorld.WaveLocations, false);
            Target.Colour = m_PlayerColor;
            Target.LockShip(aShip);
            m_ShipTargetters.Add(Target);
            theWorld.AddTargetter(Target, true);

            m_playerShips[m_totalAvailableShips].InitGold(m_GoldModel);

            theWorld.AddGameObject(m_playerShips[m_totalAvailableShips].Gold, true);

            m_totalAvailableShips++;
        }

        /*  @fn SetCurShip
        *  @brief Sets the Current Ship
        */
        public void SetCurShip( int ShipIndex)
        {
            m_currentShip = ShipIndex;
        }

        /* @fn CheckAllDisabled
        *  @brief Checks to see if all ships are disabled
        */
        public bool CheckAllDisabled()
        {
            for (int CurrentCheck = 0; CurrentCheck < m_totalAvailableShips; CurrentCheck++)
            {
                if (Ships[CurrentCheck].isDisabled == false)
                    return false;
            }
            return true;
        }

        

        /*  @fn killShip
         *  @brief kills the selected ship
         *  @param shipNum [in] The index number of the selected ship
         */
        public void killShip(int shipNum)
        {
            m_playerShips[Math.Min(shipNum, 2)].disableShip();
        }

        /*  @fn EnableShips
        *  @brief Enables the Ships that are ready to be enabled
        */
        public List<Ship> EnableShips()
        {
            List<Ship> EnabledList = new List<Ship>();
            for (int CurrentCheck = 0; CurrentCheck < m_totalAvailableShips; CurrentCheck++)
            {
                if (Ships[CurrentCheck].PrepEnable == true)
                {
                    Ships[CurrentCheck].PrepEnable = false;
                    {
                        Ships[CurrentCheck].EnableShip();
                        EnabledList.Add(Ships[CurrentCheck]);
                    }
                }
            }
            return EnabledList;
        }

        /*  @fn PrepEnableShips
        *  @brief Tells the player which ships to enable next turn
        */
        public List<Ship> PrepEnableShips()
        {
            List<Ship> PrepEnabledList = new List<Ship>();
            for (int CurrentCheck = 0; CurrentCheck < m_totalAvailableShips; CurrentCheck++)
            {
                if (Ships[CurrentCheck].isDisabled == true)
                {
                    Ships[CurrentCheck].PrepEnable = true;
                    PrepEnabledList.Add(Ships[CurrentCheck]);
                }
            }
            return PrepEnabledList;
        }

        /** @fn     void Clear
         *  @brief  Clears out the ships
         */
        public void Clear() {
            m_playerShips = new Ship[3];
            m_totalAvailableShips = 0;
        }

        public void Draw(GraphicsDevice device,Matrix matView, Matrix ProjectionMatrix,Vector3 theCam){
            /*
            if (IsAI) {
                if (currentAction.Action != ActionStates.None.Action) {

                    if (currentAction.Action == ActionStates.Events.MoveTo) {
                        m_AITargetModel.Location = new Vector3(currentAction.MoveTo.X, 10, currentAction.MoveTo.Y);
                        m_AITargetModel.Draw(device, matView, ProjectionMatrix, theCam);
                    }
                    else if (currentAction.Action == ActionStates.Events.FireAt) {
                        m_AITargetModel.Location = new Vector3(currentAction.FireAt.X, 10, currentAction.FireAt.Y);
                        m_AITargetModel.Draw(device, matView, ProjectionMatrix, theCam);
                    }
                }
            }
            */

        }

        #endregion
    }
}
