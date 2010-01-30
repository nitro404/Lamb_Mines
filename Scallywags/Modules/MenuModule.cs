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

namespace Scallywags
{
    public enum GameMenuID : int
    {
        GMN_THIS        =   0,//Don't change menu
        GMN_MAINMENU    =   1,
        GMN_OPTIONS     =   2,
        GMN_PLAYERSELECT=   3,
        GMN_CONTROLS    =   4,
        GMN_STARTGAME   =   5,
        GMN_CREDITS     =   6,
        GMN_GAMEOPTIONS =   7,
        GMN_HOWTOPLAY   =   8,
        GMN_PAUSEMENU   =   9,
        GMN_UNPAUSE     =   10
    }
    
    /** @class  MenuModule
     *  @brief  the opening menu screen, a type of module
     */
    public class MenuModule : XNAModule
    {
        SwitchInfo          m_switchInfo;       ///< Information about the active menu switch

        Camera              m_camera;           ///< The menu camera
        Water               m_MenuWater;        ///< The menu water
        Object3D            m_objTerrain;       ///< The menu terrain
        GameSettings        m_GameSettings;     ///< The current game settings

        Rectangle           m_rectScreenDraw;   ///< The area of the window the menu screens can draw to

        Dictionary< GameMenuID, MainMenuScreen >    m_dtMenuScreens;    ///< The map of menu screens
        MainMenuScreen                              m_currentScreen;    ///< The current menu screen

        Texture2D           m_texMenuBG;        ///< The 2D menu Background
        Texture2D           m_texDisolve;       ///< The 2d texture for the screen switch effect

        SpriteBatch         m_sb;               ///< The sprite batch for 2D drawing
        ButtonIndicator     m_btnIndicator;     ///< to show what buttons are available
        Particles.ParticleSystem m_ptSmoke;
        Particles.ParticleSystem m_ptSmoke2;
        Object3D TestLocator;
        ReferenceableInt m_iWhoPaused;  
                     
        /** @prop   TheGameSettings
         *  @brief  the application level game settings
         */
        public GameSettings TheGameSettings
        {
            get
            {
                return m_GameSettings;
            }
        }

        /** @fn     MenuModule()
         *  @brief  constructor
         */
        public MenuModule()
            : base(MODULE_IDENTIFIER.MID_MENU_MODULE)
        {
            m_switchInfo    = null;
            m_camera        = new Camera();
            m_GameSettings  = null;
            m_rectScreenDraw= Rectangle.Empty;
            m_MenuWater     = null;
            m_currentScreen = null;
            m_dtMenuScreens = new Dictionary<GameMenuID,MainMenuScreen>();

            RegisterResource(ResourceType.RT_MODEL, @"Content\Models\water2" );
            RegisterResource(ResourceType.RT_MODEL, @"Content\Models\Menu_Terrain");

            RegisterResource(ResourceType.RT_EFFECT, @"Content\Shaders\BasicEffect");

            RegisterResource(ResourceType.RT_FONT, @"Content\Font\PirateFont" );
            RegisterResource(ResourceType.RT_FONT, @"Content\Font\PirateFontMedium");
            RegisterResource(ResourceType.RT_FONT, @"Content\Font\PirateFontLarge");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\MenuBack" );
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\Banner");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectBG");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectRing");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelPortrait0");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelPortrait1");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelPortrait2");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelPortrait3");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelPortrait4");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelPortrait5");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelPortraitUnknown");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor1");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor2");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor3");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor4");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor1NoGlow");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor2NoGlow");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor3NoGlow");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectCursor4NoGlow");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\CharSelectMiniPortraits");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\SlideBack");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\SlideCursor");

            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\ControlsDisplay" );


            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Tower");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Boat");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Cannon");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Influence");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Textures\Coin");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\Menu\Compass");

            RegisterResource(ResourceType.RT_MODEL, @"Content\Models\Coin");
           
            ////////////////////////////
            //Xbox buttons
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\HUD\XBOX Buttons\xboxButtonsAcrop" );
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\HUD\XBOX Buttons\xboxButtonsBcrop" );
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\HUD\XBOX Buttons\xboxButtonsYcrop");
            RegisterResource(ResourceType.RT_TEXTURE, @"Content\HUD\XBOX Buttons\xboxControllerButtonStartCrop");

            ShowLoadScreen = false;
        }

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public override void Initialize()
        {
            m_dtMenuScreens = new Dictionary<GameMenuID,MainMenuScreen>();
            m_switchInfo    = null;
            m_sb            = new SpriteBatch(ParentApp.Device);
            m_GameSettings  = ( (ScallyWagsApp)ParentApp ).TheGameSettings;
            int nViewWidth  = ParentApp.Device.Viewport.Width;
            int nViewHeight = ParentApp.Device.Viewport.Height;

            m_iWhoPaused = new ReferenceableInt();
            m_iWhoPaused.Integer = 0;

            ////////////////////////////////////////////////
            //Create and initialize the 3D menu assets
            m_camera.Position   = new Vector3( 0, 5.0f, -25.0f );
            m_camera.View       = new Vector3( 0.0f, 0.0f, 0.0f );
            m_camera.Up         = Vector3.Up;

            m_camera.CameraInit( nViewWidth, nViewHeight);

            Vector3 vLightDir = new Vector3( 0, -1, 0 );
            vLightDir.Normalize();

            m_ptSmoke = new Particles.ParticleSystem(ParentApp, ParentApp.Content, "Smoke");
            m_ptSmoke.Location = new Vector3(300.0f,0.0f,500.0f);
            m_ptSmoke2 = new Particles.ParticleSystem(ParentApp, ParentApp.Content, "Smoke");
            m_ptSmoke2.Location = new Vector3(300.0f, 0.0f, 500.0f);

            TestLocator = new Object3D(Models["Coin"]);

            List<Vector3> WaveLocations = new List<Vector3>();
            WaveLocations.Add(new Vector3(100.0f, 0.0f, 100.0f));
            WaveLocations.Add(new Vector3(-100.0f, 0.0f, 100.0f));
            WaveLocations.Add(new Vector3(100.0f, 0.0f, -100.0f));
            WaveLocations.Add(new Vector3(-100.0f, 0.0f, -100.0f));

            m_MenuWater = new Water( Models[ "water2" ], WaveLocations );
            m_MenuWater.Shaders.Add( Shaders[ "BasicEffect" ] );
            m_MenuWater.Y = -1.0f;
            m_MenuWater.TechniqueName = "WaterTechNoTex";
            m_MenuWater.Scale   = 1.0f;
            //m_MenuWater.Yaw     = 3.1415f;

            m_objTerrain = new Object3D( Models[ "Menu_Terrain" ] );

            /////////////////////////////////////////////////////////////
            //Create the and initialize 2D Screens and assets
            m_texMenuBG         = Textures[ "MenuBack" ];
            m_texDisolve        = Textures[ "MenuBack" ];

            int nBGScreenX      = ( nViewWidth - m_texMenuBG.Width ) / 2;
            int nBGScreenY      = ( nViewHeight - m_texMenuBG.Height ) / 2;

            m_rectScreenDraw    = new Rectangle( nBGScreenX, nBGScreenY, m_texMenuBG.Width, m_texMenuBG.Height );

            MainMenu mainMenu = new MainMenu(m_iWhoPaused);
            OptionsMenu     optMenu     = new OptionsMenu( ParentApp.SoundPlayer );
            ControlsMenu    contMenu    = new ControlsMenu();
            CharSelectMenu charMenu = new CharSelectMenu(this, m_iWhoPaused);
            GameOptions gameOptMenu = new GameOptions(m_GameSettings, SoundPlayer);
            HowToPlay       howPlayMenu = new HowToPlay(); 
            
            m_currentScreen = mainMenu;

            m_dtMenuScreens.Add(GameMenuID.GMN_MAINMENU, mainMenu );
            m_dtMenuScreens.Add(GameMenuID.GMN_OPTIONS, optMenu );
            m_dtMenuScreens.Add(GameMenuID.GMN_PLAYERSELECT, charMenu );
            m_dtMenuScreens.Add(GameMenuID.GMN_CONTROLS, contMenu);
            m_dtMenuScreens.Add(GameMenuID.GMN_GAMEOPTIONS, gameOptMenu );
            m_dtMenuScreens.Add(GameMenuID.GMN_HOWTOPLAY, howPlayMenu);

            m_currentScreen.Init( ParentApp.Device, Fonts, m_rectScreenDraw, Textures );

            m_btnIndicator = new ButtonIndicator( Textures );
            m_btnIndicator.SetText( "/A Select", Fonts[ "PirateFont" ], m_rectScreenDraw, Color.Black );

            SoundPlayer.PlayLoop("MenuMusic", -1);
            //Play the menu music
            //SoundPlayer.PlayCue( "menu_rough" );
        }

        /** @fn     MODULE_IDENTIFIER Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @return the ID of the module to switch to, or ( MID_THIS {0} ) if this module is to continue executing
         *  @param  fTotalTime [in] the total elapsed time since the app began running, in seconds
         *  @param  fElapsedTime [in] the time elapsed since the last frame
         */
        public override MODULE_IDENTIFIER Update(double fTotalTime, float fElapsedTime)
        {
            MODULE_IDENTIFIER midNext = MODULE_IDENTIFIER.MID_THIS; //Continue running the module unless this is changed.

            ///Locator Test
            if (ParentApp.Inputs.IsKeyDown(Keys.W))
            {
                TestLocator.Z += 1.0f;
            }
            if (ParentApp.Inputs.IsKeyDown(Keys.S))
            {
                TestLocator.Z -= 1.0f;
            }
            if (ParentApp.Inputs.IsKeyDown(Keys.A))
            {
                TestLocator.X += 1.0f;
            }
            if (ParentApp.Inputs.IsKeyDown(Keys.D))
            {
                TestLocator.X -= 1.0f;
            }

            TestLocator.Update(fElapsedTime);

            //Update the shaders
            Shaders["BasicEffect"].Parameters["g_fTime"].SetValue((float)fTotalTime);

            m_ptSmoke.UpdateEffect(new Vector3(0.0f, 10.0f, 0.0f), fElapsedTime);
            m_ptSmoke2.UpdateEffect(new Vector3(0.0f, 5.0f, 0.0f), fElapsedTime);

            if( m_switchInfo != null )
            {
                //Switching screens
                m_switchInfo.fSwitchTimer -= fElapsedTime;

                if( m_switchInfo.fSwitchTimer <= 0 )
                {
                    if( m_switchInfo.bFadeOut )
                    {
                        //Perform the switch
                        m_currentScreen.ShutDown();
                        m_currentScreen = m_dtMenuScreens[ m_switchInfo.gmidNext ];
                        m_currentScreen.Init( ParentApp.Device, Fonts, m_rectScreenDraw, Textures );

                        //Set the button indicator properties for the new screen
                        switch (m_switchInfo.gmidNext)
                        {
                            case GameMenuID.GMN_MAINMENU:
                                m_btnIndicator.SetText( "/A Select", Fonts[ "PirateFont" ], m_rectScreenDraw, Color.Black );
                                break;
                            case GameMenuID.GMN_CONTROLS:
                                m_btnIndicator.SetText("/A How To Play   /B Back To Menu", Fonts["PirateFont"], m_rectScreenDraw, Color.Black );
                                break;
                            case GameMenuID.GMN_GAMEOPTIONS:
                                m_btnIndicator.SetText( "/A Select   /B Cancel", Fonts[ "PirateFont" ], m_rectScreenDraw, Color.Black );
                                break;
                            case GameMenuID.GMN_OPTIONS:
                                m_btnIndicator.SetText( "/A Select   /B Cancel", Fonts[ "PirateFont" ], m_rectScreenDraw, Color.Black );
                                break;
                            case GameMenuID.GMN_PLAYERSELECT:
                                m_btnIndicator.SetText("/A Select   /B Cancel   /Y Game Options", Fonts["PirateFont"], m_rectScreenDraw, Color.Black);
                                break;
                            case GameMenuID.GMN_HOWTOPLAY:
                                m_btnIndicator.SetText("/A Back To Menu   /B Controls", Fonts["PirateFont"], m_rectScreenDraw, Color.Black );
                                break;
                            default:
                                break;
                                
                        }
                        
                        //Reset the switch info
                        m_switchInfo.fSwitchTimer   = SwitchInfo.FADE_IN_TIME;
                        m_switchInfo.bFadeOut       = false;
                    }
                    else
                    {
                        //Fade sequence is complete
                        m_switchInfo = null;
                    }
                }
            }
            else
            {
                /////////////////////////////////////
                //Update the current screen and Handle any requested state change
                GameMenuID menuID = m_currentScreen.Update( fTotalTime, fElapsedTime, ParentApp.Inputs, ParentApp.SoundPlayer,  m_iWhoPaused.Integer);
                           
                switch( menuID )
                {
                    case GameMenuID.GMN_CONTROLS:
                        SwitchScreen( GameMenuID.GMN_CONTROLS );
                        break;
                    case GameMenuID.GMN_CREDITS:
                        //Switch the module
                        midNext = MODULE_IDENTIFIER.MID_CREDITS_MODULE;
                        break;
                    case GameMenuID.GMN_MAINMENU:
                        SwitchScreen( GameMenuID.GMN_MAINMENU );
                        break;
                    case GameMenuID.GMN_OPTIONS:
                        SwitchScreen( GameMenuID.GMN_OPTIONS );
                        break;
                    case GameMenuID.GMN_PLAYERSELECT:
                        SwitchScreen( GameMenuID.GMN_PLAYERSELECT );
                        break;
                    case GameMenuID.GMN_STARTGAME:
                        //Switch the module
                        midNext = MODULE_IDENTIFIER.MID_GAME_MODULE;
                        break;
                    case GameMenuID.GMN_GAMEOPTIONS:
                        SwitchScreen( GameMenuID.GMN_GAMEOPTIONS );
                        break;
                    case GameMenuID.GMN_HOWTOPLAY:
                        SwitchScreen(GameMenuID.GMN_HOWTOPLAY);
                        break;
                    default:
                        //GameMenuID.GMN_THIS
                        break;
                }
            }
            return midNext;   
        }

        /** @fn     void SwitchScreen( GameMenuID gmid )
         *  @brief  switch the active menu screen
            @param  gmid [in] the ID of the screen to switch to
         */
        private void SwitchScreen( GameMenuID gmid )
        {
            ParentApp.SoundPlayer.PlayCue("pageTurn");
            //ParentApp.SoundPlayer.PlayCue("Menu_Transition");
            //ParentApp.SoundPlayer.PlayCue( "wearepirates" );
            m_switchInfo = new SwitchInfo( gmid );
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void Draw(GraphicsDevice device, GameTime gameTime)
        {
            device.Clear( new Color( 50, 175, 175, 255 ) );//Color.SteelBlue ); //Kind of sky blue

            TestLocator.Draw(device, m_camera.ViewMatrix, m_camera.ProjectionMatrix, m_camera.Position);

            //m_objTerrain.Draw(device, m_camera.ViewMatrix, m_camera.ProjectionMatrix, m_camera.Position);
            m_MenuWater.Draw(device, m_camera.ViewMatrix, m_camera.ProjectionMatrix, m_camera.Position);
            //m_MenuWater.Shaders[0].CurrentTechnique = Shaders["BasicEffect"].Techniques["BasicTech"];
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public override void DrawNonEdgeDetectedFeatures(GraphicsDevice device, GameTime gameTime)
        {

            m_ptSmoke.Draw(m_camera.ViewMatrix, m_camera.ProjectionMatrix, gameTime);
            m_ptSmoke2.Draw(m_camera.ViewMatrix, m_camera.ProjectionMatrix, gameTime);

            m_sb.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            
            //Draw the menu background
            m_sb.Draw(m_texMenuBG, m_rectScreenDraw, Color.White);

            //Draw the button indicator
            m_btnIndicator.Draw(m_sb);

            //Draw the current menu
            m_currentScreen.Draw(m_sb);

            //Draw the transition effect if it's switching
            if( m_switchInfo != null )
            {
                float fPercentComplete = 1.0f - m_switchInfo.fSwitchTimer / SwitchInfo.FADE_OUT_TIME;

                if( m_switchInfo.bFadeOut == false )
                    fPercentComplete =  ( m_switchInfo.fSwitchTimer / SwitchInfo.FADE_IN_TIME );

                Color colFade   = Color.White;
                colFade.A       = (byte)( 255.0f * fPercentComplete );

                m_sb.Draw(m_texMenuBG, m_rectScreenDraw, colFade);
            }
            
            m_sb.End();
        }

        /** @fn     void ShutDown()
         *  @brief  clean up the modules resources
         */
        public override void ShutDown()
        {
            foreach( MainMenuScreen screen in m_dtMenuScreens.Values )
                screen.ShutDown();

            m_dtMenuScreens.Clear();

            base.ShutDown();
        }

        /** @class  SwitchInfo
         *  @brief  struct to contain information about an active screen switch
         */
        public class SwitchInfo
        {
            public const float FADE_OUT_TIME = 0.25f;
            public const float FADE_IN_TIME  = 0.25f;

            public bool bFadeOut;
            public float fSwitchTimer;
            public GameMenuID gmidNext;

            public SwitchInfo(GameMenuID gmid)
            {
                fSwitchTimer = FADE_OUT_TIME;
                gmidNext = gmid;
                bFadeOut = true;
            }
        }
    }
}
