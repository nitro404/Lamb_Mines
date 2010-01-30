using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  World
     *  @brief  the game world, which contains all the objects in the world
     */
    public class World
    {
        ///////////////////////////////////
        //Gameplay related
        private Town                m_town;                 ///< The central town
        private CannonBall          m_cannonBall;           ///< The cannon ball
        private List<Port>          m_lstCoves;             ///< The pirate coves
        private List<Ship>          m_lstShips;             ///< The list of ships
        private List<Coin>          m_lstCoins;             ///< The list of coins
        private List<Object3D>      m_lstTicks;             ///< The list of Arc Ticks
        private List<Vector3>       m_WaveLocations;
        private List<Targetter>     m_lstTargetters;

        private CannonBall          m_TossedCoin;     

        /////////////////////////////////////////////
        //Display lists and rendering
        private List< Object3D >    m_lstGameObjects;       ///< The objects that the world will display
        private List< Object3D >    m_lstBlendObjects;      ///< The game objects to be drawn with blending                                                   
        private Effect              m_effLightingShader;    ///< The general lighting shader...\                           
                      
        ////////////////////////////////////
        //Lights
        private Vector3             m_vAmbientLight;        ///< The ambient light colour
        private Light               m_lightSun;             ///< The sun, a directional light
        private List<Light>         m_lstDirectionalLights; ///< The directional lights in the world
        private List<PointLight>    m_lstPointLights;       ///< The poitn lights in the world

        #region PROPERTIES

        /** @prop   Town TheTown
         *  @brief  the world's town
         */
        public Town TheTown
        {
            get
            {
                return m_town;
            }
            set
            {
                value.Shaders.Add(m_effLightingShader);
                m_town = value;
            }
        }

        public CannonBall TossedCoin
        {
            get
            {
                return m_TossedCoin;
            }
        }

        /** @prop   List<Vector3> WaveLocations
         *  @brief  the Wave Locations
         */
        public List<Vector3> WaveLocations
        {
            get
            {
                return m_WaveLocations;
            }
            set
            {
                m_WaveLocations = value;
            }
        }

        /** @prop   List<Targettters> Targetters
         *  @brief  the Boat Targetters
         */
        public List<Targetter> Targetters
        {
            get
            {
                return m_lstTargetters;
            }
        }

        /** @prop   List<Port> PirateCoves
         *  @brief  the array of pirate coves
         */
        public List<Port> PirateCoves
        {
            get
            {
                return m_lstCoves;
            }
        }

        /** @prop   List<Ship> Ships
         *  @brief  the ships in the game world
         */
        public List<Ship> Ships
        {
            get
            {
                return m_lstShips;
            }
        }

        /** @prop   List< Coin > Coins
         *  @brief  the available coins
         */
        public List<Coin> Coins
        {
            get
            {
                return m_lstCoins;
            }
        }

        /** @prop   CannonBall
         *  @brief  the active cannonball
         */
        public CannonBall TheCannonBall
        {
            get
            {
                return m_cannonBall;
            }
            set
            {
                m_cannonBall = value;
            }
        }

        #endregion

        /** @fn     World
         *  @brief  Constructor
         */
        public World( )
        {
            m_vAmbientLight         = new Vector3();

            m_lstDirectionalLights  = new List<Light>();
            m_lstPointLights        = new List< PointLight >();
            m_lstGameObjects        = new List<Object3D>();
            m_lstBlendObjects       = new List<Object3D>();
            m_lstTargetters         = new List<Targetter>();
            m_effLightingShader     = null;
            m_cannonBall            = null;

            m_lstShips = new List<Ship>();
            m_lstCoves = new List<Port>();

            m_lstCoins = new List<Coin>();
            m_lstTicks = new List<Object3D>();
        }

        /** @fn     Init( Effect effLightingShader )
         *  @brief  initialize the world
         *  @param  effLightingShader [in] the shader
         */
        public void Init( Effect effLightingShader )
        {
            m_effLightingShader = effLightingShader;

            m_lstTargetters.Clear();

            m_vAmbientLight = new Vector3(0.5f, 0.5f, 0.5f);

            ///////////////////////////////
            //Create a "sun" light
            m_lightSun          = new Light();
            m_lightSun.Diffuse  = new Color(0.9f, 0.9f, 0.8f);
            m_lightSun.Specular = new Color(0.3f, 0.3f, 0.3f);

            m_lightSun.Direction = Vector3.Normalize(new Vector3(-1.0f, -1.0f, -1.0f));

            AddDirectionalLight(m_lightSun);

            UpdateLightValues();
        }
        /** @fn     void AddCoin( Coin coin )
         *  @brief  Add a coin to the list
         *  @param  coin [in] the referance to the coin
         */
        public void AddCoin( Coin coin )
        {
            coin.Shaders.Add( m_effLightingShader );
            m_lstCoins.Add( coin );
        }

        /** @fn     void AddCoinToss( Cannonball Coin )
        *  @brief  Add a coin Toss to the list
        *  @param  cannonball [in] the referance to the coin
        */
        public void AddCoinToss(CannonBall coin)
        {
            m_TossedCoin = coin;
            coin.Shaders.Add(m_effLightingShader);
            m_lstGameObjects.Add(coin);
        }

        /** @fn     void SetCannonBall( CannonBall cb )
         *  @brief  set the active cannonball
         *  @param  cb [in] the cannonball
         */
        void SetCannonBall( CannonBall cb )
        {
            m_cannonBall = cb;
        }

        /** @fn     void RemoveCoin( Coin coin )
         *  @brief  Removes the coin from the list
         *  @param  coin [in] the referance to the coin we want to remove
         */
        public void RemoveCoin( Coin coin )
        {
            for( int i = 0; i < m_lstCoins.Count; ++i )
            {
                if( m_lstCoins[ i ] == coin )
                {
                    m_lstCoins.RemoveAt( i );
                    break;
                }
            }
        }

        /** @fn     void AddArc( List<Object3D> )
         *  @brief  Adds a List of Ticks to the Arc List to be drawn
         *  @param  ArcTicks [in] the referance to the list of ticks we want to add
         */
        public void AddArc(List<Object3D> ArcTicks)
        {
            foreach (Object3D Tick in ArcTicks)
            {
                Tick.Shaders.Add(m_effLightingShader);
                m_lstTicks.Add(Tick);
            }
        }


        /** @fn     void ClearArc( )
         *  @brief  Clears the current List of Arc Ticks
         */
        public void ClearArc()
        {
            m_lstTicks.Clear();
        }

        /** @fn     void AddCove( Port cove )
         *  @brief  add a pirate cove
         *  @param  cove [in] the cove to add
         */
        public void AddCove(Port cove)
        {
            m_lstCoves.Add(cove);
            AddGameObject(cove, true);
        }

        /** @fn     void AddShip( Ship ship )
         *  @brief  add a ship to the world
         *  @param  ship [in] the ship to add
         */
        public void AddShip(Ship ship)
        {
            m_lstShips.Add(ship);
            AddGameObject( ship, true );
        }

        public void AddTargetter(Targetter obj, bool bUseLightingShader)
        {
            if (bUseLightingShader)
                obj.Shaders.Add(m_effLightingShader);

            m_lstTargetters.Add(obj);
        }

        /** @fn     void AddGameObject( Object3D obj, bool bUseLightingShader )
         *  @brief  add a game object to the world
         *  @param  obj [in] the display object to add
         *  @param  bUseLightingShader [in] does the object use the world lighting shader?
         */
        public void AddGameObject( Object3D obj, bool bUseLightingShader )
        {
            if( bUseLightingShader )
                obj.Shaders.Add( m_effLightingShader );

            m_lstGameObjects.Add(obj);
        }

        /** @fn    void RemoveGameObject( Object3D obj )
        *  @brief  remove a game object from the world
        *  @param  obj [in] the display object to remove
        */
        public void RemoveGameObject(Object3D obj)
        {
            m_lstGameObjects.Remove(obj);
            obj.Shaders.Clear();
        }

        /** @fn     void AddBlendedGameObject(Object3D objBlend, bool bUseLightingShader)
         *  @brief  add a game object that will be blended with the opaque objects
         *  @param  objBlend [in] the object to draw
         *  @param  bUseLightingShader [in] does the object use the world lighting shader?
         */
        public void AddBlendedGameObject(Object3D objBlend, bool bUseLightingShader)
        {
            if (bUseLightingShader)
                objBlend.Shaders.Add(m_effLightingShader);

            m_lstBlendObjects.Add( objBlend );
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the game objects
         *  @param  fElapsedTime [in] the time elapsed sinec the previous frame
         */
        public void Update( float fElapsedTime )
        {
            foreach (Object3D obj in m_lstGameObjects)
            {
                obj.Update( fElapsedTime );
            }

            foreach( Coin coin in m_lstCoins )
            {
                coin.Update( fElapsedTime );
            }

            foreach (Object3D Tick in m_lstTicks)
            {
                Tick.Update(fElapsedTime );
            }
            
            foreach( Object3D blendObj in m_lstBlendObjects )
            {
                blendObj.Update( fElapsedTime );
            }

            m_town.Update(fElapsedTime);
            m_cannonBall.Update( fElapsedTime );
        }

        /** @fn     void Draw( Matrix matProj, Matrix matView )
         *  @brief  draw the world and all its objects
         *  @param  matView [in] the active view matrix
         *  @param  matProj [in] the active projection matrix         
         */
        public void Draw( GraphicsDevice device, Matrix matView, Matrix matProj, Vector3 CameraPosition )
        {
            //UpdateLightValues();

            //Draw the non-removable game objects
            foreach (Object3D obj in m_lstGameObjects)
            {
                obj.Draw(device, matView, matProj, CameraPosition);
            }

            //Draw the coins
            foreach( Coin coin in m_lstCoins )
            {
                coin.Draw(device, matView, matProj, CameraPosition);
            }

            foreach (Targetter Target in m_lstTargetters)
            {
                Target.Draw(device, matView, matProj, CameraPosition);
            }

            //Draw the Arc
            foreach (Object3D Tick in m_lstTicks)
            {
                Tick.Draw(device, matView, matProj, CameraPosition);
            }

            m_town.Draw(device, matView, matProj, CameraPosition);
            m_cannonBall.Draw(device, matView, matProj, CameraPosition);

            //**Remember to do this last...
            //Post Processing
            //if (Settings.DETECT_EDGES == false)
            //{
                //Draw the objects that will blend with the world
                foreach (Object3D objBlend in m_lstBlendObjects)
                {
                    objBlend.Draw(device, matView, matProj, CameraPosition);
                }
            //}

        }

        /** @fn     void Clear()
         *  @brief  clear the world object array(s)
         */
        public void Clear()
        {
            m_lstGameObjects.Clear();
            m_lstBlendObjects.Clear();

            m_lstDirectionalLights.Clear();
            m_lstPointLights.Clear();

            m_lstShips.Clear();
            m_lstCoves.Clear();

            m_lstCoins.Clear();
            m_lstTicks.Clear();
        }

        /** @fn     void AddDirectionalLight( Light ltDir )
         *  @brief  add a directional light to the world
         *  @param  ltDir [in] the light to add
         */
        void AddDirectionalLight( Light ltDir )
        {
            if( ltDir != null )
                m_lstDirectionalLights.Add( ltDir );
        }

        /** @fn     void UpdateLightValues()
         *  @brief  set the lighting values in the shader
         */
        private void UpdateLightValues()
        {
            //Update directional lights
            for( int i = 0; i < m_lstDirectionalLights.Count; ++ i )
            {
                Light lt = m_lstDirectionalLights[ i ];

                //Parameter names
                string strDirParamName      = "DirLight" + i.ToString() + "Direction";
                string strDiffuseParamName  = "DirLight" + i.ToString() + "DiffuseColor";
                string strSpecParamName = "DirLight" + i.ToString() + "SpecularColor";

                //Colours
                if( m_effLightingShader.Parameters["AmbientLightColor"] != null )
                    m_effLightingShader.Parameters["AmbientLightColor"].SetValue( m_vAmbientLight );

                if (m_effLightingShader.Parameters[strDiffuseParamName] != null)
                    m_effLightingShader.Parameters[strDiffuseParamName].SetValue(lt.Diffuse.ToVector3());

                if (m_effLightingShader.Parameters[strSpecParamName] != null)
                    m_effLightingShader.Parameters[strSpecParamName].SetValue(lt.Specular.ToVector3());
                
                //Direction
                if( m_effLightingShader.Parameters[ strDirParamName ] != null )
                    m_effLightingShader.Parameters[ strDirParamName ].SetValue( lt.Direction );
            }
        }      
    }
}
