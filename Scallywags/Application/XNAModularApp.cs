using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace LambMines
{
    public class XNAModularApp : Microsoft.Xna.Framework.Game
    {
        #region DATA_MEMBERS

        //Handling modules
        private List<XNAModule> m_vModules;         ///< Application module array
        private XNAModule       m_currentModule;    ///< Reference to currently executing module
        private XNAModule       m_startModule;      ///< The module to start running with
        private ModuleLoader    m_loader;           ///< Module content loader
        private float           m_fElapsedDelay;    ///< Time elapsed since the last module switch, up to SWITCH_INPUT_DELAY
        private double          m_fTotalTime;       ///< Total elapsed time                                            
     
        //XNA objects
        private GraphicsDeviceManager m_graphics;   ///< Graphic device manager

        //Frame per second counting                     
        private int             m_nFPS;             ///< Frames per seconds - updated every second
        private double          m_fSecondTime;      ///< Time elapsed this second, in seconds
        private int             m_nFrameCount;      ///< Frame count this second

        //Our Utilities
        private FontUtil        m_font;             ///< A vector font for debugging
        private InputManager    m_Inputs;
        //private SoundManager    m_soundManager;     ///< The sound manager
                                                    ///
        SpriteBatch spriteBatch;
                                                          
        #endregion

        #region PROPERTIES

        /** @prop   CurrentModule
         *  @brief  the currently executing module
         **/
        public XNAModule CurrentModule
        {
            get
            {
                return m_currentModule;
            }
            set
            {
                m_currentModule = value;
            }
        }

        /** @prop   Device
         *  @brief  the graphic device
         */
        public GraphicsDevice Device
        {
            get
            {
                return m_graphics.GraphicsDevice;
            }
        }

        /** @prop   SoundPlayer
         *  @brief  the tool to play sounds and music with
         */
        //public SoundManager SoundPlayer
        //{
        //    get
        //    {
                //return m_soundManager;
        //    }
        //}

        /** @prop   Graphics
         *  @brief  the device manager
         */
        public GraphicsDeviceManager Graphics
        {
            get
            {
                return m_graphics;
            }
        }

        /** @prop   FPS
         *  @brief  the frames per second
         */
        public int FPS
        {
            get
            {
                return m_nFPS;
            }
        }

        /** @prop   KeyState
         *  @brief  get the state of the keyboard
         */
        public InputManager Inputs
        {
            get
            {
                return m_Inputs;
            }
        }

        /** @prop  DebugFont
         *  @brief Allows access to the FontUtil class
         */
        public FontUtil DebugFont
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }

        /** @prop   TotalTime
         *  @brief  get the total elapsed time since the program started
         */
        public double TotalTime
        {
            get
            {
                return m_fTotalTime;
            }
        }

        #endregion

        #region CONSTRUCTION

        /** @fn     
         *  @brief  Constructor
         *  @param  startModule [in] reference to the start module
         */
        public XNAModularApp(XNAModule startModule)
            : base()
        {
            m_vModules = new List<XNAModule>();

            AddModule(startModule);
            m_startModule   = startModule;
            m_currentModule = null;
            m_fElapsedDelay = 0;

            m_graphics = new GraphicsDeviceManager(this);

            //Anti aliasing doesn't seem to have an effect with edge enhancement on.
            m_graphics.PreferMultiSampling = true;

            if( Settings.START_FULL_SCREEN )
            {
#if XBOX
                m_graphics.PreferredBackBufferHeight = Settings.PREFFERED_WINDOW_HEIGHT;
                m_graphics.PreferredBackBufferWidth = Settings.PREFFERED_WINDOW_WIDTH;
#else
                m_graphics.PreferredBackBufferWidth     = 1440;//m_graphics.GraphicsDevice.Viewport.Width;
                m_graphics.PreferredBackBufferHeight    = 900;//m_graphics.GraphicsDevice.Viewport.Height;
#endif
                m_graphics.IsFullScreen = true;
            }
#if WINDOWS
            else
            {
                //Only request the back buffer width/height when not running full screen (effectively becomes the window dimensions)
                m_graphics.PreferredBackBufferHeight    = Settings.PREFFERED_WINDOW_HEIGHT;
                m_graphics.PreferredBackBufferWidth     = Settings.PREFFERED_WINDOW_WIDTH;
            }
#endif
            //Prepare device callback to set graphic settings.
            m_graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>( PrepareDevice );

            m_loader = new ModuleLoader(Services, this );

            m_nFPS = 0;
            m_fSecondTime = 0;
            m_nFrameCount = 0;
            m_fTotalTime = 0;

            m_font = new FontUtil();

            m_Inputs = new InputManager();
            //m_soundManager = new SoundManager();
        }

        #endregion

        #region INTERFACE

        /** @fn     AddModule( XNAModule newModule )
         *  @brief  add a module to the application
         *  @param  newModule [in] reference to the module to add
         */
        public void AddModule(XNAModule newModule)
        {
            if (null != newModule)
            {
                //Set the parent application and add the module to the stack
                newModule.ParentApp = this;
                m_vModules.Add(newModule);
            }
        }

        #endregion

        #region FRAMEWORK


         void PrepareDevice(object sender, PreparingDeviceSettingsEventArgs eventargs)
        {
            return;               
        }

        /** @fn     void Initialize()
         *  @brief  set up running module properties.
         */
        protected override void Initialize()
        {
            //m_soundManager.Init( Content );
            m_loader.Init( GraphicsDevice );

            //Allow key presses right as the app starts (so you can skip the splash module)
            m_fElapsedDelay = Settings.SWITCH_INPUT_DELAY;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load the start module 
            SwitchModule( m_startModule.ID );            
            base.Initialize();
        }

        /** @fn     void Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @param  gameTime [in] information about the time between frames
         */
        protected override void Update(GameTime gameTime)
        {
            //m_soundManager.Update();

            //Calculate the elapsed time in seconds
            float fElapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            m_fTotalTime += fElapsedTime;
                        
            if( m_currentModule == null )
            {
                //If current module is null, then one should be loading.. update the loader
                m_loader.Update(gameTime, fElapsedTime);
            }
            else
            {
                UpdateFPS(fElapsedTime);
                UpdateInput(fElapsedTime);
                
                #if WINDOWS
                    ///////////////////////////
                    //Handle escape keyboard press on windows builds
                    if( m_Inputs.IsKeyDown( Keys.Escape ) )
                        Exit();

                #endif


                //Update the module
                MODULE_IDENTIFIER moduleID = m_currentModule.Update(m_fTotalTime, fElapsedTime);
                base.Update(gameTime);

                if (moduleID != MODULE_IDENTIFIER.MID_GAME_MODULE && moduleID != MODULE_IDENTIFIER.MID_THIS )
                {
                    for (int i = 0; i < 4; i++)
                        GamePad.SetVibration((PlayerIndex)i, 0.0f, 0.0f);
                }

                //Check if the module requested a change, if so, change it.
                if (moduleID != MODULE_IDENTIFIER.MID_THIS )
                {
                    SwitchModule(moduleID);
                }
            }
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  gameTime [in] information about the time between frames
         */
        protected override void Draw(GameTime gameTime)
        {

            SpriteBatch sb = new SpriteBatch( Graphics.GraphicsDevice );
           //Draw the current module, or the loader if no module is loaded
            if (m_currentModule == null)
            {
                m_loader.Draw(m_graphics.GraphicsDevice, gameTime);
            }
            else
            {
                if (Settings.FAST_MODE == false)
                {

                    m_currentModule.Draw(m_graphics.GraphicsDevice, gameTime);

                    if (Settings.SHOW_FPS)
                    {
                        string strFPS = "FPS: " + m_nFPS;
                        m_font.DrawFont(sb, strFPS, 0, 0, Color.White);
                    }
                }

            }

            base.Draw(gameTime);
        }

     

        #endregion

        #region INTERNAL

        /** @fn     void SwitchModule( MODULE_IDENTIFIER nID )
        *   @brief  change the current module
        */
        private void SwitchModule(MODULE_IDENTIFIER nID)
        {
            //Find the module with the matching ID
            foreach (XNAModule theModule in m_vModules)
            {
                if (nID == theModule.ID)
                {
                    m_Inputs.ResetInput();// ClearInput();
                    
                    //m_soundManager.StopAll();

                    m_fElapsedDelay = 0;

                    //Launch the loader
                    m_loader.LoadModule( theModule );

                    /////////////////////////
                    //Warning, potential thread synch issue here.
                    m_font.Init(Graphics.GraphicsDevice.Viewport.Width,
                    Graphics.GraphicsDevice.Viewport.Height,
                    m_loader.Content.Load<SpriteFont>(@"Content\Font\DebugFont"));

                    break;
                }
            }
        }

        /** @fn     void UpdateFPS( float fElapsedTime )
         *  @brief  update the frame count
         *  @param  fElapsedTime [in] the time elapsed since the previous frame
           */
        private void UpdateFPS( float fElapsedTime )
        {
            //Update frames per second, every second
            m_fSecondTime += fElapsedTime;
            m_nFrameCount += 1;

            if (m_fSecondTime > 1.0)
            {
                //Error.Trace( "Frames This Second: " + m_nFrameCount );
                m_nFPS          = m_nFrameCount;

                m_nFrameCount   = 0;
                m_fSecondTime   -= 1.0;
            }
        }

        /** @fn     void UpdateInput( float fElapsedTime )
         *  @brief  check if the user is attempting to maximize or window the app
         *  @param  fElapsedTime [in] the time elapsed since last frame
         */
        private void UpdateInput( float fElapsedTime )
        {
            //Update input as soon as SWITCH_INPUT_DELAY seconds have passed.  (Prevents module actions multiple times on one hit while switching)
            if (m_fElapsedDelay > Settings.SWITCH_INPUT_DELAY)
            {
                m_Inputs.Update(fElapsedTime);
            }
            else
                m_fElapsedDelay += fElapsedTime;
        }

        #endregion

    }//end class
}