#region USING

using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

#endregion

namespace Scallywags
{
    /** @enum   ResourceType
     *  @brief  defines the possible types of resources
     */
    public enum ResourceType
    {
        RT_UNKNOWN  = -1,
        RT_FONT     = 1,
        RT_TEXTURE  = 2,
        RT_MODEL    = 3,
        RT_EFFECT   = 4
    }

    /** @class  CXNAModule
     *  @brief  class to contain a section of program execution
     */
    public abstract class XNAModule
    {
        #region DATA_MEMBERS

        private MODULE_IDENTIFIER   m_nModuleID;            ///< The ID of the module
        private XNAModularApp       m_parentApp;            ///< The parent module this instance belongs to, if any

        //Registered asset names, for the loader           
        private List< string >      m_lstModelResources;    ///< The list of models the module requires
        private List< string >      m_lstTextureResources;  ///< The list of textures the module requires
        private List< string >      m_lstShaderResources;   ///< The list of shaders the module requires
        private List< string >      m_lstFontResources;     ///< The list of fonts the module requires
           
        /////////////////////////////
        //Hashtables indexed by asset name                 
        private Dictionary< string, Texture2D >     m_dtTextures;   ///< The loaded textures
        private Dictionary< string, Model >         m_dtModels;     ///< The loaded list of models
        private Dictionary< string, Effect >        m_dtShaders;    ///< The shaders loaded for the module                                         
        private Dictionary< string, SpriteFont >    m_dtFonts;      ///< The fonts the module has loaded                                            

        private bool                m_bUseLoadScreen;       ///< Should the app show a load screen when loading this module?

        #endregion

        #region PROPERTIES

        /** @prop   ShowLoadScreen
         *  @brief  should the app show a load screen when this module is loading?
         */
        public bool ShowLoadScreen
        {
            get
            {
                return m_bUseLoadScreen;
            }
            set
            {
                m_bUseLoadScreen = value;
            }
        }

        /** @prop   ModelResources
         *  @brief  the list of models the module requires
         *          Read-Only
         */
        public List< string > ModelResources
        {
            get
            {
                return m_lstModelResources;
            }
        }

        /** @prop   TextureResources
         *  @brief  the list of textures the module requires
         *          Read-Only
         */
        public List< string > TextureResources
        {
            get
            {
                return m_lstTextureResources;
            }
        }

        /** @prop   ShaderResources
         *  @brief  the list of shaders the module requires
         *          ReadOnly
         */
        public List<string> ShaderResources
        {
            get
            {
                return m_lstShaderResources;
            }
        }

        /** @prop   FontResources
         *  @brief  the list of fonts resources
         *          Read Only
         */
        public List< string > FontResources
        {
            get
            {
                return m_lstFontResources;
            }
        }
        
        /** @prop   Textures
         *  @brief  get the list of textures
         *          Read Only
         */
        public Dictionary<string, Texture2D> Textures
        {
            get
            {
                return m_dtTextures;
            }
        }

        /** @prop   Models
         *  @brief  get the list of models
         *          Read Only
         */
        public Dictionary<string, Model> Models
        {
            get
            {
                return m_dtModels;
            }
        }

        /** @prop   Shaders
         *  @brief  the loaded shaders
         *          read only
         */
        public Dictionary<string, Effect> Shaders
        {
            get
            {
                return m_dtShaders;
            }
        }

        /** @prop   Fonts
         *  @brief  the loaded fonts
         *          read only
         */
        public Dictionary< string, SpriteFont > Fonts
        {
            get
            {
                return m_dtFonts;
            }
        }

        /** @prop   ParentApp
         *  @brief  The parent application.
         */
        public XNAModularApp ParentApp
        {
            set
            {
                if (null == m_parentApp)
                {
                    m_parentApp = value;
                }
            }
            get
            {
                return m_parentApp;
            }
        }

        /** @prop   SoundPlayer
         *  @brief  shorthand to the app's sound player
         */
        //public SoundManager SoundPlayer
        //{
        //    get
        //    {
        //        return ParentApp.SoundPlayer;
        //    }
        //}

        /** @prop   ID
         *  @brief  the id of the module, should be unique
         *          Read Only
         */
        public MODULE_IDENTIFIER ID
        {
            get
            {
                return m_nModuleID;
            }
        }
        #endregion

        #region CONSTRUCTION

        /** @fn     CXNAModule( MODULE_IDENTIFIER nID )
         *  @brief  Constructor
         *  @param  nID [in] the ID of the module
         */
        public XNAModule(MODULE_IDENTIFIER nID)
        {
            m_nModuleID     = nID;
            m_parentApp     = null;

            m_lstTextureResources   = new List< string >();
            m_lstModelResources     = new List< string >();
            m_lstShaderResources    = new List< string >();
            m_lstFontResources      = new List<string>();

            m_dtTextures    = new Dictionary<string, Texture2D>();
            m_dtModels      = new Dictionary<string, Model>();
            m_dtShaders     = new Dictionary<string, Effect>();
            m_dtFonts       = new Dictionary<string,SpriteFont>();

            m_bUseLoadScreen = true;
        }

        #endregion

        #region INTERFACE

        /** @fn     void AddModel( Model mdl )
         *  @brief  add a model to the module resource list
         *  @param  mdl [in] the model to add
         */
        public void AddModel( string strName, Model mdl )
        {
            if( m_dtModels.ContainsKey( strName ) == false )
            {
                m_dtModels.Add( strName, mdl );
            }
        }

        /** @fn     void AddTexture( Texture2D tex )
         *  @brief  add a texture to the module resource list
         *  @param  tex [in] the texture to add
         */
        public void AddTexture( string strName, Texture2D tex )
        {
            if (m_dtTextures.ContainsKey(strName) == false)
            {
                m_dtTextures.Add(strName, tex);
            }
        }

        /** @fn     void AddEffect( Effect eff )
         *  @brief  add an effect to the module resouce list
         *  @param  eff [in] the effect to add
         */
        public void AddEffect( string strName, Effect eff )
        {
            if( m_dtShaders.ContainsKey( strName ) == false )
            {
                m_dtShaders.Add( strName, eff );
            }
        }

        /** @fn     void AddFont( string strName, SpriteFont font )
         *  @brief  add a font to the module resource list
         *  @param  strName [in] the name of the font resource
         *  @param  font [in] the font instance
         */
        public void AddFont( string strName, SpriteFont font )
        {
            if( m_dtFonts.ContainsKey( strName ) == false )
            {
                m_dtFonts.Add( strName, font );
            }
        }

        /** @fn     void RegisterResource( ResourceType type, string assetOrFileName )
         *  @brief  add a resource to the module's resource list
         *  @param  type [in] the type of resource ( sound, texture, model...)
         *  @param  strName [in] the name of the asset or file that will contain the resource
         */
        protected void RegisterResource( ResourceType type, string strName )
        {
            switch( type )
            {
                case ResourceType.RT_MODEL:
                {
                    //Check that the model hasn't already been added
                    for (int i = 0; i < m_lstModelResources.Count; ++i)
                    {
                        if (m_lstModelResources[i].Equals(strName))
                        {
                            Error.Trace("Error, model already registered with module. - " + strName);
                            return;
                        }
                    }

                    //Add the model
                    m_lstModelResources.Add( strName );

                    break;
                }
                case ResourceType.RT_TEXTURE:
                {
                    //Check that the texture hasn't already been added
                    for (int i = 0; i < m_lstTextureResources.Count; ++i)
                    {
                        if (m_lstTextureResources[i].Equals(strName))
                        {
                            Error.Trace("Error, texture already registered with module. - " + strName);
                            return;
                        }
                    }

                    //Add the texture
                    m_lstTextureResources.Add(strName);

                    break;
                }
                case ResourceType.RT_EFFECT:
                {
                    //Check that the shader hasn't already been added
                    for (int i = 0; i < m_lstShaderResources.Count; ++i)
                    {
                        if (m_lstShaderResources[i].Equals(strName))
                        {
                            Error.Trace("Error, shader already registered with module. - " + strName);
                            return;
                        }
                    }

                    //Add the texture
                    m_lstShaderResources.Add(strName);

                    break;
                }
                case ResourceType.RT_FONT:
                {
                    for( int i = 0; i < m_lstFontResources.Count; ++i )
                    {
                        if( m_lstFontResources[i].Equals( strName ) )
                        {
                            Error.Trace( "Error, font already registered with the module. - " + strName );
                            return;
                        }
                    }

                    //Add the font
                    m_lstFontResources.Add( strName );

                    break;
                }
                default:
                {
                    Error.Trace( "Unknown resource type for asset " + strName );
                    break;
                }
            }
        }

        /** @fn     void UnregisterResource( string strName )
         *  @brief  remove a resource from a module
         *  @param  strName [in] the name of the resource to unregister
         */
        protected void UnregisterResource( ResourceType type, string strName)
        {
            switch (type)
            {
                case ResourceType.RT_MODEL:
                {
                    //Check that the model hasn't already been added
                    for (int i = 0; i < m_lstModelResources.Count; ++i)
                    {
                        if (m_lstModelResources[i].Equals(strName))
                        {
                            m_lstModelResources.RemoveAt( i );
                            return;
                        }
                    }

                    Error.Trace( "Model asset not registered: " + strName );
                    break;
                }
                case ResourceType.RT_TEXTURE:
                {
                    //Check that the texture hasn't already been added
                    for (int i = 0; i < m_lstTextureResources.Count; ++i)
                    {
                        if (m_lstTextureResources[i].Equals(strName))
                        {
                            m_lstTextureResources.RemoveAt( i );
                            return;
                        }
                    }

                    Error.Trace("Texture asset not registered: " + strName);
                    break;
                }
                default:
                {
                    Error.Trace("Unknown resource type for asset " + strName);
                    break;
                }
            }
        }

        #endregion

        #region MUST_INHERIT_VIRTUAL

        /** @fn     void Initialize()
         *  @brief  set up module properties.  This is the first call
         *          after the constructor.
         */
        public abstract void Initialize();

        /** @fn     MODULE_IDENTIFIER Update( GameTime gameTime )
         *  @brief  Update the module scene
         *  @return the ID of the module to switch to, or ( MID_THIS {0} ) if this module is to continue executing
         *  @param  fTotalTime [in] the total elapsed time since the app began running, in seconds
         *  @param  fElapsedTime [in] the time elapsed since the last frame
         */
        public abstract MODULE_IDENTIFIER Update(double fTotalTime, float fElapsedTime);

        /** @fn     void Draw( GameTime gameTime )
         *  @brief  Draw the module's scene
         *  @param  device [in] the active graphics device
         *  @param  gameTime [in] information about the time between frames
         */
        public abstract void Draw(GraphicsDevice device, GameTime gameTime);

        #endregion

        #region CAN_INHERIT_VIRTUAL

        /** @fn     void ShutDown()
         *  @brief  clean up the modules resources
         */
        public virtual void ShutDown()
        {
            //SoundPlayer.CleanUp();
            m_dtTextures.Clear();
            m_dtModels.Clear();
            m_dtShaders.Clear();
            m_dtFonts.Clear();
        }

        #endregion
    }
}
