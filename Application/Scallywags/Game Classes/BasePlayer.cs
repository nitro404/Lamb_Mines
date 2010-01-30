using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags {
    public class BasePlayer {

        protected Port m_playerPort;
        protected Ship[] m_playerShips;
        protected Model m_targetModel;
        protected Model m_GoldModel;
        protected List<Targetter> m_ShipTargetters;
        protected int m_currentShip;
        protected int m_totalAvailableShips;
        protected int m_playerNum;
        protected int m_totalGold;
        protected Vector2 m_homeLoc;
        protected float m_fMovePoints;          ///< Total amount of moves the player has
        protected Color m_PlayerColor;
        protected Coin m_PortCoin;  

        protected int m_nCharacterID;           ///< The ID of the character
        protected bool m_bActive;                ///< Is this player active?
        protected bool m_isAI, m_hasShot;                 ///<Is this player controlled by the AI
        protected Object3D m_AITargetModel;
        protected World m_world;

        #region Properties

        public bool hasShot
        {
            get
            {
                return m_hasShot;
            }
            set
            {
                m_hasShot = value;
            }
        }

        /*  @prop totalShips
         *  @brief The current number of ships in existance
         */
        public int totalShips {
            get {
                return m_totalAvailableShips;
            }
            set {
                m_totalAvailableShips = value;
            }
        }
        /*  @prop playerNum
         *  @brief The number of the player
         */
        public int playerNumber {
            get {
                return m_playerNum;
            }
        }
        /*  @prop CurrentShip
        *  @brief The Currently Selected Ship
        */
        public Ship CurrentShip {
            get 
            {
                return m_playerShips[m_currentShip];
            }
            set
            {
                m_playerShips[m_currentShip] = value;
            }
        }
        public int CurrentShipIndex {
            get 
            {
                return m_currentShip;
            }
            set
            {
                m_currentShip = value;
            }

        }
        /*  @prop totalGold
         *  @brief The total gold or score of the player. cannot be less than 0.
         */
        public int totalGold {
            get {
                return m_playerPort.Coins;
                //return m_totalGold; //this was never set.
            }
            set {
                m_totalGold = Math.Max(0, value);
            }
        }
        /*  @prop Ships
        *   @brief The Player's Ships
        */
        public Ship[] Ships {
            get {
                return m_playerShips;
            }
        }

        /** @prop   float MovePoints
         *  @brief  Gets and sets the amount of move points
         */
        public float MovePoints {
            get {
                return m_fMovePoints;
            }
            set {
                m_fMovePoints = value;
            }
        }

        /** @prop   Port Port
        *  @brief  Gets and sets the Players port;
        */
        public Port Port {
            get {
                return m_playerPort;
            }
            set {
                m_playerPort = value;
                m_playerPort.Location = new Vector3(m_homeLoc.X, 1.0f, m_homeLoc.Y);
                m_playerPort.Owner = m_playerNum;
            }
        }

        /** @prop   int CharacterID
         *  @brief  the ID of the character the player chose
         */
        public int CharacterID {
            get {
                return m_nCharacterID;
            }
            set {
                m_nCharacterID = value;
            }
        }

        /** @prop   bool Active
         *  @brief  is this player active?
         */
        public bool Active {
            get {
                return m_bActive;
            }
            set {
                m_bActive = value;
            }
        }
        /** @prop   bool IsAI
         *  @brief  is this player controlled by the AI?
         */
        public bool IsAI {
            get {
                return m_isAI;
            }
            set{
                m_isAI = value;
            }
        }

        #endregion


        /*  @fn buyShip
         *  @brief adds a new ship to the list based of the first ship.
         *  @return bool True if a ship has been created false if there is no more room
         */
        public bool buyShip(World theWorld) {
            if (m_totalAvailableShips < 3)//if there are less than 3 ships.
            {
                if (Port.Coins > 0) {
                    m_playerShips[m_totalAvailableShips] = new Ship(m_playerShips[0].TheModel, m_GoldModel, m_playerNum, m_playerShips[0].Diffuse);
                    m_playerShips[m_totalAvailableShips].Color = m_PlayerColor;
                    theWorld.AddShip(m_playerShips[m_totalAvailableShips]);
                    m_playerShips[m_totalAvailableShips].spawnAtHome(m_homeLoc.X, m_homeLoc.Y);
                    m_playerShips[m_totalAvailableShips].InitGold(m_GoldModel);
                    //m_playerShips[m_totalAvailableShips].Yaw = (float)(Math.PI * 5 / 4) - (float)(Math.PI / 2) * playerNumber;
                    m_playerShips[m_totalAvailableShips].ControllingPlayer = playerNumber;
                    ForceShipToFacePoint(m_playerShips[m_totalAvailableShips], Vector2.Zero);

                    Targetter Target = new Targetter(m_targetModel, theWorld.WaveLocations, false);
                    Target.Colour = m_PlayerColor;
                    Target.LockShip(m_playerShips[m_totalAvailableShips]);
                    m_ShipTargetters.Add(Target);
                    theWorld.AddTargetter(Target, true);

                    theWorld.AddGameObject(m_playerShips[m_totalAvailableShips].Gold, true);

                    m_totalAvailableShips++;
                    Port.decrementCoins(1);
                    return true;
                }
            }
            return false;
        }

        /*  @fn NextShip
         *  @brief Sets Current Ship to the Next Ship
         */
        public void NextShip() {
            m_currentShip = (m_currentShip + 1) % m_totalAvailableShips;

            if (m_playerShips[m_currentShip].isDisabled == true) {
                NextShip();
            }
        }
        /*  @fn PrevShip
         *  @brief Sets Current Ship to the Previous Ship
         */
        public void PrevShip() {
            m_currentShip = (m_currentShip + m_totalAvailableShips - 1) % m_totalAvailableShips;

            if (m_playerShips[m_currentShip].isDisabled == true) {
                PrevShip();
            }
        }

        /*  @fn moveForward
         *  @brief This will move the current ship forward a little and check for collision
         *  @param speed [in] this is the overall speed of the boat
         *  @param collision [in] this is a reference to the collision class
         */
        public void moveForward(float speed, Collision collision) {
            m_playerShips[m_currentShip].moveForward(speed);

            if (Settings.USE_COLLISION) {
                //Move the ship if any collision is detected
                Vector2 vMoveTo = collision.DoesCollide(
                    m_playerShips[m_currentShip].PreviousLocation2D,
                    m_playerShips[m_currentShip].CurrentLocation2D);

                //m_playerShips[m_currentShip].moveToLocation(vMoveTo);
                m_fMovePoints -= m_playerShips[m_currentShip].moveToLocation(vMoveTo) * speed*Settings.SHIP_MOVE_COST;
            }
        }

        /** @fn      void ForceShipToFacePoint( Ship ship, Vector2 vPoint )
         *  @brief   make a ship face a particular point
         *  @param  ship [in] the ship to alter
         *  @param  vPoint [in] the point to face on the X/Z plane
         */
        public void ForceShipToFacePoint(Ship ship, Vector2 vPoint) {
            float fXDisp = vPoint.X - ship.X;
            float fYDisp = vPoint.Y - ship.Z;

            //float fXDisp = ship.X - vPoint.X;
            //float fYDisp = ship.Z - vPoint.Y;


            float fOrientation = (float)Math.Asin(fYDisp / (Math.Sqrt(fYDisp * fYDisp + fXDisp * fXDisp)));
            //float fOrientation = (float)(Math.Atan2(fYDisp, fXDisp));

            if (Math.Sign(fXDisp) == -1) {
                ship.shipHeading = fOrientation;
            }
            else {
                ship.shipHeading = MathHelper.Pi-fOrientation;
            }
        }

        /*  @fn void TurnShipToFacePoint
         *  @brief This function will turn the ship slightly towards the desired target location.
         *  @param Ship The ship to move.
         *  @param Vector2 The location to turn to.
         *  @param float The total distance you can turn this frame
         */
        public void TurnShipToFacePoint(Ship currentShip, Vector2 targetLocation,float turnSpeed){

            float fXDisp = targetLocation.X - currentShip.X;
            float fYDisp = targetLocation.Y - currentShip.Z;
            float desiredTurnRad, finalRot;

            //float fXDisp = ship.X - vPoint.X;
            //float fYDisp = ship.Z - vPoint.Y;


            float fOrientation = (float)Math.Asin(fYDisp / (Math.Sqrt(fYDisp * fYDisp + fXDisp * fXDisp)));

            if (Math.Sign(fXDisp) == -1){
                desiredTurnRad = fOrientation;
            }
            else{
                desiredTurnRad = MathHelper.Pi - fOrientation;
            }
            finalRot = desiredTurnRad;
            finalRot -= currentShip.shipHeading;
            if (finalRot > MathHelper.Pi) {
                finalRot -= MathHelper.TwoPi;
            }
            else if (finalRot < -MathHelper.Pi)
            {
                finalRot += MathHelper.TwoPi;
            }
            if (finalRot < 0 ) {
                //finalRot -= MathHelper.TwoPi;
                //now turn right.
                if (finalRot > -turnSpeed) {
                    currentShip.shipHeading = desiredTurnRad;
                }
                else{
                    currentShip.shipHeading -= turnSpeed;
                }
            }
            else {//turn left
                if (finalRot < turnSpeed) {
                    currentShip.shipHeading = desiredTurnRad;
                }
                else {
                    currentShip.shipHeading += turnSpeed;
                }
            }
            if ((desiredTurnRad + turnSpeed) > currentShip.shipHeading && (desiredTurnRad - turnSpeed) < currentShip.shipHeading)
            {
                currentShip.shipHeading = desiredTurnRad;
            }
            
        }

    }
}
