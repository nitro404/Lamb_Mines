using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Scallywags
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
        private SoundManager    m_soundManager;     ///< The sound manager
                                                    ///

        // Effect used to apply the edge detection and pencil sketch postprocessing.
        Effect postprocessEffect;

        int settingsIndex = 0;


        // Custom rendertargets.
        RenderTarget2D sceneRenderTarget;
        RenderTarget2D normalDepthRenderTarget;

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

        public int PostProcessEffect
        {
            set
            {
                settingsIndex = value;
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
        public SoundManager SoundPlayer
        {
            get
            {
                return m_soundManager;
            }
        }

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

        // Choose what display settings to use.
        NonPhotoRealisticSettings PPSettings
        {
            get { return NonPhotoRealisticSettings.PresetSettings[settingsIndex]; }
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

            Components.Add(new GamerServicesComponent(this));

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
            m_soundManager = new SoundManager();
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

#if XBOX
            eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = 0;
            eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = MultiSampleType.FourSamples;
            return;  
#else
            //For the purple problem
            eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            
             //Set up antialiasing
             int quality = 0;
            GraphicsAdapter adapter = eventargs.GraphicsDeviceInformation.Adapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            // Check for 4xAA
            if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, format,
                false, MultiSampleType.FourSamples, out quality))
            {
                // even if a greater quality is returned, we only want quality 0
                eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = 0;
                eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleType =
                    MultiSampleType.FourSamples;
            }
            // Check for 2xAA
            else if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, format,
                false, MultiSampleType.TwoSamples, out quality))
            {
                // even if a greater quality is returned, we only want quality 0
                eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = 0;
                eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleType =
                    MultiSampleType.TwoSamples;
            }

            return;               
#endif
         }

        /** @fn     void Initialize()
         *  @brief  set up running module properties.
         */
        protected override void Initialize()
        {
            m_soundManager.Init( Content );
            m_loader.Init( GraphicsDevice );

            //Allow key presses right as the app starts (so you can skip the splash module)
            m_fElapsedDelay = Settings.SWITCH_INPUT_DELAY;

            postprocessEffect = Content.Load<Effect>("Content/Shaders/PostprocessEffect");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create two custom rendertargets.
            PresentationParameters pp =GraphicsDevice.PresentationParameters;

#if XBOX
           sceneRenderTarget = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality, RenderTargetUsage.DiscardContents );

            normalDepthRenderTarget = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality, RenderTargetUsage.DiscardContents ); 
#else
            sceneRenderTarget = new RenderTarget2D( GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality, RenderTargetUsage.PreserveContents );

            normalDepthRenderTarget = new RenderTarget2D( GraphicsDevice,
                pp.BackBufferWidth, pp.BackBufferHeight, 1,
                pp.BackBufferFormat, pp.MultiSampleType, pp.MultiSampleQuality, RenderTargetUsage.PreserveContents );

#endif
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
            m_soundManager.Update();

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

                    if (m_Inputs.IsKeyPressed(Keys.Tab))
                    {
                        if (settingsIndex != NonPhotoRealisticSettings.PresetSettings.Length - 1)
                            settingsIndex++;
                        else
                            settingsIndex = 0;
                    }

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
            Device.RenderState.DepthBufferEnable = true;

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
                    // If we are doing edge detection, first off we need to render the
                    // normals and depth of our model into a special rendertarget.
                    if (PPSettings.EnableEdgeDetect)
                    {
                        GraphicsDevice.SetRenderTarget(0, normalDepthRenderTarget);

                        //GraphicsDevice.Clear(Color.Black);

                        Settings.DETECT_EDGES = true;
                        m_currentModule.Draw(m_graphics.GraphicsDevice, gameTime);
                        Settings.DETECT_EDGES = false;
                    }
                    // If we are doing edge detection and/or pencil sketch processing, we
                    // need to draw the model into a special rendertarget which can then be
                    // fed into the postprocessing shader. Otherwise can just draw it
                    // directly onto the backbuffer.
                    if (PPSettings.EnableEdgeDetect || PPSettings.EnableSketch)
                        GraphicsDevice.SetRenderTarget(0, sceneRenderTarget);
                    else
                        GraphicsDevice.SetRenderTarget(0, null);


                    m_currentModule.Draw(m_graphics.GraphicsDevice, gameTime);

                    if (Settings.SHOW_FPS)
                    {
                        string strFPS = "FPS: " + m_nFPS;
                        m_font.DrawFont(sb, strFPS, 0, 0, Color.White);
                    }

                    // Run the postprocessing filter over the scene that we just rendered.
                    if (PPSettings.EnableEdgeDetect || PPSettings.EnableSketch)
                    {
                        GraphicsDevice.SetRenderTarget(0, null);

                        ApplyPostprocess();
                    }
                    m_currentModule.DrawNonEdgeDetectedFeatures(m_graphics.GraphicsDevice, gameTime);
                }
                else
                {
                    m_currentModule.Draw(m_graphics.GraphicsDevice, gameTime);
                    m_currentModule.DrawNonEdgeDetectedFeatures(m_graphics.GraphicsDevice, gameTime);
                }
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper applies the edge detection and pencil sketch postprocess effect.
        /// </summary>
        void ApplyPostprocess()
        {
            EffectParameterCollection parameters = postprocessEffect.Parameters;
            string effectTechniqueName;

            // Set effect parameters controlling the pencil sketch effect.
            if (PPSettings.EnableSketch)
            {
                parameters["SketchThreshold"].SetValue(PPSettings.SketchThreshold);
                parameters["SketchBrightness"].SetValue(PPSettings.SketchBrightness);
            }

            // Set effect parameters controlling the edge detection effect.
            if (PPSettings.EnableEdgeDetect)
            {
                Vector2 resolution = new Vector2(sceneRenderTarget.Width,
                                                 sceneRenderTarget.Height);

                Texture2D normalDepthTexture = normalDepthRenderTarget.GetTexture();

                parameters["EdgeWidth"].SetValue(PPSettings.EdgeWidth);
                parameters["EdgeIntensity"].SetValue(PPSettings.EdgeIntensity);
                parameters["ScreenResolution"].SetValue(resolution);
                parameters["NormalDepthTexture"].SetValue(normalDepthTexture);

                // Choose which effect technique to use.
                if (PPSettings.EnableSketch)
                {
                    if (PPSettings.SketchInColor)
                        effectTechniqueName = "EdgeDetectColorSketch";
                    else
                        effectTechniqueName = "EdgeDetectMonoSketch";
                }
                else
                    effectTechniqueName = "EdgeDetect";
            }
            else
            {
                // If edge detection is off, just pick one of the sketch techniques.
                if (PPSettings.SketchInColor)
                    effectTechniqueName = "ColorSketch";
                else
                    effectTechniqueName = "MonoSketch";
            }

            // Activate the appropriate effect technique.
            postprocessEffect.CurrentTechnique =
                                    postprocessEffect.Techniques[effectTechniqueName];

            // Draw a fullscreen sprite to apply the postprocessing effect.
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.SaveState);

            postprocessEffect.Begin();
            postprocessEffect.CurrentTechnique.Passes[0].Begin();

            spriteBatch.Draw(sceneRenderTarget.GetTexture(), Vector2.Zero, Color.White);

            spriteBatch.End();

            postprocessEffect.CurrentTechnique.Passes[0].End();
            postprocessEffect.End();
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
                    
                    m_soundManager.StopAll();

                    m_fElapsedDelay = 0;

                    //Launch the loader
                    m_loader.LoadModule( theModule );

                    /////////////////////////
                    //Warning, potential thread synch issue here.
                    m_font.Init(Graphics.GraphicsDevice.Viewport.Width,
                    Graphics.GraphicsDevice.Viewport.Height,
                    m_loader.Content.Load<SpriteFont>(@"Content\Font\DebugFont"));
                    PostProcessEffect = 0;

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