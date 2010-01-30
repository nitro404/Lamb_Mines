using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;    //needed for checking whether or not the guide is visible

using Particles;

namespace Scallywags
{
    /** @class  GameModule
     *  @brief  the main branch of game execution, a type of module
     */
    public class GameModule : XNAModule
    {
        #region DATA_MEMBERS

                private TurnManager     m_TurnManager;      ///< A Turn Manager for keeping track of Turns
                private World           m_world;            ///< The game world
                private Camera          m_camera;           ///< The game camera
                                                       
                private SpriteBatch     m_sb;               ///< The sprite batch for 2D rendering
                private HUD             m_hud;              ///< The class that will handle everything HUD
                private WinScreen       m_winScreen;        ///< The win screen instance              
                                                      
                private Collision       m_collision;        ///< The collision class
                private Town            m_Town;
                private bool            m_bIsPaused;
                                 
                private bool            m_bUseDebugCamera;  ///< use the debug camera? 
                private GameSettings    m_GameSettings;
                private bool            m_isGameOver;
                private Player[]        m_Players;
                List<ParticleSystem>    m_lstParticles; ///<All Particle Effects we plan to use
                Clouds                  m_cClouds;
                PauseHandler            m_phPauser;

                private ButtonIndicator m_buttonText;

                //string LeaderBoard;
                int InitialPortGold;

                TimeSpan timeToNextProjectile = TimeSpan.Zero;

        #endregion

        public bool GameOver
        {
            get
            {
                return m_isGameOver;
            }
        }

        /** @prop
         *  @brief  the application game settings
         */
        public GameSettings TheGameSettings
        {
            get
            {
                return m_GameSettings;
            }
        }

        /** @fn     GameModule()
         *  @brief  constructor
         */
        public GameModule()
            : base(MODULE_IDENTIFIER.MID_GAME_MODULE)
        {
            m_camera        = new Camera();
            m_world         = new World();
            m_TurnManager   = new TurnManager(m_world );
            m_hud           = new HUD();
            m_winScreen     = new WinScreen();
            m_collision     = new Collision();
            m_cClouds       = new Clouds();
            
            
            m_bUseDebugCamera = false;

            #region RESOURCE_REGISTRATION
            ////////////////////////////////////////
            //Register resources -- These need to match the ordering in RESOURCE_CONSTANTS section

            //Models 
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TerrainTex");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/waterTex");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/IslandTrees");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/SkyHorizon");

            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Boat");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/BoatGold");
            //RegisterResource(ResourceType.RT_MODEL, "Content/Models/BelgiumSmack");
            //RegisterResource(ResourceType.RT_MODEL, "Content/Models/ScottishBoat");
            //RegisterResource(ResourceType.RT_MODEL, "Content/Models/JapWarBoat");

            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TargetCircle");
            //RegisterResource(ResourceType.RT_MODEL, "Content/Models/menu sign");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Coin");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/FiringArchOrb");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/CannonBall");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TargetingSelector1");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TargetingSelector2");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TargetingSelector4");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TargetCyl");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Lighthouse");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/HousesRed");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/HousesOrange");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/HousesPink");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Dock1");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Dock2");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Dock3");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Dock4");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/NorthBoeys");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/EastBoeys");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/SouthBoeys");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/WestBoeys");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Cove1");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Cove2");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Cove3");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Cove4");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Tent1");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Tent2");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Tent3");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Tent4");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Chest");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/ChestLid");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TEST_Seaweed001");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TEST_Seaweed002");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TEST_Seaweed003");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TEST_Seaweed004");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TEST_Seaweed005");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TEST_Seaweed006");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Seaweed");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Rocks");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Trees");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/IslandShrubLayout");
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Sunkenshipslayout");

            RegisterResource(ResourceType.RT_MODEL, "Content/Models/AITarget");
            
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Tower" );
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/TowerBase" );
            RegisterResource(ResourceType.RT_MODEL, "Content/Models/Cannon" );

            //Shaders
            RegisterResource(ResourceType.RT_EFFECT, "Content/Shaders/BasicEffect");

            //Textures
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_ppAmber");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_ppBlack");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_ppBlue");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_ppMoldy");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_ppRed");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_ppIndigo");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/endTurnBG" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/cannonBall" );

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\SlideBack");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\SlideCursor");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Tower");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Boat");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Cannon");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Influence");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Coin");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Paused");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\Compass");

            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD__0003_rings");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_glow");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_glow2");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_miniRing");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_GoldDisplay");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_psBoatDown" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_psBoatUp" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_psBuy" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_psGold" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_psEmpty" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/thubstickAni2" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_woodBacking" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Player Status/HUD_miniRingHalf" );

            RegisterResource(ResourceType.RT_TEXTURE, "Content/Menu/MenuBack" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/Menu/ControlsDisplay" );

            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/MovementBar/MovementBarBacking");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/MovementBar/MovementBarBorder");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/MovementBar/MovementBarWater" );

            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/HUD_cannon");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/HUD_fuse");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/PauseBox");

            // XBOX Buttons
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsA" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsAcrop" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsB" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsBcrop" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsX" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsY" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsYcrop");
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonsGuide" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerButtonBack" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerButtonStart" );
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\HUD\XBOX Buttons\xboxControllerButtonStartCrop");
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerDPad" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerDPadDown" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerDPadLeft" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerDPadRight" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerDPadUp" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerLeftShoulder" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerLeftThumbstick" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerLeftTrigger" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerRightShoulder" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerRightThumbstick" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxControllerRightTrigger" );
            RegisterResource( ResourceType.RT_TEXTURE, "Content/HUD/XBOX Buttons/xboxButtonTip" );

            RegisterResource(ResourceType.RT_TEXTURE, "Content/Textures/TerrainHeightMap");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/Textures/Tent Diffuse Map");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/Textures/coveFlagDiffuseMap");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/Textures/BoatDiffuseMap");

            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Win/win_duke" );
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Win/win_irongut");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Win/win_scarlet");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Win/win_indigo");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Win/win_amber");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Win/win_moldy");
            RegisterResource(ResourceType.RT_TEXTURE, "Content/HUD/Win/coin_anim" );

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\HUD\Turn Display\TurnBG");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\HUD\Turn Display\TurnInfinity");

            RegisterResource(ResourceType.RT_FONT, @"Content\Font\PirateFont");
            RegisterResource(ResourceType.RT_FONT, @"Content\Font\PirateFontMedium");
            RegisterResource(ResourceType.RT_FONT, @"Content\Font\PirateFontLarge" );
            RegisterResource(ResourceType.RT_FONT, @"Content\Font\PirateFontExtraLarge");
            RegisterResource(ResourceType.RT_FONT, @"Content\Font\DebugFont");

            #endregion
        }

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public override void Initialize()
        {
            m_camera = new Camera();
            m_world = new World();
            m_TurnManager = new TurnManager(m_world);
            m_hud = new HUD();
            m_winScreen = new WinScreen();
            m_collision = new Collision();
            m_cClouds = new Clouds();

            ParentApp.SoundPlayer.StopAll();

            //Get the selected settings from the app
            m_GameSettings = ((ScallyWagsApp)ParentApp).TheGameSettings;
            //m_GameSettings.NumPlayers = 2;
            m_bIsPaused = false;

            m_phPauser = new PauseHandler();
            //TraceGameSettings();

            m_phPauser.Initialize(ParentApp.SoundPlayer, this);

            Error.Trace("Game Module Init");

            ParentApp.PostProcessEffect = 0;
            //LeaderBoard = "";
            m_isGameOver = false;
            InitialPortGold = 0;

            m_sb = new SpriteBatch(ParentApp.Device);

            m_world.Init(Shaders["BasicEffect"]);

            /////////////////////////////////
            //Camera Init
            m_camera.Position = new Vector3(-1000, Settings.CAMERA_HEIGHT, -1000);
            m_camera.View = Vector3.Zero;
            m_camera.CameraInit(ParentApp.Graphics.GraphicsDevice.Viewport.Width, ParentApp.Graphics.GraphicsDevice.Viewport.Height);

            //////////////////////////////////
            //Init shaders
            Shaders["BasicEffect"].CurrentTechnique = Shaders["BasicEffect"].Techniques["BasicTech"];
            Effect WaterEffect = Shaders["BasicEffect"].Clone(ParentApp.Device);
            WaterEffect.CurrentTechnique = WaterEffect.Techniques["WaterTech"];

            Object3D objTerrain = new Object3D(Models["TerrainTex"]);
            /////////////////////////////////////
            //Create Game Objects
            Object3D objTreesOutside = new Object3D(Models["Trees"]);
            Object3D objShrubsIsland = new Object3D(Models["IslandShrubLayout"]);
            m_world.AddGameObject(objShrubsIsland, true);
            Object3D objSeaweed = new Object3D(Models["Seaweed"]);
            m_world.AddGameObject(objSeaweed, true);
            Object3D objRocks = new Object3D(Models["Rocks"]);
            m_world.AddGameObject(objRocks, true);
            Object3D objSunkenShip = new Object3D(Models["Sunkenshipslayout"]);
            m_world.AddGameObject(objSunkenShip, true);
            Object3D objTrees = new Object3D(Models["IslandTrees"]);
            Object3D objSky = new Object3D(Models["SkyHorizon"]);
            Object3D objTownDock1 = new Object3D(Models["Dock1"]);
            Object3D objTownDock2 = new Object3D(Models["Dock2"]);
            Object3D objTownDock3 = new Object3D(Models["Dock3"]);
            Object3D objTownDock4 = new Object3D(Models["Dock4"]);
            Object3D objNorthBoeys = new Object3D(Models["NorthBoeys"]);
            objNorthBoeys.Color = Color.Black;
            Object3D objEastBoeys = new Object3D(Models["EastBoeys"]);
            objEastBoeys.Color = Color.Black;
            Object3D objSouthBoeys = new Object3D(Models["SouthBoeys"]);
            objSouthBoeys.Color = Color.Black;
            Object3D objWestBoeys = new Object3D(Models["WestBoeys"]);
            objWestBoeys.Color = Color.Black;

            Object3D objTownHouse1 = new Object3D(Models["HousesRed"]);
            m_world.AddGameObject(objTownHouse1, true);
            Object3D objTownHouse2 = new Object3D(Models["HousesOrange"]);
            m_world.AddGameObject(objTownHouse2, true);
            Object3D objTownHouse3 = new Object3D(Models["HousesPink"]);
            m_world.AddGameObject(objTownHouse3, true);

            List<Vector3> WaveLocations = new List<Vector3>();
            WaveLocations.Add(new Vector3(0.0f, 0.0f, 0.0f));
            WaveLocations.Add(new Vector3(-300.0f, 0.0f, 300.0f));
            WaveLocations.Add(new Vector3(300.0f, 0.0f, -300.0f));
            WaveLocations.Add(new Vector3(-300.0f, 0.0f, -300.0f));
            m_world.WaveLocations = WaveLocations;

            Water theWater = new Water(Models["waterTex"], WaveLocations);
            theWater.Location = new Vector3(0, 0.0f, 0);

            Targetter Target; Target = new Targetter(Models["TargetingSelector4"], WaveLocations, true);

            //Create the town and the ports within
            m_Town = new Town(Models["Lighthouse"], Models["Coin"], 4, Shaders["BasicEffect"]);

            Model[] vTowerModels = { Models["Tower"], Models["TowerBase"], Models["TowerBase"] };
            m_Town.InitPorts(vTowerModels, Models[ "Cannon" ] );

            foreach (Port port in m_Town.Ports)
            {
                InitialPortGold += port.Coins;
                port.Shaders.Add(Shaders["BasicEffect"]);
                port.Color = Color.White;
            }
            foreach (Tower tower in m_Town.Towers)
            {
                Targetter PortTargets = new Targetter(Models["TargetCircle"], WaveLocations, false);
                PortTargets.Location = tower.Location;
                PortTargets.Y += 1.5f; //Ground Offset
                PortTargets.Scale = 0.5f;
                PortTargets.Colour = Color.White;
                m_world.AddGameObject(PortTargets, true);
            }

            Arc FiringArc = new Arc(Models["FiringArchOrb"], m_world);
            FiringArc.SetTicks(Settings.NUM_ARC_TICKS);
            FiringArc.Enabled = true;

            m_world.TheTown = m_Town;

            ///////////////////////////
            //Initialize the players

            m_Players = new Player[m_GameSettings.NumPlayers];
            Object3D AITarget = new Object3D(Models["AITarget"]);

            for (int i = 0; i < m_GameSettings.NumPlayers; ++i)
            {
                m_Players[i] = new Player(i, false, m_GameSettings.Characters[i].color, Models["TargetingSelector2"], Models["Coin"], m_world, m_Players, m_Town,AITarget);
                m_Players[i].CharacterID = m_GameSettings.Characters[i].nCharacterID;
                m_Players[i].Name = m_GameSettings.Characters[i].strName;
                Ship ship;
                switch (m_Players[i].CharacterID)
                {
                    case 0:
                        ship = new Ship(Models["Boat"], Models["BoatGold"], i, Textures["BoatDiffuseMap"]);
                        break;
                    case 1:
                        //ship = new Ship(Models["BelgiumSmack"], i);
                        ship = new Ship(Models["Boat"], Models["BoatGold"], i, Textures["BoatDiffuseMap"]);
                        break;
                    case 2:
                        //ship = new Ship(Models["ScottishBoat"], i);
                        ship = new Ship(Models["Boat"], Models["BoatGold"], i, Textures["BoatDiffuseMap"]);
                        break;
                    case 3:
                        //ship = new Ship(Models["JapWarBoat"], i);
                        ship = new Ship(Models["Boat"], Models["BoatGold"], i, Textures["BoatDiffuseMap"]);
                        break;
                    default:
                        ship = new Ship(Models["Boat"], Models["BoatGold"], i, Textures["BoatDiffuseMap"]);
                        break;
                }
                Port cove = new Port(Models["TargetCyl"]);
                cove.PickupRadius = 7.5f;
                cove.Color = m_Players[i].Color;
                ship.Color = m_Players[i].Color;

                m_world.AddShip(ship);
                m_world.AddCove(cove);

                m_Players[i].addShipRef(ship, m_world);
                m_Players[i].Port = cove;
                m_Players[i].setGeneralHeatLevel( m_Town.Ports[0].Coins );
                ChestOpen objCoveChestLid = new ChestOpen(Models["ChestLid"]);
                
                switch (i)
                {
                    case 0:
                        objCoveChestLid.Location = new Vector3(279.326f + 1.25f, 2.875f + 2.108f, 260.234f + 1.408f);
                        objCoveChestLid.Yaw = ((float)Math.PI / 180) * 45f;
                        break;
                    case 1:
                        objCoveChestLid.Location = new Vector3(-282.575f - 1.75f, 1.503f + 2.108f, 253.077f + 1.008f);
                        objCoveChestLid.Yaw = ((float)Math.PI / 180) * (120f + 180f);
                        break;
                    case 2:
                        objCoveChestLid.Location = new Vector3(-287.616f - 1.65f, 3.082f + 2.108f, -254.378f - 1.208f);
                        objCoveChestLid.Yaw = ((float)Math.PI / 180) * (55f + 180f);
                        break;
                    case 3:
                        objCoveChestLid.Location = new Vector3(284.955f + 1.45f, 2.875f + 2.108f, -243.671f - 1.408f);
                        objCoveChestLid.Yaw = ((float)Math.PI / 180) * 135f;
                        break;
                }
                m_Players[i].ChestLid = objCoveChestLid;
                m_world.AddGameObject(objCoveChestLid,true);
            }

            m_TurnManager.Init(ParentApp.Inputs, m_Players, Target, m_collision, m_camera, m_hud, Models["Coin"], FiringArc, Models["CannonBall"], this);

            ///////////////////////////////
            //Add the objects to the containers that need to know about them

            m_world.AddGameObject(objTerrain, true);
            if (Settings.FAST_MODE == false)
            {
                m_world.AddGameObject(objTrees, true);
                //Need to remove the sky from the lighting and shaders
                //m_world.AddGameObject(objSky, false);

                //Initializing Player Ports
                if (m_GameSettings.NumPlayers > 0)
                {//Lid-Z: -1.908 from Chest Location. //Lid-Y: 2.108 from Chest Locations.
                    Object3D objCoveTent4 = new Object3D(Models["Tent4"], Textures["Tent Diffuse Map"]);
                    objCoveTent4.Color = m_GameSettings.Characters[0].color;
                    Object3D objCoveFlag4 = new Object3D(Models["Cove4"], Textures["coveFlagDiffuseMap"]);
                    objCoveFlag4.Color = m_GameSettings.Characters[0].color;
                    Object3D objCoveChest4 = new Object3D(Models["Chest"]);                    
                    objCoveChest4.Location = new Vector3(279.326f, 2.875f, 260.234f);
                    objCoveChest4.Yaw = ((float)Math.PI / 180) * 45f;//3DSMax-Z
                    //objCoveChest4Lid.Pitch = ((float)Math.PI / 180) * 45f; //Will be openned
                    m_world.AddGameObject(objCoveFlag4, true);
                    m_world.AddGameObject(objCoveTent4, true);
                    m_world.AddGameObject(objCoveChest4, true);
                }
                if (m_GameSettings.NumPlayers > 1)
                {
                    Object3D objCoveTent3 = new Object3D(Models["Tent3"], Textures["Tent Diffuse Map"]);
                    objCoveTent3.Color = m_GameSettings.Characters[1].color;
                    Object3D objCoveFlag3 = new Object3D(Models["Cove3"], Textures["coveFlagDiffuseMap"]);
                    objCoveFlag3.Color = m_GameSettings.Characters[1].color;
                    Object3D objCoveChest3 = new Object3D(Models["Chest"]);
                    objCoveChest3.Location = new Vector3(-282.575f, 1.503f, 253.077f);
                    objCoveChest3.Yaw = ((float)Math.PI / 180) * (120f + 180f);
                    //objCoveChest3Lid.Pitch = ((float)Math.PI / 180) * 45f; //Will be openned
                    m_world.AddGameObject(objCoveTent3, true);
                    m_world.AddGameObject(objCoveFlag3, true);
                    m_world.AddGameObject(objCoveChest3, true);
                }
                if (m_GameSettings.NumPlayers > 2)
                {
                    Object3D objCoveTent2 = new Object3D(Models["Tent2"], Textures["Tent Diffuse Map"]);
                    objCoveTent2.Color = m_GameSettings.Characters[2].color;
                    Object3D objCoveFlag2 = new Object3D(Models["Cove2"], Textures["coveFlagDiffuseMap"]);
                    objCoveFlag2.Color = m_GameSettings.Characters[2].color;
                    Object3D objCoveChest2 = new Object3D(Models["Chest"]);
                    Object3D objCoveChest2Lid = new Object3D(Models["ChestLid"]);
                    objCoveChest2.Location = new Vector3(-287.616f, 3.082f, -254.378f);
                    objCoveChest2.Yaw = ((float)Math.PI / 180) * (55f + 180f);
                    //objCoveChest2Lid.Pitch = ((float)Math.PI / 180) * 45f; //Will be openned
                    m_world.AddGameObject(objCoveTent2, true);
                    m_world.AddGameObject(objCoveFlag2, true);
                    m_world.AddGameObject(objCoveChest2, true);
                }
                if (m_GameSettings.NumPlayers > 3)
                {
                    Object3D objCoveTent1 = new Object3D(Models["Tent1"], Textures["Tent Diffuse Map"]);
                    objCoveTent1.Color = m_GameSettings.Characters[3].color;
                    Object3D objCoveFlag1 = new Object3D(Models["Cove1"], Textures["coveFlagDiffuseMap"]);
                    objCoveFlag1.Color = m_GameSettings.Characters[3].color;
                    Object3D objCoveChest1 = new Object3D(Models["Chest"]);
                    objCoveChest1.Location = new Vector3(284.955f, 2.875f, -243.671f);
                    objCoveChest1.Yaw = ((float)Math.PI / 180) * 135f;
                    //objCoveChest1Lid.Pitch = ((float)Math.PI / 180) * 45f; 
                    m_world.AddGameObject(objCoveTent1, true);
                    m_world.AddGameObject(objCoveFlag1, true);
                    m_world.AddGameObject(objCoveChest1, true);
                }
                m_world.AddGameObject(objTownDock1, true);
                m_world.AddGameObject(objTownDock2, true);
                m_world.AddGameObject(objTownDock3, true);
                m_world.AddGameObject(objTownDock4, true);
                m_world.AddGameObject(objNorthBoeys, false);
                m_world.AddGameObject(objEastBoeys, false);
                m_world.AddGameObject(objSouthBoeys, false);
                m_world.AddGameObject(objWestBoeys, false);
                m_world.AddGameObject(objTreesOutside, true);

                objNorthBoeys.Shaders.Add(WaterEffect);
                objWestBoeys.Shaders.Add(WaterEffect);
                objSouthBoeys.Shaders.Add(WaterEffect);
                objEastBoeys.Shaders.Add(WaterEffect);

            }
            //Drawn last for blending
            m_world.AddBlendedGameObject(Target, true);
            m_world.AddBlendedGameObject(theWater, true);

            /////////////////////////////////////
            // HUD components
            m_hud.Init(ParentApp.Graphics.GraphicsDevice.Viewport.Width,
                        ParentApp.Graphics.GraphicsDevice.Viewport.Height,
                        m_sb,
                        m_TurnManager.Players,
                        m_world, m_TurnManager, Textures, Fonts);

            m_hud.TheTurnDisplay.MaxTurn = m_GameSettings.TurnLimit;
            Object3D objSign = new Object3D( Models[ "TargetCircle" ] );
            m_collision.Init(ParentApp.Device, objTerrain,objSky, @"Content\Textures\TerrainHeightMap.bmp",ParentApp,m_Players, Textures[ "TerrainHeightMap" ]);
            
            m_buttonText    = new ButtonIndicator( Textures );

            /////////////////////////////////////////////////
            //Play BG sounds / music
            SoundPlayer.PlayLoop("wearepirates", -1);
            SoundPlayer.PlayLoop("birdNoise", -1);

            if (Settings.SHOW_INTRO)
                CameraIntro();

            //Particle Prep
            m_cClouds.Init(this);

            m_lstParticles = new List<ParticleSystem>();
            ParticleSystem m_Wake = new ParticleSystem(ParentApp, ParentApp.Content, "Wake");
            m_lstParticles.Add(m_Wake);

            ////////////////////////////////////////////
            //Initialize the win screen

            //Calculate the safe area to draw the win screen
            int nViewWidth      = ParentApp.Graphics.GraphicsDevice.Viewport.Width;
            int nViewHeight     = ParentApp.Graphics.GraphicsDevice.Viewport.Height;
            int nBorderWidth    = (int)( nViewWidth * Settings.SAFE_REGION_PERCENTAGE );
            int nBorderHeight   = (int)( nViewHeight * Settings.SAFE_REGION_PERCENTAGE );

            m_winScreen.Init(
                 nBorderWidth,
                 nBorderHeight,
                 nViewWidth - 2 * nBorderWidth,
                 nViewHeight - 2 * nBorderHeight,
                 ParentApp.Graphics.GraphicsDevice,
                 Textures,
                 Fonts[ "PirateFontLarge" ],
                 Fonts[ "PirateFont" ] );
        }

        /// <summary>
        /// Set the camera rotation to that of the targeter
        /// </summary>
        public void SetCameraToTargeter()
        {
            m_camera.Rotation = -(m_TurnManager.Targetter.TargetterLocation);// + MathHelper.Pi ); 
        }

        /** @fn     MODULE_IDENTIFIER Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @return the ID of the module to switch to, or ( MID_THIS {0} ) if this module is to continue executing
         *  @param  fTotalTime [in] the total elapsed time since the app began running, in seconds
         *  @param  fElapsedTime [in] the time elapsed since the last frame
         */
        public override MODULE_IDENTIFIER Update(double fTotalTime, float fElapsedTime)
        {
            m_world.Update(fElapsedTime);

            if (ParentApp.Inputs.IsKeyPressed(Keys.M))
                CameraIntro();

            if (m_phPauser.Update(fElapsedTime) == false)
                return MODULE_IDENTIFIER.MID_MENU_MODULE;

            ////////////////////
            //Update shaders
            Shaders["BasicEffect"].Parameters["g_fTime"].SetValue((float)fTotalTime);
  
            if (!m_phPauser.IsPaused)
            {          
                /////////////////////
                //Update HUD
                if (m_TurnManager.TotalTurns <= m_GameSettings.TurnLimit || m_GameSettings.TurnLimit == -1)
                    m_hud.TheTurnDisplay.CurrentTurn = m_TurnManager.TotalTurns;
                else
                    m_hud.TheTurnDisplay.CurrentTurn = m_GameSettings.TurnLimit;

                if (ParentApp.Inputs.ControllerIdleTime[m_TurnManager.Turn] < ParentApp.Inputs.KeyIdleTime)
                    m_hud.Update(fElapsedTime, m_TurnManager.IsTownTurn, ParentApp.Inputs.ControllerIdleTime[m_TurnManager.Turn]);
                else
                    m_hud.Update(fElapsedTime, m_TurnManager.IsTownTurn, ParentApp.Inputs.KeyIdleTime);


                ParentApp.PostProcessEffect = 0;

                //Particle Prep
                foreach (ParticleSystem particle in m_lstParticles)
                {
                    particle.UpdateEffect(Vector3.Zero, fElapsedTime);
                
                }
                m_cClouds.Update(fElapsedTime);

                m_camera.FollowTarget(m_TurnManager.CameraTarget);

                m_camera.Update(fElapsedTime);

                if (!m_isGameOver)
                {
                    //TEMP - Switch to the next module on enter key press
                    //if (ParentApp.Inputs.IsKeyPressed(Keys.Enter))
                        //return MODULE_IDENTIFIER.MID_CREDITS_MODULE;

                    if (!m_camera.IsBusy)
                    {
                        m_hud.ButtonDisplay.Enabled = true;
                        m_TurnManager.RunTurn(fElapsedTime, SoundPlayer, m_lstParticles);
                    }
                    else
                    {
                        m_hud.ButtonDisplay.Enabled = false;
                        for (int i = 0; i < m_Players.Length; i++)
                        {
                            if (ParentApp.Inputs.IsButtonPressed(i, Buttons.A))
                            {
                                m_camera.ClearScript();
                            }
                        }
                    }

                    checkEndGameState();
                    //////////////////////
                    //Debug
                    if (ParentApp.Inputs.IsKeyPressed(Keys.F3))
                        m_bUseDebugCamera = !m_bUseDebugCamera;

                    if (ParentApp.Inputs.IsKeyPressed(Keys.N))
                    {
                        foreach (ParticleSystem particle in m_lstParticles)
                        {
                            particle.Active = !particle.Active;
                        }
                    }
                }
                else //Game is over
                {
                    if (!m_camera.IsBusy)
                    {
                        m_TurnManager.EndGame(fElapsedTime, SoundPlayer, m_lstParticles, GetWinningPlayerID() );
                    }                    
                }
            }
            else
            {
                ///////////////////////////////////
                //Game is paused
                ParentApp.PostProcessEffect = 1;

                //TEMP - Switch to the next module on enter key press
                if (ParentApp.Inputs.IsKeyPressed(Keys.Enter) || ParentApp.Inputs.IsButtonPressed(m_TurnManager.Turn, Buttons.Back))
                    return MODULE_IDENTIFIER.MID_MENU_MODULE;
            }

            ///////////////////////////////////////////////////
            //Check for pause key press so long as the end turn popup isn't active
            if( m_TurnManager.EndTurn == false )
            {
                
                for (int i = 0; i < m_Players.Length; i++)
                {
                    if (ParentApp.Inputs.IsButtonPressed(i, Buttons.Start) && m_isGameOver == false)
                    {

                        if (m_bIsPaused && m_phPauser.WhoPaused == i)
                        {
                            m_phPauser.IsPaused = !m_phPauser.IsPaused;
                            m_bIsPaused = m_phPauser.IsPaused;
                            SetCameraToTargeter();
                            m_hud.ButtonDisplay.Kill();
                        }
                        else if (!m_bIsPaused)
                        {
                            m_phPauser.WhoPaused = i;
                            m_phPauser.IsPaused = !m_phPauser.IsPaused;
                            m_bIsPaused = m_phPauser.IsPaused;
                            ParentApp.SoundPlayer.PlayCue("pageTurn");
                            m_hud.ButtonDisplay.QueueButtonToDraw("xboxButtonTip");
                        }

                        break;
                    }
                }

                if (ParentApp.Inputs.IsKeyPressed(Keys.P))
                {
                    m_phPauser.IsPaused = !m_phPauser.IsPaused;
                    m_bIsPaused = m_phPauser.IsPaused;
                    if (m_bIsPaused)
                    {
                        ParentApp.SoundPlayer.PlayCue("pageTurn");
                        m_hud.ButtonDisplay.QueueButtonToDraw("xboxButtonTip");
                    }
                    else
                    {
                        SetCameraToTargeter();
                        //m_camera.Rotation = -( m_TurnManager.Targetter.TargetterLocation );// + MathHelper.Pi ); 
                        m_hud.ButtonDisplay.Kill();
                    }
                }
            }

            //When the game is over... do this.
            if( m_isGameOver )
            {
                m_winScreen.Update( fElapsedTime );

                if( m_winScreen.WinComplete )
                {
                    //return to the menu if enter is pressed
                    if (ParentApp.Inputs.IsKeyPressed(Keys.Enter))
                        return MODULE_IDENTIFIER.MID_MENU_MODULE;

                    //Go to credits when back button is hit on any controller
                    int nWinID = GetWinningPlayerID();

                    //Check if the winning player was AI
                    if( m_Players[ nWinID ].IsAI == true )
                    {
                        //Check all player's start buttons
                        for( int i = 0; i < m_Players.Length; ++i )
                        {
                            if (ParentApp.Inputs.IsButtonPressed(i, Buttons.Start))
                                return MODULE_IDENTIFIER.MID_MENU_MODULE;
                        }
                    }
                    else
                    {
                        //Other wise, only let the winning player press start
                        if( ParentApp.Inputs.IsButtonPressed( nWinID, Buttons.Start ) )
                        {
                            return MODULE_IDENTIFIER.MID_MENU_MODULE;
                        }
                    }
                }
            }


            ///////////////////////////
            //Temp... reset the game
            if( ParentApp.Inputs.IsKeyPressed(Keys.F12) )
                this.Initialize();

            return MODULE_IDENTIFIER.MID_THIS;    //Continue running the module.
        }

        /*  @fn checkEndGameState
         *  @brief Checks the status of all the players to see if anyone has won yet.
         */
        private void checkEndGameState() 
        {
            ///////////////////////////////
            //Check gold limits
            if (m_GameSettings.EndOnTotalPillage)
            {
                int TotalGoldReturned = 0;
                foreach (Player player in m_Players)
                {
                    TotalGoldReturned += player.Port.Coins;

                    //Account for the 1 gold spent by each player on each extra ship
                    for( int i = 1; i < player.Ships.Length; ++i )
                    {
                        if( player.Ships[ i ] != null )
                        {
                            TotalGoldReturned++;
                        }
                    }
                }
                if (TotalGoldReturned >= InitialPortGold)
                {
                    m_isGameOver = true;
                }
            }
            else if (m_GameSettings.GoldLimit > 0)
            {
                foreach (Player player in m_Players)
                {
                    if (player.Port.Coins >= m_GameSettings.GoldLimit)
                    {
                        m_isGameOver = true;                        
                    }
                }
            }
            
            ////////////////////////////
            //Check turn limit
            if (m_GameSettings.TurnLimit > 0)
            {
                //Did the final turn just finish?
                if (m_TurnManager.TotalTurns > m_GameSettings.TurnLimit)
                {
                    m_isGameOver = true;
                }
            }
          
            //Determine the winner if the game is over
            if (m_isGameOver == true)
            {
                m_TurnManager.FinalizeStats();

                //GenerateLeaderBoard();
                m_winScreen.GenerateStatistics(m_Players, Textures, GetWinningPlayerID());

                m_TurnManager.PrepEndGame(GetWinningPlayerID(), m_lstParticles);
            }
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void Draw(GraphicsDevice device, GameTime gameTime)
        {
            device.Clear(Color.SkyBlue);

            Matrix matView = m_camera.ViewMatrix;

            if( m_bUseDebugCamera )
                matView = m_camera.DebugView;

            //device.RenderState.FillMode = FillMode.WireFrame;
            m_world.Draw(device, matView, m_camera.ProjectionMatrix, m_camera.Position);
            //device.RenderState.FillMode = FillMode.Solid;
            m_collision.Draw(device,matView,m_camera.ProjectionMatrix);
            foreach (Player m in m_Players) {
                if (m.Active) {
                    m.Draw(device, matView, m_camera.ProjectionMatrix, m_camera.Position);
                }
            }

            if (Settings.DETECT_EDGES == false)
            {
                foreach (ParticleSystem particle in m_lstParticles)
                    particle.Draw(matView, m_camera.ProjectionMatrix, gameTime);
                m_cClouds.Draw(matView, m_camera.ProjectionMatrix, gameTime);
            }
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene Hud Aspects without Edges
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void DrawNonEdgeDetectedFeatures(GraphicsDevice device, GameTime gameTime)
        {
            Matrix matView = m_camera.ViewMatrix;

            if (m_bUseDebugCamera)
                matView = m_camera.DebugView;

            m_collision.Draw(device, matView, m_camera.ProjectionMatrix);

            SpriteBatch sb = new SpriteBatch(device);

            if( m_isGameOver )
            {
                sb.Begin();
                m_winScreen.Draw(sb, Fonts["PirateFontExtraLarge"]);
                sb.End();
            }
            else
            {
                m_hud.Draw(device, m_TurnManager.IsTownTurn);
            }

            if (m_phPauser.IsPaused)
            {
                //spin the camera while game is paused
                m_camera.Rotate( (float)ParentApp.TargetElapsedTime.Milliseconds * 0.00005f );
                m_camera.Update( (float)ParentApp.TargetElapsedTime.Milliseconds / 1000.0f );

                sb.Begin();

                m_phPauser.Draw(sb);

                sb.End();
            }


            if( m_TurnManager.EndTurn == true )
            {
                m_hud.ButtonDisplay.Kill();
                SpriteFont  pirateFont  = Fonts[ "PirateFont" ];
                Texture2D   texBG       = Textures[ "endTurnBG" ];
                //Texture2D   texControl  = Textures[ "ControlsDisplay" ];
                string      strMsg1     = "End Turn";
                string      strMsg2     = "Press A to end turn";
                string      strMsg3     = "Press B to resume";

                //Center the bg image
                Vector2 vPauseBoxLoc = Vector2.Zero;
                vPauseBoxLoc.X = ParentApp.Device.Viewport.Width * 0.5f - texBG.Width * 0.5f;
                vPauseBoxLoc.Y = ParentApp.Device.Viewport.Height * 0.5f - texBG.Height * 0.5f;

                //Center the strings on the background image
                Vector2 vStrDim = pirateFont.MeasureString( strMsg1 );

                Vector2 vLine1Loc = Vector2.Zero;
                vLine1Loc.X = vPauseBoxLoc.X + texBG.Width * 0.5f - vStrDim.X * 0.5f;
                vLine1Loc.Y = vPauseBoxLoc.Y + texBG.Height /3 * 0.5f;

                vStrDim = pirateFont.MeasureString(strMsg2);

                Vector2 vLine2Loc = Vector2.Zero;
                //Line2Loc.X = vPauseBoxLoc.X + texBG.Width * 0.5f - vStrDim.X + 15;// * 0.5f;
                vLine2Loc.X = ( ParentApp.Device.Viewport.Width - texBG.Width ) / 2;
                vLine2Loc.Y = ( ParentApp.Device.Viewport.Height - texBG.Height ) / 2;

                vStrDim = pirateFont.MeasureString( strMsg3 );

                Vector2 vLine3Loc = Vector2.Zero;
                vLine3Loc.X = ( ParentApp.Device.Viewport.Width - texBG.Width ) / 2;
                vLine3Loc.Y = ( ParentApp.Device.Viewport.Height - texBG.Height ) / 2 + texBG.Height * 0.3f;

                Rectangle rectDraw = new Rectangle();

                sb.Begin();
                sb.Draw( texBG, vPauseBoxLoc, Color.White );
                //sb.DrawString(pirateFont, strMsg1, new Vector2( vLine1Loc.X + 1.0f, vLine1Loc.Y + 1.0f ), Color.DarkRed );
                
                sb.DrawString( pirateFont, strMsg1, vLine1Loc, Color.White );
                //sb.DrawString(pirateFont, strMsg2, new Vector2(vLine2Loc.X + 1.0f, vLine2Loc.Y + 1.0f), Color.DarkRed);
                //sb.DrawString( pirateFont, strMsg2, vLine2Loc, Color.White );
                rectDraw = new Rectangle( (int)vLine2Loc.X, (int)vLine2Loc.Y, texBG.Width, texBG.Height );
                m_buttonText.SetText( "Press /A to end turn", Fonts[ "PirateFont" ], rectDraw, Color.White );  
                m_buttonText.Draw( sb );

                rectDraw = new Rectangle( (int)vLine3Loc.X, (int)vLine3Loc.Y, texBG.Width, texBG.Height );
                m_buttonText.SetText( "Press /B to resume", Fonts[ "PirateFont" ], rectDraw, Color.White );
                m_buttonText.Draw( sb );

                //sb.DrawString( pirateFont, strMsg3, vLine3Loc, Color.White );

                Vector2 vControlTiploc = Vector2.Zero;
                vControlTiploc.X       = vPauseBoxLoc.X + texBG.Width / 6 * 1.0f;
                vControlTiploc.Y       = vPauseBoxLoc.Y + texBG.Height / 5 * 1.0f;
                //draw the control tip during pause
               // sb.Draw( texControl, new Rectangle( (int)vControlTiploc.X, (int)vControlTiploc.Y, texControl.Width, texControl.Height ), Color.White );
                sb.End();
            }
        }

        /** @fn     void ShutDown()
         *  @brief  clean up the modules resources
         */
        public override void ShutDown()
        {
            m_hud.Clear();
            m_world.Clear();

            foreach (Player player in m_TurnManager.Players)
                player.Clear();

            base.ShutDown();
        }

        /** @fn     void TraceGameSettigns()
         *  @brief  debug function to output the current game settigns to the console
         */
        private void TraceGameSettings()
        {
            Error.Trace( "End on total pillage " + m_GameSettings.EndOnTotalPillage.ToString() );
            Error.Trace( "Gold Limit: " + m_GameSettings.GoldLimit.ToString() );
            Error.Trace( "Turn Limit: " + m_GameSettings.TurnLimit.ToString() );

            Error.Trace( "Num Players: " + m_GameSettings.NumPlayers.ToString() );

            for( int i = 0; i < m_GameSettings.Characters.Length; ++i )
            {
                Error.Trace( "Player " + i );
                Error.Trace( "\tCharacter ID: " + m_GameSettings.Characters[i].nCharacterID );
                Error.Trace( "\tName: " + m_GameSettings.Characters[i].strName );
                Error.Trace( "\tAI controlled: " + m_GameSettings.Characters[ i ].bAiControlled );
                Error.Trace( "\tColour: " + m_GameSettings.Characters[ i ].color.ToString() );
            }
        }

        private void CameraIntro()
        {
            CameraWaypoint Point;
            Point.m_Position = new Vector3(-1000.0f, 75.0f, -1000.0f);
            Point.m_Target = new Vector3(0.0f, 0.0f, 0.0f);
            m_camera.AddCameraScript(Point);

            Point.m_Position = new Vector3(100.0f, 55.0f, -100.0f);
            Point.m_Target = new Vector3(25.0f, 0.0f, 25.0f);
            m_camera.AddCameraScript(Point);

            Point.m_Position = new Vector3(300.0f, 40.0f, 300.0f);
            Point.m_Target = new Vector3(0.0f, 0.0f, 0.0f);
            m_camera.AddCameraScript(Point);

            m_camera.RunScript(7.5f);
        }

        /// <summary>
        /// Calculate the ID of the winning player
        /// </summary>
        /// <returns> the winning player's ID </returns>
        private int GetWinningPlayerID()
        {
            int WinningPlayer = 0;
            int TopGold = 0;
            for (int i = 0; i < m_Players.Length; i++)
            {
                if (m_Players[i].Port.Coins > TopGold)
                {
                    WinningPlayer = i;
                    TopGold = m_Players[i].Port.Coins;
                }
            }

            return WinningPlayer;
        }
    }
}
