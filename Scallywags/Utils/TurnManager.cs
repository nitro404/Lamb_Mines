using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Scallywags
{
    public class TurnManager
    {
        #region Data Members

        private float           m_Timer;
        private int             m_Turn;
        private bool            m_FireMode;
        //private bool            m_Cannon;
        private Targetter       Target;
        private InputManager    m_InputManager;
        private World           m_world;
        private Collision       m_collision;
        private Player[]        m_Players;
        private int             m_TotalTurns;
        private bool            m_isPaused;
        private float           m_PauseTimer;
        private bool            m_moving;
        bool m_debugAI;
        private GameModule      m_ParentModule;  //To be able to access creation interfaces
        List<Ship>              m_EnabledList;      //A List of all recently Enabled Ships;
        List<Ship>              m_PrepEnabledList;      //A List of all recently Enabled Ships;
        CannonBall              m_CoinToss;
        private bool            m_bEndTurn;
        Fireworks               m_cFireworks;

        private HUD     m_hud;      ///< The game hud
        private Camera  m_Camera;   //So the turn manager knows about the camera

        private Model   m_mdlCoin;  ///< storage for the coin
                                    
        private Arc     m_arc;      ///< The firing arc
        
        private TownPlayer  m_townPlayer;   ///< The Ai controlling the town                         

        //temporary to show the AI is working
        private bool initAI = true; //a boolean to determine whether or not to run BeginAI

        #endregion

        #region Properties

        public Targetter Targetter
        {
            get
            {
                return Target;
            }
        }

        /** @prop   TotalTurns
         *  @brief  The number of full turns that have passed
         */
        public int TotalTurns
        {
            get
            {
                return m_TotalTurns;
            }
        }

        /** @prop   Players
         *  @brief  the array of players in the game
         */
        public Player[] Players
        {
            get
            {
                return m_Players;
            }
        }
        
        /** @prop   Turn
         *  @brief  Who's Turn it is based on Controller Indices
         */
        public int Turn
        {
            get
            {
                return m_Turn;
            }
            set
            {
                m_Turn = value;
            }
        }

        /** @prop   FireMode
         *  @brief  If the Player/Game is in Firing Mode
         */
        private bool FireMode
        {
            get
            {
                return m_FireMode;
            }
            set
            {
                m_FireMode = value;
            }
        }

        /** @prop   MovePoints
         *  @brief  number of remaining move points
         *  @author Mike - Just in case you wanted move to remain private get/set, this one is read only.
         */
        public float MovePoints
        {
            get
            {
                return m_Players[ m_Turn ].MovePoints;
            }
        }

        /** @prop   CurrentShip
         *  @brief  Gets a reference to the current ship
         */
        public Ship CurrentShip
        {
            get
            {
                return m_Players[ m_Turn ].CurrentShip;
            }
        }

        /// <summary>
        /// Is the end turn popup visible?
        /// </summary>
        public bool EndTurn
        {
            get
            {
                return m_bEndTurn;
            }
        }

        /** @prop   CameraTarget
         *  @brief  the current game object for the camera to follow
         */
        public Vector3 CameraTarget
        {
            get
            {
                if (m_townPlayer.Active)
                {
                    Vector3 vLoc = m_townPlayer.CurrentTower.Location;
                    m_Camera.Radius = 100.0f;
                    m_Camera.Height = Settings.CAMERA_HEIGHT * 2;
                    //vLoc.Y += 40.0f;
                    return vLoc;
                }
                else if (FireMode == true)
                {
                    //return (Target.Location + Selected.Location) / 2; //Average Distance
                    Vector3 Targets = (Target.Location + m_Players[Turn].CurrentShip.Location) / 2;
                    //Targets.Y = ((Target.Location - m_Players[Turn].CurrentShip.Location).Length() / 100 - 25);
                    //m_Camera.Height = Settings.CAMERA_HEIGHT + m_Camera.Radius/5;
                    m_Camera.Radius = ((Target.Location - m_Players[Turn].CurrentShip.Location).Length()* 1.25f);
                    m_Camera.Height = Settings.CAMERA_HEIGHT * 2;
                    return Target.Location;
                    //return Targets;
                }
                else
                {
                    m_Camera.Radius = ((Target.Location - m_Players[Turn].CurrentShip.Location).Length()* 0.95f);
                    m_Camera.Height = Settings.CAMERA_HEIGHT + m_Camera.Radius / 5;
                    return m_Players[m_Turn].CurrentShip.Location;    
                }
            }
        }


        /** @prop   ParentModule
         *  @brief  for the creation of particle systems from the Turn Manager
         */
        public GameModule ParentModule
        {
            get
            {
                return m_ParentModule;
            }
        }

        /** @prop   IsTownTurn
         *  @brief  is it the Town Ai turn?
         */
        public bool IsTownTurn
        {
            get
            {
                return m_townPlayer.Active;
            }
        }

        #endregion

        #region Functions

        /** @fn     TurnManager()
         *  @brief  (Constructor) Sets up at Player 1 with 1 shot and full movement in movement mode
         */
        public TurnManager( World theWorld)
        {
            m_Players   = null;
            m_world     = theWorld;
            m_hud       = null;
            m_townPlayer = null;
        }

        /** @fn     Init()
        *  @brief  Sets up default parameters and brings in the Input manager
        */
        public void Init(InputManager Input, Player[] players, Targetter target, Collision collision,
            Camera cam, HUD hud, Model coin, Arc FiringArc, Model CannonBall, GameModule ParentModule)
        {
            m_Players       = players;
            m_ParentModule  = ParentModule;
            
            for (int i = 0; i < m_Players.Length; i++)
                m_Players[i].SetCurShip(0);

            //set everything to its default state
            m_InputManager  = Input;
            Target          = target;
            Target.Colour   = Color.DarkRed;
            m_TotalTurns    = 1;
            m_moving = false;
            m_debugAI = false;
            m_CoinToss = new CannonBall(coin);
            m_world.AddCoinToss(m_CoinToss);
            m_cFireworks = new Fireworks(ParentModule);

            m_collision     = collision;

            //Random rand = new Random();
            Turn            = 0;//rand.Next(0, 3);

            m_Camera    = cam;
            m_hud       = hud;

            m_mdlCoin = coin;

            m_world.TheCannonBall = new CannonBall(CannonBall);

            m_arc = FiringArc;

            m_townPlayer = new TownPlayer( m_world, m_Camera );

            m_EnabledList = new List<Ship>();
            m_PrepEnabledList = new List<Ship>();

            //Initializes Player Stats for each player
            for (int i = 0; i < m_Players.Length; i++)
            {
                if (Settings.DISABLE_AI) {
                    m_Players[i].IsAI = false;
                }
                else if (Settings.ALL_AI) {
                    m_Players[i].IsAI = true;
                }
                else {
                    m_Players[i].IsAI = m_ParentModule.TheGameSettings.Characters[i].bAiControlled;
                }
            }

            m_bEndTurn = false;

            //Set up the first turn
            StartPlayerTurn();

            for (int i = 0; i < m_Players.Length; i++)
            {
                    GamePad.SetVibration((PlayerIndex)i, 0.0f, 0.0f);
            }
        }

        /** @fn     void HandleStandardInput( float fElapsedTime )
         *  @brief  handle the game input while not in fire mode
         *  @param  fElapsedTime [in] the time elapsed since the previous frame, in seconds.
         */
        private void HandleStandardInput(float fElapsedTime, SoundManager soundPlayer)
        {
            //Check if the game is over
            bool bGameOver = m_ParentModule.GameOver;
            
            if( m_bEndTurn == false )
            {
                //Moving Forward
                if (m_InputManager.IsKeyDown(Keys.W) || m_InputManager.IsButtonDown(Turn ,Buttons.A) || (m_InputManager.CheckLStick(Turn).Y > Settings.DEAD_ZONE))
                {
                    if (MovePoints > 0)
                    {
                        m_moving = true; //Boat is moving

                        if (m_InputManager.IsKeyDown(Keys.LeftShift))
                        {
                            m_Players[Turn].moveForward(2 * Settings.SHIP_MOVE_SPEED * fElapsedTime, m_collision);
                        }
                        else
                        {
                            if (m_InputManager.CheckLStick(Turn).Y > Settings.DEAD_ZONE)
                                m_Players[Turn].moveForward(Settings.SHIP_MOVE_SPEED * m_InputManager.CheckLStick(Turn).Y * fElapsedTime, m_collision);
                            else
                                m_Players[Turn].moveForward(Settings.SHIP_MOVE_SPEED * fElapsedTime, m_collision);
                        }
                        m_Players[Turn].PlayerStats.addDistanceTravelled();
                    }
                }
                else
                {
                   
                    m_Players[Turn].MovePoints -= Settings.IDLE_COST;
                }

                //Turning Left
                if (m_InputManager.IsKeyDown(Keys.A) || (m_InputManager.CheckLStick(Turn).X < -Settings.DEAD_ZONE))
                {
                    if (m_InputManager.CheckLStick(Turn).X < -Settings.DEAD_ZONE)
                    {
                        m_Players[Turn].CurrentShip.turnLeft(Settings.SHIP_TURN_SPEED * fElapsedTime * -m_InputManager.CheckLStick(Turn).X);
                        m_Camera.Rotate(m_InputManager.CheckLStick(Turn).X * Settings.SHIP_TURN_SPEED * fElapsedTime);
                        if (m_InputManager.IsKeyDown(Keys.LeftShift))
                        {
                            m_Players[Turn].CurrentShip.turnLeft(Settings.SHIP_TURN_SPEED * fElapsedTime * -m_InputManager.CheckLStick(Turn).X);
                            m_Camera.Rotate(m_InputManager.CheckLStick(Turn).X * Settings.SHIP_TURN_SPEED * fElapsedTime);
                        }
                    }
                    else
                    {
                        m_Players[Turn].CurrentShip.turnLeft(Settings.SHIP_TURN_SPEED * fElapsedTime);
                        m_Camera.Rotate(-1 * Settings.SHIP_TURN_SPEED * fElapsedTime);
                        if (m_InputManager.IsKeyDown(Keys.LeftShift))
                        {
                            m_Players[Turn].CurrentShip.turnLeft(Settings.SHIP_TURN_SPEED * fElapsedTime);
                            m_Camera.Rotate(-1 * Settings.SHIP_TURN_SPEED * fElapsedTime);
                        }
                    }
                }

                //Turning Right
                if (m_InputManager.IsKeyDown(Keys.D) || (m_InputManager.CheckLStick(Turn).X > Settings.DEAD_ZONE))
                {
                    if (m_InputManager.CheckLStick(Turn).X > Settings.DEAD_ZONE)
                    {
                        m_Players[Turn].CurrentShip.turnRight(Settings.SHIP_TURN_SPEED * fElapsedTime * m_InputManager.CheckLStick(Turn).X);
                        m_Camera.Rotate(Settings.SHIP_TURN_SPEED * fElapsedTime * m_InputManager.CheckLStick(Turn).X);
                        if (m_InputManager.IsKeyDown(Keys.LeftShift))
                        {
                            m_Players[Turn].CurrentShip.turnRight(Settings.SHIP_TURN_SPEED * fElapsedTime * m_InputManager.CheckLStick(Turn).X);
                            m_Camera.Rotate(Settings.SHIP_TURN_SPEED * fElapsedTime * m_InputManager.CheckLStick(Turn).X);
                        }
                    }
                    else
                    {
                        m_Players[Turn].CurrentShip.turnRight(Settings.SHIP_TURN_SPEED * fElapsedTime);
                        m_Camera.Rotate(Settings.SHIP_TURN_SPEED * fElapsedTime);
                        if (m_InputManager.IsKeyDown(Keys.LeftShift))
                        {
                            m_Players[Turn].CurrentShip.turnRight(Settings.SHIP_TURN_SPEED * fElapsedTime);
                            m_Camera.Rotate(Settings.SHIP_TURN_SPEED * fElapsedTime);
                        }
                    }
                }

                //Buying ships
                if ( ( m_InputManager.IsKeyPressed(Keys.B) || m_InputManager.IsButtonPressed(Turn, Buttons.Y) ) && bGameOver == false )
                {
                    m_Players[Turn].buyShip(m_world);
                    m_Players[Turn].CurrentShipIndex = m_Players[Turn].totalShips - 1;
                    Target.Ship = m_Players[Turn].CurrentShip;
                    m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);
                    PauseGame(1.0f);
                    if (m_Players[Turn].Port.hasCoins() == false)
                        m_Players[Turn].PortCoin.Exists = false;
                    soundPlayer.PlayCue( "bell" );
                }

                //Select next ship
                if (m_InputManager.IsKeyPressed(Keys.E) || m_InputManager.IsButtonPressed(Turn, Buttons.RightShoulder))
                {
                    m_Players[Turn].NextShip();
                    Target.Ship = m_Players[Turn].CurrentShip;
                    m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);
                    soundPlayer.PlayCue("Slide");
                }

                //Select previous ship
                if (m_InputManager.IsKeyPressed(Keys.Q) || m_InputManager.IsButtonPressed(Turn, Buttons.LeftShoulder))
                {
                    m_Players[Turn].PrevShip();
                    Target.Ship = m_Players[Turn].CurrentShip;
                    m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);
                    soundPlayer.PlayCue("Slide");
                }

                //Dock Ship
                //if (m_InputManager.IsKeyPressed(Keys.G) || m_InputManager.IsButtonPressed(Turn, Buttons.X))
                //{
                //    CheckPorts( soundPlayer );
                //}
                ////////////////////////////////
                //Move the fire target
                if (m_InputManager.IsKeyDown(Keys.J) || (m_InputManager.CheckRStick(Turn).X < -Settings.DEAD_ZONE))
                {
                    if (m_InputManager.CheckRStick(Turn).X < -Settings.DEAD_ZONE)
                    {
                        m_Camera.Rotate(Settings.CAMERA_TURN_SPEED * fElapsedTime * m_InputManager.CheckRStick(Turn).X);    //-0.02f
                        Target.OrbitTarget(-Settings.CAMERA_TURN_SPEED * fElapsedTime * m_InputManager.CheckRStick(Turn).X);
                    }
                    else
                    {
                        m_Camera.Rotate(-Settings.CAMERA_TURN_SPEED * fElapsedTime);
                        Target.OrbitTarget(Settings.CAMERA_TURN_SPEED * fElapsedTime);
                    }
                }
                if (m_InputManager.IsKeyDown(Keys.L) || (m_InputManager.CheckRStick(Turn).X > Settings.DEAD_ZONE))
                {
                    if (m_InputManager.CheckRStick(Turn).X > Settings.DEAD_ZONE)
                    {
                        m_Camera.Rotate(Settings.CAMERA_TURN_SPEED * fElapsedTime * m_InputManager.CheckRStick(Turn).X);     //0.02f
                        Target.OrbitTarget(-Settings.CAMERA_TURN_SPEED * fElapsedTime * m_InputManager.CheckRStick(Turn).X);
                    }
                    else
                    {
                        m_Camera.Rotate(Settings.CAMERA_TURN_SPEED * fElapsedTime);     //0.02f
                        Target.OrbitTarget(-Settings.CAMERA_TURN_SPEED * fElapsedTime);
                    }
                }
                if (m_InputManager.IsKeyDown(Keys.I) || (m_InputManager.CheckRStick(Turn).Y > Settings.DEAD_ZONE))
                {
                    if (m_InputManager.CheckRStick(Turn).Y > Settings.DEAD_ZONE)
                    {
                        Target.ChangeRadius(Settings.CAMERA_ZOOM_SPEED * fElapsedTime * m_InputManager.CheckRStick(Turn).Y);
                        //m_Camera.Zoom(-Settings.CAMERA_ZOOM_SPEED * fElapsedTime * m_InputManager.CheckRStick(Turn).Y);//-1.0f);
                    }
                    else
                    {
                        Target.ChangeRadius(Settings.CAMERA_ZOOM_SPEED * fElapsedTime);//-1.0f);
                        //m_bEndTurn = true;  
                    }
                }
                if (m_InputManager.IsKeyDown(Keys.K) || (m_InputManager.CheckRStick(Turn).Y < -Settings.DEAD_ZONE))
                {
                    if (m_InputManager.CheckRStick(Turn).Y < -Settings.DEAD_ZONE)
                    {
                        Target.ChangeRadius(-Settings.CAMERA_ZOOM_SPEED * fElapsedTime * -m_InputManager.CheckRStick(Turn).Y);
                        //m_Camera.Zoom(Settings.CAMERA_ZOOM_SPEED * fElapsedTime * -m_InputManager.CheckRStick(Turn).Y);
                    }
                    else
                    {
                        Target.ChangeRadius(-Settings.CAMERA_ZOOM_SPEED * fElapsedTime);
                    }
                }

                //Enter fire mode
                if ( m_InputManager.IsKeyPressed(Keys.T) || m_InputManager.IsButtonPressed(Turn, Buttons.B ) )
                {
                    if (m_Players[Turn].CurrentShip.HasCannon == true)
                    { 
                        m_FireMode = true;
                        Target.Scale = 0.25f;
                        Target.SavePosition();
                        m_arc.Enabled = true;
                        m_world.TheCannonBall.Scale = 2.0f;

                        m_hud.ButtonDisplay.Kill();
                        //m_Camera.SetWobble();

                        //Only play the sizzle when the game is not over.
                        if( m_ParentModule.GameOver == false )
                            soundPlayer.PlayCue("Sizzle");

                        m_hud.LightFuse(true);
                        //m_Players[Turn].hasShot = true;
                        m_Players[Turn].CurrentShip.HasCannon = false;

                    }
                }
            }

            ////////////////////
            //Skip turns
            if( m_InputManager.IsKeyPressed(Keys.Space ) )
            {
                NextTurn();
                initAI = true;
                m_bEndTurn = false;
            }

            //Skip turn option
            if( m_InputManager.IsKeyPressed(Keys.End) || m_InputManager.IsButtonPressed( Turn, Buttons.X ) )
            {
                soundPlayer.PlayCue("Flip");
                m_bEndTurn = true;
                m_isPaused = true;             
            }

            if( m_bEndTurn == true )
            {  
                if( m_InputManager.IsKeyPressed(Keys.W) || m_InputManager.IsButtonPressed( Turn, Buttons.A ) )
                {
                    soundPlayer.PlayCue("Flip");
                    m_bEndTurn = false;
                    NextTurn();
                    initAI = true;
                }
                if( m_InputManager.IsKeyPressed(Keys.Q) || m_InputManager.IsButtonPressed( Turn, Buttons.B ) )
                {
                    soundPlayer.PlayCue("Flip");
                    m_bEndTurn = false;
                }
            }
        }

        private void InstantFireMode(float fElapsedTime, SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles)
        {
            if (m_world.TheCannonBall.Firing == false)
            {
                string strAttacker = m_Players[Turn].Name;

                //Fire the cannon ball
                m_world.TheCannonBall.FireCannonAtTarget(strAttacker, m_Players[Turn].CurrentShip.Location, Target.Location);
                soundPlayer.PlayCue("CannonShot");

                if(!m_Players[Turn].IsAI)
                    GamePad.SetVibration((PlayerIndex)Turn, 1.0f, 1.0f);
                PauseGame(0.50f);

                //m_Camera.ResetWobble();
                m_hud.IsFiring = false;
                //turn off the fuse and firing arc
                m_hud.LightFuse(false);
                m_arc.Enabled = false;
            }
        }
         
        /** @fn     void HandleStandardInput( float fElapsedTime )
         *  @brief  handle the game input while in fire mode
         *  @param  fElapsedTime [in] the time elapsed since the previous frame, in seconds.
         */
        private void HandleFireModeInput(float fElapsedTime, SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles)
        {
            //Handle fire mode input
            CannonBall cannonBall = m_world.TheCannonBall;

            //Locks Camera at an Angle
            //m_Camera.Rotation = -m_Players[Turn].CurrentShip.Yaw;
  
            //Check if the timer has expired
            if( m_Timer > Settings.SHIP_FIRE_TIMER )
            {
               if( cannonBall.Firing == false )
                {
                    string strAttacker = m_Players[ Turn ].Name;

                    //Fire the cannon ball
                    m_world.TheCannonBall.FireCannon(strAttacker, m_arc.GetVelocity(), m_arc.StartPoint, m_arc.EndPoint);

                    soundPlayer.StopCue("Sizzle");
                    soundPlayer.PlayCue("CannonShot");

                    if (!m_Players[Turn].IsAI)
                        GamePad.SetVibration((PlayerIndex)Turn, 1.0f, 1.0f);
                    PauseGame(0.50f);

                    //m_Camera.ResetWobble();
                    m_hud.IsFiring = false;
                    //turn off the fuse and firing arc
                    m_hud.LightFuse( false );
                    m_arc.Enabled = false;
                    m_Players[Turn].doneAction();
                }
            }
            else //The timer is counting down
            {
                m_Timer += fElapsedTime;
                m_arc.Enabled = true;

                Target.Wobble(m_InputManager, m_Camera, m_Players.Length, m_Players[Turn].CurrentShip, m_collision,Turn,m_Players,m_world.TheTown);
                m_arc.StartPoint = m_Players[Turn].CurrentShip.Location;
                m_arc.EndPoint = Target.Location;
                m_arc.Update();
                m_hud.IsFiring = true;
            }
           // m_hud.IsFiring = false;
        }

        private void PauseGame(float Time)
        {
            m_isPaused = true;
            m_PauseTimer = Time;

        }

        void CheckCoinToss()
        {
            CannonBall CoinToss = m_world.TossedCoin;
            if (CoinToss != null)
            {
                if (CoinToss.Complete)
                {
                    GotCoin NewCoin = new GotCoin(CoinToss.TheModel);
                    Vector3 TempLoc = new Vector3();
                    TempLoc = CoinToss.Location;
                    TempLoc.Y += 5.0f;
                    NewCoin.Launch(TempLoc, 0.75f);
                    m_world.AddGameObject(NewCoin, true);
                    CoinToss.Complete = false;
                }
            }
        }

        /** @fn     void CheckCannonBall( string strAttacker, SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles )
         *  @brief  check if the cannon ball hit anything
         */
        private void CheckCannonBall( SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles )
        {
            CannonBall cannonBall = m_world.TheCannonBall; 
            if( cannonBall.Complete )
            {
                ///////////////////////////////////
                //Check the ships
                bool bHit = false;

                foreach( Ship ship in m_world.Ships )
                {
                    if( ship.isDisabled == false )
                    {
                        if( cannonBall.CheckCollision( ship ) )
                        {
                            //Ship is hit
                            bHit = true;
                            DisableShip( ship, soundPlayer, listParticles );

                            for (int i = 0; i < listParticles.Count; i++)
                            {
                                if (listParticles[i].Type == Particles.ParticleState.FireBoom)
                                { listParticles.RemoveAt(i); }
                            }
                            Particles.ParticleSystem ShipExplo = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "FireBoom");
                            ShipExplo.Location = cannonBall.Location;
                            listParticles.Add(ShipExplo);

                            if(!m_Players[ship.ControllingPlayer].IsAI)
                                GamePad.SetVibration((PlayerIndex)ship.ControllingPlayer, 1.0f, 1.0f);
                            PauseGame(1.00f);

                            break;
                        }
                    }
                }
                if (bHit == false)
                {
                    ///////////////////////////////////////////
                    //Check the towers
                    foreach (Tower tower in m_world.TheTown.Towers)
                    {
                        if (tower.IsAlive)
                        {
                            if (tower.CheckCollision(cannonBall))
                            {
                                bHit = true;
                                DamageTower(CurrentShip, tower, soundPlayer, listParticles);

                                //Explosion from hitting the tower
                                for (int i = 0; i < listParticles.Count; i++)
                                {
                                    if (listParticles[i].Type == Particles.ParticleState.StructureBoomSmoke1 ||
                                        listParticles[i].Type == Particles.ParticleState.StructureBoomSmoke2 ||
                                        listParticles[i].Type == Particles.ParticleState.Dirt ||
                                        listParticles[i].Type == Particles.ParticleState.Stone)
                                    { listParticles.RemoveAt(i); }
                                }
                                Particles.ParticleSystem MushroomCloud = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "StructureBoomSmoke1");
                                Particles.ParticleSystem TwrSmokeBoom = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "StructureBoomSmoke2");
                                Particles.ParticleSystem TwrStoneBoom = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Stone");
                                Particles.ParticleSystem DirtBoom = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Dirt");
                                DirtBoom.Location = tower.Location;
                                MushroomCloud.Location = tower.Location;
                                TwrSmokeBoom.Location = tower.Location;
                                TwrStoneBoom.Location = tower.Location;
                                listParticles.Add(MushroomCloud);
                                listParticles.Add(TwrSmokeBoom);
                                listParticles.Add(TwrStoneBoom);
                                listParticles.Add(DirtBoom);
                            }
                        }
                    }

                    if (bHit == false)
                    {
                        //Missed and hit water
                        if (Target.Y < 3.0)
                        {
                            soundPlayer.PlayCue("WaterSplash");

                            for (int i = 0; i < listParticles.Count; i++)
                            {
                                if (listParticles[i].Type == Particles.ParticleState.Splash)
                                { listParticles.RemoveAt(i); }
                            }
                            Particles.ParticleSystem Splash = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Splash");
                            Splash.Location = cannonBall.Location;
                            listParticles.Add(Splash);
                        }
                        else
                        {//Missed and hit terrain
                            soundPlayer.PlayCue("GroundHit");
                            for (int i = 0; i < listParticles.Count; i++)
                            {
                                if (listParticles[i].Type == Particles.ParticleState.Dirt ||
                                    listParticles[i].Type == Particles.ParticleState.Sand ||
                                    listParticles[i].Type == Particles.ParticleState.LandBoomSmoke)
                                { listParticles.RemoveAt(i); }
                            }
                            Particles.ParticleSystem DirtBoom = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Dirt");
                            Particles.ParticleSystem SandBoom = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Sand");
                            Particles.ParticleSystem SmokeBoom = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "LandBoomSmoke");
                            DirtBoom.Location = cannonBall.Location;
                            SandBoom.Location = cannonBall.Location;
                            SmokeBoom.Location = cannonBall.Location;
                            listParticles.Add(DirtBoom);
                            listParticles.Add(SandBoom);
                            listParticles.Add(SmokeBoom);
                        }
                        m_Players[Turn].PlayerStats.incrementMisses();
                    }
                }

                m_world.TheCannonBall.Complete = false;

                //////////////////////////
                //TEMP

                //Reset fire states
                //Target.LoadPosition();
                Target.Scale = 1.0f;

                m_FireMode = false;
                m_Players[Turn].CurrentShip.HasCannon = false;
                //m_Players[Turn].hasShot = m_Cannon;
                m_arc.Enabled = false;
                m_Timer = 0.0f;

                m_hud.LightFuse(false);

            }
        }


        /** @fn     void DisableShip( Ship ship )
         *  @brief  disable a hit ship and perform al lthe related tasks
         *  @param  ship [in] the ship to disable
         */
        private void DisableShip( Ship ship, SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles)
        {
            //hit a boat
            //string strMessage = m_world.TheCannonBall.Owner + " has sunk one of Player " + m_Players[ship.ControllingPlayer].Name + "'s ships!\n";
            m_hud.ShowPopupMessage(m_world.TheCannonBall.Owner + " has sunk one of " + m_Players[ship.ControllingPlayer].Name + "'s ships!", Color.Red);
            if (m_world.TheCannonBall.Owner.Contains("Tower"))
            {
                m_Players[ship.ControllingPlayer].PlayerStats.incrementTowerLosses();
            }
            else if (m_Players[Turn] == m_Players[ship.ControllingPlayer])
            {
                m_Players[ship.ControllingPlayer].PlayerStats.incrementSelfDestructs();
            }
            else
            {
                m_Players[Turn].PlayerStats.incrementShipHits();
                m_Players[ship.ControllingPlayer].PlayerStats.incrementShipLosses();
            }
            
            TraceFireInformation( m_world.TheCannonBall.Owner, m_Players[ship.ControllingPlayer].Name );

            soundPlayer.PlayCue("Hit");
            ship.disableShip();

            //Adding a Fire and Smoke to the Ship
            Particles.ParticleSystem Fire = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Fire");
            Particles.ParticleSystem Smoke = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Smoke");
            Fire.Location = ship.Location;
            Smoke.Location = ship.Location;

            listParticles.Add(Fire);
            listParticles.Add(Smoke);

            //Add a coin if the ship was carrying one
            if (ship.hasBooty)
            {
                Coin coin = new Coin(m_mdlCoin);
                coin.Location = ship.Location;
                coin.Y += 8.0f;

                m_world.AddCoin(coin);

                //strMessage += m_Players[Turn].Name + " has dropped a coin!";
                m_hud.ShowPopupMessage(m_Players[ship.ControllingPlayer].Name + " has dropped a coin!", Color.Red);
                ship.hasBooty = false;
                for (int i = 0; i < Players.Length; i++) {
                    m_Players[i].affectShipHeat(ship.ControllingPlayer, m_Players[ship.ControllingPlayer].CurrentShipIndex, -1);
                }
            }

            //Show a sink message
            //m_hud.ShowPopupMessage(strMessage, Color.Red);
        }

        
        /** @fn     void DamageTower( Tower tower, Soundmanager sound, List< Particles > lstParticles )
         *  @brief  
         *  
         */
        private void DamageTower( Ship ship, Tower tower, SoundManager soundPlayer, List< Particles.ParticleSystem > lstParticles )
        {
            //hit a boat
            string strMessage = m_world.TheCannonBall.Owner + " has been hit by " + m_Players[ship.ControllingPlayer].Name + "!";

            m_Players[Turn].PlayerStats.incrementTowerHits();

            soundPlayer.PlayCue("Hit");
            tower.TakeDamage( 100 );

            //Adding a Fire and Smoke to the Ship
            Particles.ParticleSystem Fire = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Fire");
            Particles.ParticleSystem Smoke = new Particles.ParticleSystem(m_ParentModule.ParentApp, m_ParentModule.ParentApp.Content, "Smoke");
            Fire.Location = tower.Location;
            Smoke.Location = tower.Location;

            lstParticles.Add(Fire);
            lstParticles.Add(Smoke);
        }

        /** @fn     void HandleDebugInput( float fElapsedTime )
            @brief  handle the debug keys
            @param  fElapsedTime [in] the time, in seconds, since the previous frame
         */
        private void HandleDebugInput(float fElapsedTime)
        {
            //Debug add Coin
            if (m_InputManager.IsKeyPressed(Keys.F1))
            {
                m_Players[m_Turn].Port.incrementCoins();
                if (m_Players[Turn].Port.hasCoins() == true)
                    m_Players[Turn].PortCoin.Exists = true;
            }
            //Debug remove Coin
            if (m_InputManager.IsKeyPressed(Keys.F2))
            {
                m_Players[m_Turn].Port.decrementCoins();
                if (m_Players[Turn].Port.hasCoins() == false)
                    m_Players[Turn].PortCoin.Exists = false;
            }

            //Debug replenish move points
            if (m_InputManager.IsKeyPressed(Keys.F4))
            {
                m_Players[ m_Turn ].MovePoints = m_Players[ m_Turn ].InitialMovePoints;
            }

        }

        /*  @fn updateAI
        *  @brief  this is the standard update loop for the AI
        */
        public void updateAI(float speed, float frameTime, Collision collision, World theWorld, SoundManager soundPlayer) {
            if (m_InputManager.IsKeyPressed(Keys.NumPad0)) {
                m_debugAI = !m_debugAI;
                m_hud.ShowPopupMessage("AI Debug is " + (m_debugAI ? "ON" : "Off"),m_Players[Turn].Color);
            }
            if (m_debugAI) {
                HandleStandardInput(frameTime, soundPlayer);
                
                    //m_Players[Turn].ForceShipToFacePoint(CurrentShip,(m_Players[Turn].currentAction.MoveTo)*-1);
                m_Players[Turn].CurrentShip.moveToAISafeRegion(speed * Settings.SHIP_MOVE_SPEED * frameTime);
                return;
            }
            m_Players[Turn].currentDelay += frameTime;
            m_Players[Turn].checkAI();
            if (m_Players[Turn].currentDelay >= Settings.AI_TOTAL_DELAY) {//this gives the AI a little bit of a pause between action.

                if (m_Players[Turn].currentAction.Action == ActionStates.Events.FireAt) {//Fire the cannon at the target
                    Target.Location = new Vector3(m_Players[Turn].currentAction.FireAt.X, collision.findHeight(m_Players[Turn].currentAction.FireAt).heightAtPoint, m_Players[Turn].currentAction.FireAt.X);
                    if (m_Players[Turn].CurrentShip.HasCannon == true)
                    {
                        m_FireMode = true;
                        Target.Scale = 0.25f;
                        Target.SavePosition();
                        m_arc.Enabled = true;
                        m_world.TheCannonBall.Scale = 2.0f;

                        m_Camera.SetWobble();

                        m_hud.LightFuse(true);
                        m_Players[Turn].CurrentShip.HasCannon = false;
                    }
                    else
                    {

                    }
                    //m_Players[Turn].doneAction();
                }
                else if (m_Players[Turn].currentAction.Action == ActionStates.Events.MoveTo) {//move the ship

                    //m_Camera.Rotate(Settings.SHIP_TURN_SPEED * frameTime * m_InputManager.CheckLStick(Turn).X);
                    //m_Camera.OrbitPlayer(m_Players[Turn]);
                    m_Camera.Rotation = -m_Players[Turn].CurrentShip.Yaw;
                        

                    m_Players[Turn].TurnShipToFacePoint(CurrentShip, m_Players[Turn].currentAction.MoveTo, Settings.SHIP_TURN_SPEED * frameTime);
                    Vector2 temp = new Vector2(CurrentShip.Location.X, CurrentShip.Location.Z) - m_Players[Turn].currentAction.MoveTo;
                    if (temp.Length() < speed * 2) {//if the ship is close enough then end the turn
                        m_Players[Turn].doneAction();
                        if (m_Players[Turn].currentAction.Action == ActionStates.Events.MoveTo) {
                            m_Players[Turn].currentDelay = Settings.AI_TOTAL_DELAY;
                        }
                    }
                    else {
                        m_Players[Turn].CurrentShip.moveForward(speed * Settings.SHIP_MOVE_SPEED * frameTime);
                        //if (!m_Players[Turn].isOK(CurrentShip.CurrentLocation2D)) {
                            //m_Players[Turn].ForceShipToFacePoint(CurrentShip,(m_Players[Turn].currentAction.MoveTo)*-1);
                            //m_Players[Turn].CurrentShip.shipHeading += MathHelper.Pi;
                        m_Players[Turn].CurrentShip.moveToAISafeRegion(speed * Settings.SHIP_MOVE_SPEED*frameTime);
                            m_Players[Turn].MovePoints -= (m_Players[Turn].CurrentShip.PreviousLocation2D - m_Players[Turn].CurrentShip.CurrentLocation2D).Length() * speed * Settings.SHIP_MOVE_COST;
                        //}
                    }
                    
                }//end if
                //else if (m_Players[Turn].currentAction.Action == ActionStates.Events.PickUp) {//get a coin
                //    CheckPorts(soundPlayer);
                //}
                else if (m_Players[Turn].currentAction.Action == ActionStates.Events.Purchase) {//buy a new ship
                    m_Players[Turn].buyShip(theWorld);
                    m_Players[Turn].doneAction();
                    foreach (Player m in m_Players) {
                        m.affectCoveHeat(Turn, m_Players[Turn].Port.Coins-1);
                    }
                    if (m_Players[Turn].Port.hasCoins() == false)
                        m_Players[Turn].PortCoin.Exists = false;
                    soundPlayer.PlayCue("bell");
                }
                else if (m_Players[Turn].currentAction.Action == ActionStates.Events.Switch) {//switch Ships.
                    m_Players[Turn].SetCurShip(m_Players[Turn].currentAction.SwitchShip);

                    Target.Ship = m_Players[Turn].CurrentShip;
                    m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);
                    soundPlayer.PlayCue("Slide");

                    m_Players[Turn].doneAction();
                }
                else if (m_Players[Turn].currentAction.Action == ActionStates.Events.ReEvaluate){
                    m_Players[Turn].decideAI(frameTime);
                    //m_Players[Turn].doneAction();
                }
                else{//No Actions on the List.
                    m_Players[Turn].decideAI(frameTime);

                }
                 
            }//end delay if

        }//end function

        /** @fn    RunTurn()
        *  @brief  Basic Inputs to toggle between Boats, as well as Players, and moves
        */
        public void EndGame(float fElapsedTime, SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles, int WinningPlayer)
        {
            if (m_Players[Turn].CurrentShip.HasCannon == false)
                m_Players[Turn].CurrentShip.HasCannon = true;

            m_Players[m_Turn].MovePoints = m_Players[m_Turn].InitialMovePoints;

            CheckParticles(listParticles);
            CheckCoinToss();            

            for (int i = 0; i < m_Players.Length; i++)
                GamePad.SetVibration((PlayerIndex)i, 0.0f, 0.0f);

            HandleDebugInput(fElapsedTime);

            //Check the cannonball every frame
            CheckCannonBall(soundPlayer, listParticles);

            if (FireMode)
            {
                InstantFireMode(fElapsedTime, soundPlayer, listParticles);
            }
            else
            {
                if (m_Players[Turn].CurrentShip.isDisabled == true)
                    m_Players[Turn].NextShip();

                HandleStandardInput(fElapsedTime, soundPlayer);

                CheckPorts(soundPlayer);

                Target.Update( m_collision );

                foreach (Targetter BoatTarget in m_world.Targetters)
                {
                    BoatTarget.Update(m_collision);
                }
            }

            //temporary stat end game readout - for debug

            //string statP1, statP2, statP3, statP4;

            //statP1 = "Distance Travelled: " + m_PlayerStats[0].returnDistance() + "\n" +
            //         "Accuracy: " + m_PlayerStats[0].returnHitPercentage() + "\n" +
            //         "Number of Ships Hit: " + m_PlayerStats[0].returnShipHits() + "\n" +
            //         "Number of Ships Lost: " + m_PlayerStats[0].returnLosses() + "\n" +
            //         "Number of Towers Destroyed: " + m_PlayerStats[0].returnTowerHits();

            //statP2 = "Distance Travelled: " + m_PlayerStats[1].returnDistance() + "\n" +
            //         "Accuracy: " + m_PlayerStats[1].returnHitPercentage() + "\n" +
            //         "Number of Ships Hit: " + m_PlayerStats[1].returnShipHits() + "\n" +
            //         "Number of Ships Lost: " + m_PlayerStats[1].returnLosses() + "\n" +
            //         "Number of Towers Destroyed: " + m_PlayerStats[1].returnTowerHits();

            //statP3 = "Distance Travelled: " + m_PlayerStats[2].returnDistance() + "\n" +
            //         "Accuracy: " + m_PlayerStats[2].returnHitPercentage() + "\n" +
            //         "Number of Ships Hit: " + m_PlayerStats[2].returnShipHits() + "\n" +
            //         "Number of Ships Lost: " + m_PlayerStats[2].returnLosses() + "\n" +
            //         "Number of Towers Destroyed: " + m_PlayerStats[2].returnTowerHits();

            //statP4 = "Distance Travelled: " + m_PlayerStats[3].returnDistance() + "\n" +
            //         "Accuracy: " + m_PlayerStats[3].returnHitPercentage() + "\n" +
            //         "Number of Ships Hit: " + m_PlayerStats[3].returnShipHits() + "\n" +
            //         "Number of Ships Lost: " + m_PlayerStats[3].returnLosses() + "\n" +
            //         "Number of Towers Destroyed: " + m_PlayerStats[3].returnTowerHits();
        }

        /** @fn    RunTurn()
        *  @brief  Basic Inputs to toggle between Boats, as well as Players, and moves
        */
        public void RunTurn(float fElapsedTime, SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles)
        {
            if (m_Players[m_Turn].IsAI)
            {
                if (initAI)//this check should force the AI to only run AIBeginTurn once
                {
                    m_Players[m_Turn].AIBeginTurn(fElapsedTime);
                    initAI = false;
                }
            }

            HandleCoinVisibility(m_Players[Turn].CurrentShip);
            CheckParticles(listParticles);
            CheckCoinToss();

            if (!m_isPaused && !Guide.IsVisible)
            {
                for (int i = 0; i < m_Players.Length; i++)
                    GamePad.SetVibration((PlayerIndex)i, 0.0f, 0.0f);
                HandleDebugInput(fElapsedTime);

                //Check the cannonball every frame
                CheckCannonBall(soundPlayer, listParticles);

                //Player turns
                if (m_townPlayer.Active)
                {
                    //AI turn
                    HandleTownTurn(fElapsedTime, soundPlayer, listParticles);
                }
                else
                {
                    if (FireMode)
                    {
                        HandleFireModeInput(fElapsedTime, soundPlayer, listParticles);
                    }
                    else
                    {
                        //if (m_Players[m_Turn].MovePoints < 0 && Cannon == false)
                        if (!m_isPaused)
                        {
                            if (m_Players[m_Turn].MovePoints < 0 || m_Players[m_Turn].CheckAllDisabled())
                            {
                                NextTurn();
                                initAI = true;
                            }
                            else if (m_Players[Turn].CurrentShip.isDisabled == true)
                            {
                                {
                                    m_Players[Turn].NextShip();
                                    Target.Ship = m_Players[Turn].CurrentShip;
                                    m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);
                                }
                            }


                            //Handle the input, and potentially change the position
                            //HandleCameraInput(fElapsedTime);
                            if (m_Players[m_Turn].IsAI)
                            {
                                updateAI(1.0f, fElapsedTime, m_collision, m_world, soundPlayer);
                            }
                            else
                            {
                                HandleStandardInput(fElapsedTime, soundPlayer);
                            }

                            Target.Update(m_collision);
                            foreach (Targetter TheTarget in m_world.Targetters)
                            {
                                if (TheTarget.UseShipRotation == false)
                                {
                                    TheTarget.Rotation = -((float)Math.Atan2((m_Players[TheTarget.Ship.ControllingPlayer].Port.Z * 1.2) - TheTarget.Z, (m_Players[TheTarget.Ship.ControllingPlayer].Port.X * 1.2) - TheTarget.X)) + (float)Math.PI;
                                }
                            }
                            CheckPorts(soundPlayer);

                            foreach (Targetter BoatTarget in m_world.Targetters)
                            {
                                BoatTarget.Update(m_collision);
                            }
                        }
                    }
                }
            }
            else
            {
                m_PauseTimer -= fElapsedTime;
                //Target.Update(m_collision);
                if (m_PauseTimer < 0)
                    m_isPaused = false;
            }

            if (m_Players[Turn].MovePoints < 0.25f)//reset to true if there isn't enough move points
            {
                initAI = true;
                if (m_Players[Turn].IsAI)
                {
                    NextTurn();

                }
                
            }
        }

        /** @fn     void HandleAITurn( float fElapsedTime )
         *  @brief  handle the AI turn
         *  @param  fElapsedTime [in] the time since the last frame
         */
        private void HandleTownTurn(float fElapsedTime, SoundManager soundPlayer, List<Particles.ParticleSystem> listParticles)
        {
            m_townPlayer.Update( fElapsedTime, soundPlayer );

            if( m_townPlayer.Active == false )
            {                
                StartPlayerTurn();
            }
        }

        /** @fn     CheckPorts( SoundManager soundPlayer ))
         *  @brief  Checks the position of the player's boat in relation to other landmarks (such as the town or the ports)
         *  @param  soundPlayer [in] the handle to the soundPlayer
         */
        public void CheckPorts( SoundManager soundPlayer )
        {
            CheckCoins(soundPlayer);
            CheckTownPorts(soundPlayer);
            CheckCoves(soundPlayer);
            CheckDropoff(soundPlayer);
        }

        public void CheckTownPorts( SoundManager soundPlayer )
        {
            if( !m_Players[ Turn ].CurrentShip.hasBooty )
            {
                Port portCollide = m_world.TheTown.CheckPorts( m_Players[ Turn ].CurrentShip );

                if( portCollide != null && portCollide.hasCoins() )
                {
                    m_Players[Turn].CurrentShip.hasBooty = true;
                    portCollide.decrementCoins();
                    Vector3 TempLocation = new Vector3();
                    TempLocation = m_world.TheTown.Location;
                    TempLocation.Y += 30;
                    for( int i = 0; i < m_world.TheTown.Ports.Length; i++)
                        if( m_world.TheTown.Ports[i] == portCollide )
                        {
                            TransferCoin(m_world.TheTown.PortCoins[i].Location, m_Players[Turn].CurrentShip.Location);
                        }
                    PauseGame(0.5f);

                    for (int i = 0; i < m_Players.Length; i++) {
                        m_Players[i].affectShipHeat(Turn, m_Players[Turn].CurrentShipIndex, 1);
                        //NEED TO FIND THE INDEX OF THE PILAGED PORT
                        m_Players[i].affectTownHeat(portCollide.Owner, portCollide.Coins);
                    }

                    soundPlayer.PlayCue("Coins_drop");
                    m_hud.ShowPopupMessage( "You have a coin! Now bring it Home", m_Players[Turn].Color );
                }
            }
        }
        public void CheckCoins( SoundManager soundPlayer )
        {
            /////////////////////////////
            //Check any sunken areas for coins (if it didn't just pick one up)
            if (!m_Players[Turn].CurrentShip.hasBooty)
            {
                for (int i = 0; i < m_world.Coins.Count; ++i)
                {
                    if (m_world.Coins[i].CheckCollide(m_Players[Turn].CurrentShip))
                    {
                        m_Players[Turn].CurrentShip.hasBooty = true;
                        m_world.Coins.RemoveAt(i);

                        GotCoin NewCoin = new GotCoin(m_CoinToss.TheModel);
                        Vector3 TempLoc = new Vector3();
                        TempLoc = m_Players[Turn].CurrentShip.Location;
                        TempLoc.Y += 5.0f;
                        NewCoin.Launch(TempLoc, 0.75f);
                        m_world.AddGameObject(NewCoin, true);

                        for (int wha = 0; i < 4; i++) {
                            m_Players[wha].affectShipHeat(Turn, m_Players[Turn].CurrentShipIndex, 1);
                        }

                        m_hud.ShowPopupMessage(m_Players[Turn].Name + " has collected a coin!", Color.Yellow);
                        soundPlayer.PlayCue( "Coins_drop" );

                        break;
                    }
                }
            }
        }

        public void CheckCoves( SoundManager soundPlayer )
        {
            //////////////////////////////////
            //Check Sonya's ports - if it hasn't picked up a coin elsewhere
            if (!m_Players[Turn].CurrentShip.hasBooty)
            {
                int f = 0;
                foreach (Port cove in m_world.PirateCoves)
                {
                    //Skip the player's own port
                    if (cove == m_Players[Turn].Port)
                        continue;

                    //Check the enemy port
                    if (cove.hasCoins() && cove.checkPort(m_Players[Turn].CurrentShip))
                    {
                        m_Players[Turn].CurrentShip.hasBooty = true;
                        cove.decrementCoins();

                        Vector3 TempLocation = new Vector3();
                        TempLocation = cove.Location * 1.2f;
                        TempLocation.Y += 25;
                        TransferCoin(m_Players[cove.Owner].PortCoin.Location, m_Players[Turn].CurrentShip.Location);
                        if (m_Players[cove.Owner].Port.hasCoins() == false)
                            m_Players[cove.Owner].PortCoin.Exists = false;

                        PauseGame(1.0f);

                        for (int i = 0; i < Players.Length; i++) {
                            m_Players[i].affectCoveHeat(f, cove.Coins);
                            m_Players[i].affectShipHeat(Turn, m_Players[Turn].CurrentShipIndex, 1);
                        }

                        soundPlayer.PlayCue( "Coins_drop" );
                        m_hud.ShowPopupMessage(m_Players[Turn].Name + " has stolen a coin from " + m_Players[cove.Owner].Name + "!!!", Color.Yellow);
                    }
                    f++;
                }
            }
        }
        public void CheckDropoff(SoundManager soundPlayer)
        {
            /////////////////////////////
            //Check the player's cove to see if the player is trying to dump a coin
            if (m_Players[Turn].CurrentShip.hasBooty)
            {
                if (m_Players[Turn].Port.checkPort(m_Players[Turn].CurrentShip))
                {
                    m_Players[Turn].CurrentShip.hasBooty = false;
                    m_Players[Turn].Port.incrementCoins();
                    Vector3 TempLocation = new Vector3();
                    TempLocation = m_Players[Turn].Port.Location * 1.2f;
                    TempLocation.Y += 25;
                    TransferCoin(m_Players[Turn].CurrentShip.Location, m_Players[Turn].PortCoin.Location);
                    m_Players[Turn].ChestLid.Openning(true);
                    if (m_Players[Turn].Port.hasCoins())
                        m_Players[Turn].PortCoin.Exists = true;

                    PauseGame(0.5f);

                    for (int i = 0; i < m_Players.Length; i++) {
                        m_Players[i].affectShipHeat(Turn, m_Players[Turn].CurrentShipIndex, -1);
                        m_Players[i].affectCoveHeat(Turn, m_Players[Turn].Port.Coins);
                    }
                    soundPlayer.PlayCue( "Coins_drop" );
                    m_hud.ShowPopupMessage(m_Players[Turn].Name + " has stashed a coin!!!", Color.Yellow);
                }
            }
        }

        /** @fn     NextTurn()
         *  @brief  Switches to the next player (0, 1, 2, or 3)
         */
        private void NextTurn()
        {
            m_Players[Turn].ChestLid.Openning(false);
            EndPlayerTurn();
            //m_Players[Turn].hasShot = false;
            foreach (Coin coin in m_world.TheTown.PortCoins)
            {
                coin.Drawable = true;
            }
            foreach (Player player in m_Players)
            {
                player.PortCoin.Drawable = true;
            }
            m_Camera.Height = Settings.CAMERA_HEIGHT;

            //Advance to the next player
            m_Turn = ( m_Turn + 1 ) % ( m_Players.Length );

            //Cycle just ended - activate the town
            if( m_Turn == 0 )
            {
                StartTownTurn();
                return;
            }
            //initAI = true;
            //Another player's turn has begun
            //m_hud.ShowPopupMessage(m_Players[Turn].Name + "'s turn.", m_Players[Turn].Color);
            StartPlayerTurn();
        }

        /** @fn     void StartPlayerTurn()
         *  @brief  start the turn of the current player
         */
        private void StartPlayerTurn()
        {
             m_hud.ButtonDisplay.Kill();

             
            //Enable the ships as required
            List<Ship> TempList = new List<Ship>();
            TempList = m_Players[m_Turn].EnableShips();
            foreach (Ship ship in TempList)
            {
                m_EnabledList.Add(ship);
            }
            TempList = m_Players[m_Turn].PrepEnableShips();
            foreach (Ship ship in TempList)
            {
                m_PrepEnabledList.Add(ship);
            }

            //Check if the player has any available ships
            if (m_Players[m_Turn].CheckAllDisabled())
            {
                //Try the next player
                NextTurn();
                initAI = true;
                return; //Break out or else it'll run the rest of this function when the stack unrolls
            }

            //Select the first unsunk ship - of which there must be one.
            while (m_Players[m_Turn].CurrentShip.isDisabled == true)
            {
                m_Players[m_Turn].NextShip();
                Target.Ship = m_Players[Turn].CurrentShip;
                m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);
            }

            m_Players[m_Turn].MovePoints = m_Players[m_Turn].InitialMovePoints;
            m_Players[m_Turn].Active = true;

            foreach(Ship ship in m_Players[Turn].Ships)
                if (ship!= null)
                    ship.HasCannon = true;
            m_FireMode = false;

            m_hud.ShowPopupMessage(m_Players[Turn].Name + "'s turn.", m_Players[Turn].Color);

            /*
            if (m_Players[m_Turn].IsAI)
            {
                //m_Players[m_Turn]
                //m_Players[m_Turn].initHeatMeter();
                //m_Players[m_Turn].setGeneralHeatLevel();
            }
            else if (!m_Players[m_Turn].IsAI)
            {
                m_InputManager.ResetInput();
                m_hud.LightFuse(false);

            }
            */

            //Move the targets to the current player
            Target.Ship = m_Players[Turn].CurrentShip;
            m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);

            Target.Update(m_collision);

            if( m_TotalTurns < 2 )
                m_hud.ButtonDisplay.QueueButtonToDraw( "xboxButtonTip" );

            PauseGame(1.0f);
            GamePad.SetVibration( ( PlayerIndex )Turn, 0.3f, 0.3f );
        }

        /** @fn     void EndPlayerTurn()
         *  @brief  end the current player's turn
         */
        private void EndPlayerTurn()
        {
            //m_PlayerStats[Turn].addDistanceTravelled((int)m_Players[Turn].InitialMovePoints - (int)m_Players[Turn].MovePoints);

            m_Players[Turn].Active = false;
        }

        /** @fn void CheckParticles()
         *  @brief Checks the Particles for clearing
         */
        private void CheckParticles(List<Particles.ParticleSystem> listParticles)
        {

            for (int i = 0; i < listParticles.Count; i++)
                {
                    if (listParticles[i].Type == Particles.ParticleState.Smoke)
                    {
                        if ( listParticles[i].IsEmpty == true && listParticles[i].Active == false )
                            listParticles.RemoveAt(i);
                        }
                    }

            foreach(Particles.ParticleSystem particle in listParticles)
            {
                if (particle.Type == Particles.ParticleState.Wake)
                {
                    if (m_moving == true)
                        particle.Active = true;
                    else
                        particle.Active = false;
                    particle.Location = m_Players[m_Turn].CurrentShip.Location;
                    m_moving = false;
                }
            }
            //Handles Particle Clearing
            foreach (Ship ship in m_PrepEnabledList)
            {
                for (int i = 0; i < listParticles.Count; i++)
                {
                    if (ship.Location == listParticles[i].Location)
                    {
                        if (listParticles[i].Type == Particles.ParticleState.Smoke)
                            listParticles[i].Active = false;
                        if (listParticles[i].Type == Particles.ParticleState.Fire)
                            listParticles.RemoveAt(i);
                    }
                }
            }

            m_PrepEnabledList.Clear();

            foreach (Ship ship in m_EnabledList)
            {
                for (int i = 0; i < listParticles.Count; i++)
                {
                    if (ship.Location == listParticles[i].Location)
                    {
                        if (listParticles[i].Type == Particles.ParticleState.Smoke)
                            listParticles[i].Active = false;
                    }
                }
            }
            m_EnabledList.Clear(); 
        }

        /** @fn     void StartTownTurn()
         *  @brief  start the town's turn
         */
        private void StartTownTurn()
        {   
            bool bHasActions = m_townPlayer.StartTurn();
            m_TotalTurns++;

            int nTurnLimit =  m_ParentModule.TheGameSettings.TurnLimit;

            //Check if this was the final turn.  if so, skip the town's turn and round 
            if( m_TotalTurns > nTurnLimit && nTurnLimit != -1 )
                return;

            ////////////////////////////////
            //Not the final turn
            if( bHasActions )
            {
                m_hud.ShowPopupMessage("Town's Turn", Color.White);
                m_hud.ShowPopupMessage("Round: " + m_TotalTurns, Color.Red);
            }
            else
            {
                m_hud.ShowPopupMessage("Round: " + m_TotalTurns, Color.Red);
                StartPlayerTurn();
            }            
        }

        /** @fn    ToggleFireMode()
        *  @brief  Switches to firemode(if you have a cannon shot) or out of it
        */
        public void ToggleFireMode()
        {
            if (m_Players[Turn].CurrentShip.HasCannon == true && FireMode == false)
                FireMode = true;
            else if (FireMode == true)
                FireMode = false;
        }

        public void TransferCoin(Vector3 BasePort, Vector3 ShipTarget)
        {            
            m_CoinToss.FireCannonAtTarget("Coin Toss", BasePort, ShipTarget);        
        }

        /// <summary>
        /// One time call to prepare the winning player for his or her victory lap
        /// </summary>
        public void PrepEndGame(int WinningPlayer, List<Particles.ParticleSystem> lstParticles)
        {
            if (m_Turn != WinningPlayer)
            {
                //move to the winning player
                m_Turn = WinningPlayer;
            }

            //Enable all the winning player's ships
            foreach (Ship ship in m_Players[m_Turn].Ships)
            {
                if (ship != null)
                    ship.EnableShip();
            }
            
            Target.Ship = m_Players[Turn].CurrentShip;
            m_Camera.Rotation = (-m_Players[m_Turn].CurrentShip.Yaw);
            Target.Update(m_collision);
            m_cFireworks.BONZAI(WinningPlayer);
            lstParticles.Add(m_cFireworks.getWinnerFireworks1());
            lstParticles.Add(m_cFireworks.getWinnerFireworks2());
        }

        private void HandleCoinVisibility(Ship CurrentShip)
        {
            if (CurrentShip.hasBooty)
            {
                foreach (Coin coin in m_world.TheTown.PortCoins)
                {
                    coin.Drawable = false;
                }
                foreach (Player player in m_Players)
                {
                    player.PortCoin.Drawable = false;
                }
                foreach (Coin coin in m_world.Coins)
                {
                    coin.Drawable = false;
                }
            }
            else
            {
                foreach (Coin coin in m_world.TheTown.PortCoins)
                {
                    coin.Drawable = true;
                }
                foreach (Player player in m_Players)
                {
                    player.PortCoin.Drawable = true;
                }
                foreach (Coin coin in m_world.Coins)
                {
                    coin.Drawable = true;
                }
                m_Players[CurrentShip.ControllingPlayer].PortCoin.Drawable = false;
            } 
        }
        /// <summary>
        /// Adds the remaining stats (gold and ships) to the stats tracking
        /// </summary>
        public void FinalizeStats()
        {
            for (int i = 0; i < m_Players.Length; i++)
            {
                m_Players[i].PlayerStats.setGoldAmount(m_Players[i].totalGold);
                m_Players[i].PlayerStats.setShipAmount(m_Players[i].totalShips);
            }
        }

        #endregion

        #region DEBUG_OUTPUT

        private void TraceFireInformation(string strAttacker, string strVictim)
        {
            Error.Trace( "Attacker: " + strAttacker );
            Error.Trace( "Victim: " + strVictim );     
        }

        #endregion
    }
}
