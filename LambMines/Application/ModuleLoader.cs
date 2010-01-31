using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace LambMines
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
                                               
        private bool            m_bLoaded;      ///< Has the current module been fully loaded?


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
            
            m_app           = app;
        }

        #endregion

        #region INTERFACE

        /** @fn     void Init()
         *  @brief  initialize the loader
         *  @param  device [in] a valid graphics device
         */
        public void Init( GraphicsDevice device )
        {
          
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

            //Unload any existing content
            m_content.Unload();
            m_nextModule = moduleToLoad;

           
            Error.Trace( "Loading Module without load screen" );
            //Load the resources, don't show a splash screen
            LoadResources();
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
                    //Tell the app to start running the new module
                    m_app.CurrentModule = m_nextModule;

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
            device.Clear(Color.Black);    
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
                Thread.Sleep(100);

                foreach (string str in m_nextModule.ModelResources)
                {
                    Model mdl = m_content.Load<Model>(str);

                    string strName = ExtractResourceName( str );

                    m_nextModule.AddModel( strName, mdl );
                }
               
                Thread.Sleep(100);

                foreach ( string str in m_nextModule.TextureResources )
                {
                    Texture2D tex = m_content.Load< Texture2D >( str );
                    string strName = ExtractResourceName(str);
                    m_nextModule.AddTexture(strName, tex);
                }
                Thread.Sleep(100);

                foreach ( string str in m_nextModule.ShaderResources )
                {
                    Effect eff = m_content.Load< Effect >( str );

                    string strName = ExtractResourceName( str );
                    m_nextModule.AddEffect( strName, eff );
                }

                Thread.Sleep(100);
                foreach ( string str in m_nextModule.FontResources )
                {
                    SpriteFont font = m_content.Load< SpriteFont >( str );

                    string strName = ExtractResourceName( str );
                    m_nextModule.AddFont( strName, font );
                }

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

            return;
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