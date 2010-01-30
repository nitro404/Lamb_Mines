using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Scallywags
{
    /** @class  TownPlayer
     *  @brief  class to describe the AI player controlling the town
     */
    public class TownPlayer
    {
        const float DELAY_TIME  = 2.0f;

        private World   m_world;            ///< The world the player's town belongs to
        private Camera  m_camera;           ///< The game camera.. to see if the camera has stopped moving on a tower
                                            
        private int     m_nCurrentTower;    ///< The tower that's currently attacking
        private bool    m_bActive;          ///< Is the town player active?
        private float   m_fDelayTimer;      ///< Timer used to pause the town turn                   
                  
        enum TownPlayerState
        {
            TPS_FIRING,
            TPS_WAITING,
            TPS_FIND_TARGET
        }
                                                                                        
        /** @prop   Active
         *  @brief  is the town player active?
         */
        public bool Active
        {
            get
            {
                return m_bActive;
            }
            set
            {
                m_bActive = value;
            }
        }

        /** @prop   CurrentTower
         *  @brief  the tower the AI is controlling
         */
        public Tower CurrentTower
        {
            get
            {
                Tower tower = m_world.TheTown.Towers[ m_nCurrentTower ];
                return tower;
            }
        }

        /** @fn     TowerPlayer( World world )
         *  @brief  constructor
         *  @param  world [in] the world the player's town belongs to
         *  @param  camera [in] the game camera
         */
        public TownPlayer( World world, Camera camera )
        {
            m_world         = world;
            m_camera        = camera;
            m_nCurrentTower = 0;
            m_bActive       = false;
            m_fDelayTimer   = 0;
        }

        /** @fn     void StartTurn()
         *  @brief  start the TownPlayer turn
         *  @return true if there is a need for the AI turn, false otherwise
         */
        public bool StartTurn()
        {
            m_bActive       = false;
            m_nCurrentTower = -1;
            m_fDelayTimer   = 0;

            NextTower();

            return m_bActive;
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the town player
         *  @param  fElapsedTime [in] the time since the last frame, in seconds
         */
        public void Update(float fElapsedTime, SoundManager soundPlayer )
        {
            Tower currentTower = m_world.TheTown.Towers[m_nCurrentTower];

            //The cannonball has stopped, and this tower has already fired... tower's turn is over
            if ( m_world.TheCannonBall.Firing == false && currentTower.CanFire == false)
            {
                m_fDelayTimer += fElapsedTime;
            }
            else if ( m_camera.InPosition && m_world.TheCannonBall.Firing == false && currentTower.CanFire )
            {
                //The cannonball isn't moving, the camera is ready, and this tower can still fire.. pick a target and shoot

                //Select the best target
                List<Ship> lstTargets = GetLiveShipsInRange(currentTower);

                //the closest target
                Ship target = null;
                float fNearestSquared = -1;

                foreach (Ship ship in lstTargets)
                {
                    Vector3 vDisp = ship.Location - currentTower.Location;
                    float fDistanceSquared = vDisp.LengthSquared();

                    if (fNearestSquared == -1 || fDistanceSquared < fNearestSquared)
                    {
                        fNearestSquared = fDistanceSquared;
                        target = ship;
                    }
                }

                //Adjust the accuracy by distance
                Vector3 vAccuracyOffset = Vector3.Zero;
                Random rand = new Random();

                float fDistancePercent = fNearestSquared / (Settings.TOWER_RANGE * Settings.TOWER_RANGE);

                //Randomly miss by up to 25 in both X/Z directions
                vAccuracyOffset.X += ((float)rand.NextDouble() * 50.0f - 25.0f) * fDistancePercent;
                vAccuracyOffset.Z += ((float)rand.NextDouble() * 50.0f - 25.0f) * fDistancePercent;

                //Fire at the first one for now
                m_world.TheCannonBall.FireCannonAtTarget( currentTower.Name, currentTower.Location, target.Location + vAccuracyOffset );
                soundPlayer.PlayCue("CannonShot");

                currentTower.CanFire = false;
            }

            ///////////////////////////////
            //End the tower turn after a moment
            if( m_fDelayTimer > DELAY_TIME )
            {
                m_fDelayTimer = 0;

                //Cannon ball is finished.
                m_bActive = false;

                //Advance to the next tower..
                NextTower();
            }
        }

        /** @fn     List< Ship > GetLiveShipsInRange( Tower tower )
         *  @brief  get the list of ships in the range of a particular tower
         *  @param  tower [in] the tower whose range to check
         */
        List< Ship > GetLiveShipsInRange( Tower tower )
        {
            List<Ship > lstShips = new List<Ship>();
           
            foreach( Ship ship in m_world.Ships )
            {
                if( ship.isDisabled == false )
                {
                    if (tower.IsInRange(ship))
                    {
                        lstShips.Add( ship );
                    }
                }
            }

            return lstShips;
        }

        /** @fn     void NextTower()
         *  @brief  find the next available tower
         */
        void NextTower()
        {
            m_nCurrentTower++;

            //Find the first live tower that has ships to fire at
            Tower[] towers = m_world.TheTown.Towers;
            for (int i = m_nCurrentTower; i < towers.Length; ++i)
            {
                if (towers[i].IsAlive)
                {
                    List< Ship > lstShipsInRange = GetLiveShipsInRange( towers[ i ] );

                    if( lstShipsInRange.Count > 0 )
                    {
                        //Mark the town player as active
                        m_bActive = true;
                        m_nCurrentTower = i;
                        towers[i].CanFire = true;

                        m_camera.OrientCamera( -towers[i].Yaw );
                                                
                        break;
                    }
                }
            }
        }
    }
}
