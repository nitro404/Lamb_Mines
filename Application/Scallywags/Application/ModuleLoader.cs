using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace Scallywags
{
    /** @class  ModuleLoader
     *  @brief  a class to load module content (with a load screen)
     */
    class ModuleLoader
    {
        #region DATA_MEMBERS

        private const int NUM_TEXTURES = 5;

        private ContentManager  m_content;      ///< Content manager
        private XNAModule       m_nextModule;   ///< The ID of the next module
        private XNAModularApp   m_app;          ///< The app controlling the loader
        private string          m_strState;     ///< The current loading state
                                         
        //string                  m_strLoadImg;     ///< The image asset name
        //Texture2D               m_texLoadImg;     ///< The image to show while loading
        Texture2D[]             m_vLoadImages;      ///< the array of loading images      
        int                     m_nCurrentImage;
        Texture2D               m_texFade;          ///< The fade in/out texture
                                               
        private bool            m_bLoaded;      ///< Has the current module been fully loaded?
        private float           m_fFadeTime;    ///< Time remaining to fade complete
        private bool            m_bReady;       ///< Is the loader ready to be drawn?

        #endregion

        #region PROPERTIES

        /** @prop   NextModule
         *  @brief  the module being loaded
         */
        public XNAModule NextModule
        {
            get
            {
                return m_nextModule;
            }
        }

        /** @prop   Content
         *  @brief  the content loader
         */
        public ContentManager Content
        {
            get
            {
                return m_content;
            }
        }

        /** @prop   Ready
         *  @brief  is the loader ready to draw?
         */
        public bool Ready
        {
            get
            {
                return m_bReady;
            }
        }

        #endregion

        #region CONSTRUCTION

        /** @fn     ModuleLoader( GameServiceContainer services, XNAModularApp app )
         *  @brief  constructor
         *  @param  services [in] something the content manager needs... haven't looked into it
         *  @param  app [in] the application the loader belongs to
         */
        public ModuleLoader( GameServiceContainer services, XNAModularApp app )
        {
            m_content       = new ContentManager(services);
            m_nextModule    = null;
            
            m_strState      = "";

            m_vLoadImages   = null;

            m_app           = app;
            m_fFadeTime     = 0;
            m_texFade       = null;

            m_bReady        = false;
        }

        #endregion

        #region INTERFACE

        /** @fn     void Init()
         *  @brief  initialize the loader
         *  @param  device [in] a valid graphics device
         */
        public void Init( GraphicsDevice device )
        {
            m_nCurrentImage = 0;

            try
            {
                m_texFade = new Texture2D( device, 1, 1);

                Color[] pixels = new Color[1];
                pixels[0] = Color.Black;

                m_texFade.SetData<Color>(pixels);

                m_vLoadImages = new Texture2D[ NUM_TEXTURES ];
            }
            catch (Exception ex)
            {
                Error.Trace("Error in ModuleLoader constructor: " + ex.Message);
            }
        }
        
        /** @fn     void LoadModule( XNAModule moduleToLoad )
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public void LoadModule(XNAModule moduleToLoad)
        {
            m_bLoaded = false;

            if( m_app.CurrentModule != null )
            {
                m_app.CurrentModule.ShutDown();
                m_app.CurrentModule = null;
            }

            m_nCurrentImage = 0;

            //Unload any existing content
            m_content.Unload();
            m_nextModule = moduleToLoad;

            if( m_nextModule.ShowLoadScreen )
            {
                Error.Trace( "Spawning load thread" );

                try
                {
                    //Load the module resources with the loading screen
                    m_vLoadImages[ 0 ] = m_content.Load< Texture2D >( "Content/Textures/Load0" );
                    m_vLoadImages[ 1 ] = m_content.Load< Texture2D >( "Content/Textures/Load1" );
                    m_vLoadImages[ 2 ] = m_content.Load< Texture2D >( "Content/Textures/Load2" );
                    m_vLoadImages[ 3 ] = m_content.Load< Texture2D >( "Content/Textures/Load3" );
                    m_vLoadImages[ 4 ] = m_content.Load< Texture2D >( "Content/Textures/Load4" );
                }
                catch( Exception ex )
                {
                    Error.Trace( "Error loading module loader textures: " + ex.Message );
                    m_app.Exit();
                    return;
                }

                m_bReady = true;

                //Spawn loading thread
                Thread thLoad = new Thread( LoadResources );
                thLoad.Start();
            }
            else
            {
                Error.Trace( "Loading Module without load screen" );
                //Load the resources, don't show a splash screen
                LoadResources();
            }
        }

        /** @fn     void Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @param  gameTime [in] information about the time between frames
         */
        public void Update(GameTime gameTime, float fElapsedTime)
        {
            try
            {
                //When loading is complete, initialize the module.
                if( m_bLoaded == true )
                {
                    m_fFadeTime -= fElapsedTime;
                    SetFadeAmount( 1.0f - ( m_fFadeTime / Settings.LOADER_FADE_OUT_TIME ) );

                    if( m_fFadeTime <= 0 )
                    {
                        //Tell the app to start running the new module
                        m_bReady = false;
                        m_app.CurrentModule = m_nextModule;
                    }
                } 
            }
            catch( Exception ex )
            {
                Error.Trace("Loader Draw Exception: " + ex.Message);
                Error.Trace(" \n\nStack trace\n\n " + ex.StackTrace);
            }
        }

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public void Draw(GraphicsDevice device, GameTime gameTime)
        {
            try
            {
                device.Clear(Color.Black);

                //Cases where we don't want this to draw.
                if( ( m_bLoaded == true && m_fFadeTime <= 0 ) || m_bReady == false )
                    return;

                SpriteBatch sb = new SpriteBatch( device );

                sb.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState );

                float fY = ( device.Viewport.Height - m_vLoadImages[ m_nCurrentImage ].Height ) / 2.0f;
                float fX = ( device.Viewport.Width - m_vLoadImages[ m_nCurrentImage ].Width ) / 2.0f;

                sb.Draw( m_vLoadImages[m_nCurrentImage], new Vector2( fX, fY ), Color.White );

                sb.End();

                m_app.DebugFont.DrawFont(sb, m_strState, 0, 0, Color.White);

                sb.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

                    if (m_fFadeTime > 0)
                    {
                        //Draw the fade
                        sb.Draw(m_texFade, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.Black);

                    }
                sb.End();           
            }
            catch( Exception ex )
            {
                Error.Trace( "Loader Draw Exception: " + ex.Message );
                Error.Trace( " \n\nStack trace\n\n " + ex.StackTrace );
            }
        }

        #endregion

        #region INTERNAL

        /** @fn     void LoadResources()
         *  @brief  load the module resources
         */
        private void LoadResources()
        {
            /////////////////////////////////
            //Check for any exceptions loading resources (most likely, resource not found)
            try
            {
                //Load the models
                m_strState      = "Loading Models";
                m_nCurrentImage = 0;
                Thread.Sleep(100);

                foreach (string str in m_nextModule.ModelResources)
                {
                    Model mdl = m_content.Load<Model>(str);

                    string strName = ExtractResourceName( str );

                    m_nextModule.AddModel( strName, mdl );
                }
               
                //Load the textures
                m_strState = "Loading Textures";
                m_nCurrentImage = 1;
                Thread.Sleep(100);

                foreach ( string str in m_nextModule.TextureResources )
                {
                    Texture2D tex = m_content.Load< Texture2D >( str );
                    string strName = ExtractResourceName(str);
                    m_nextModule.AddTexture(strName, tex);
                }

                //Load the shaders
                m_strState = "Loading Shaders";
                m_nCurrentImage = 2;
                Thread.Sleep(100);

                foreach ( string str in m_nextModule.ShaderResources )
                {
                    Effect eff = m_content.Load< Effect >( str );

                    string strName = ExtractResourceName( str );
                    m_nextModule.AddEffect( strName, eff );
                }


                //Load the fonts
                m_strState = "Loading Fonts";
                m_nCurrentImage = 3;
                Thread.Sleep(100);
                foreach ( string str in m_nextModule.FontResources )
                {
                    SpriteFont font = m_content.Load< SpriteFont >( str );

                    string strName = ExtractResourceName( str );
                    m_nextModule.AddFont( strName, font );
                }

                //Signal that the thread is complete
                m_strState = "Loading Complete";
                m_nCurrentImage = 4;
                Thread.Sleep(100);
            }
            catch( Exception ex )
            {
                Error.Trace( "*****  ERROR  ********\nLoading resources: " + ex.Message );
                Error.Trace( "***** EXITING ********" );
                m_app.Exit();

                return;
            }

            /////////////////////////////////////
            //Check for any errors in module Init
            try
            {
                //Initialize the new module
                m_strState = "Initializing Module";
                m_nextModule.Initialize();                
            }
            catch( Exception ex )
            {
                Error.Trace("*****  ERROR  ********\nInitializing Module: " + ex.Message 
                    + "\n\n\t*STACK TRACE*\n" + ex.StackTrace + "\n\n" );
                Error.Trace("***** EXITING ********");
                m_app.Exit();

                return;
            }

            m_bLoaded   = true;
            m_fFadeTime = Settings.LOADER_FADE_OUT_TIME;

            return;
        }

        /** @fn     void SetFadeAmount( float fAmt )
         *  @brief  set the amount of fade to apply to the background image
         */
        private void SetFadeAmount( float fAmt )
        {
            Color[] nPixels = new Color[1];
            nPixels[0] = new Color( 0.0f, 0, 0, fAmt );
            m_texFade.SetData<Color>(nPixels);
        }

        /** @fn     string ExtractResourceName( string strFullResourcePath )
         *  @brief  get the name of the resource without the leading path
         *  @return the asset name only
         *  @param  strFullResourcePath [in] the full path of the resource in the content directory
         */
        private string ExtractResourceName( string strFullResourcePath )
        {
            //count back from the end to the first /
            int nSlashIndex = strFullResourcePath.LastIndexOf( '/' );

            //No forward slash found
            if( nSlashIndex == -1 )
            {
                //Check for a backslash
                nSlashIndex = strFullResourcePath.LastIndexOf( '\\' );

                if( nSlashIndex == -1 )
                    return strFullResourcePath;
            }

            string strName = strFullResourcePath.Substring( nSlashIndex + 1 );

            return strName;
        }

        #endregion
    }
}