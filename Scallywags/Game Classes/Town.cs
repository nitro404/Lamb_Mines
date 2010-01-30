using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  Town
     *  @brief  the central town the ships are attempting to pillage
     */
    public class Town : Object3D
    {
        private Port[]  m_Ports;    ///< The town's ports
        private Tower[] m_vTowers;  ///< The town's towers
        private Coin[]  m_vPortCoins;    ///< the coins that indicate if a port has coins at it

        /** @prop   Ports
         *  @brief  the Town's ports
         */
        public Port[] Ports
        {
            get
            {
                return m_Ports;
            }
        }

        /** @prop   Towers
         *  @brief  the town's towers
         */
        public Tower[] Towers
        {
            get
            {
                return m_vTowers;
            }
        }

        /** @prop   PortCoins
         *  @brief  the translucent coins floating above the ports
         */
        public Coin[] PortCoins
        {
            get
            {
                return m_vPortCoins;
            }
        }

        /** @fn     Town(Model mdlTown, Model mdlPort, int nNumPorts )
         *  @brief  Constructor
         *  @param  mdlTown [in] the model to use to display the Town
         *  @param  mdlPort [in] the model to use to display the port
         *  @param  mdlCoin [in] the model to use to display the coin
         *  @param  nNumPorts [in] the number of ports in the town
         */
        public Town(Model mdlTown, Model mdlCoin, int nNumPorts, Effect basicEffect )
            : base(mdlTown)
        {
            m_Ports     = new Port[ nNumPorts ];
            m_vTowers   = new Tower[ nNumPorts ]; //One tower for each port
            m_vPortCoins = new Coin[ nNumPorts ];

            for( int i = 0; i < nNumPorts; ++i )
            {
                m_Ports[i]          = new Port();
                m_vTowers[i]        = new Tower();
                m_vTowers[i].Shaders.Add(basicEffect);

                m_vPortCoins[i]     = new Coin( mdlCoin );

                m_vPortCoins[i].Alpha = 0.5f;
                m_vPortCoins[i].Shaders.Add( basicEffect );
                
            }
        }

        /** @fn     InitPorts()
         *  @brief  Adds default coins to the ports, as well as the locations
         *  @param  vTowerModels [in] the different models that represent the state of the towers
         */
        public void InitPorts( Model[] vTowerModels, Model mdlCannon )
        {
            //! World needs to be Symmetrical and is not currently

            //The port starting locations
            Vector3 [] vPortLocations = {
                new Vector3(   0.0f, 0.0f, 60.0f ),    //North
                new Vector3(-60.0f, 0.0f,   0.0f ),    //East
                new Vector3( 60.0f, 0.0f,   0.0f ),    //West
                new Vector3(   0.0f, 0.0f,-60.0f )     //South
                 };

            float fCoinHeight = 5.0f;

            Vector3 [] vCoinLocations = {
                new Vector3(   0.0f, fCoinHeight, 60.0f ),    //North
                new Vector3(-60.0f, fCoinHeight,   0.0f ),    //East
                new Vector3( 60.0f, fCoinHeight,   0.0f ),    //West
                new Vector3(   0.0f, fCoinHeight,-60.0f )     //South
                 };

            //The tower starting locations
            Vector3[] vTowerLocations = {
                new Vector3(  0.0f, 4.6f, 30.0f ),      //North
                new Vector3(-30.0f, 4.6f,  0.0f ),      //East
                new Vector3( 30.0f, 4.6f,  0.0f ),      //West
                new Vector3(  0.0f, 4.6f,-30.0f ),      //South
                 };

            float[] vTowerDirection = {
                  MathHelper.PiOver2,                   //North
                  0,                                    //East
                  MathHelper.Pi,                        //West
                  MathHelper.PiOver2 + MathHelper.Pi }; //South

            Model[] vModelsTemp = { vTowerModels[ 1 ], vTowerModels[ 2 ], vTowerModels[ 0 ] };

            ////////////////////////
            //Init the objects
            for (int i = 0; i < m_Ports.Length; ++i)
            {
                m_Ports[i].initCoins();
                m_Ports[i].Owner = i;
                m_Ports[i].Location = vPortLocations[ i ];

                m_vTowers[i].Init( vTowerModels, mdlCannon, i, vTowerLocations[i], vTowerDirection[ i ] );

                m_vPortCoins[i].Location = vCoinLocations[ i ];
                m_vPortCoins[i].Roll     = MathHelper.PiOver2;
                m_vPortCoins[i].Scale    = 1.0f;
            }
        }


        /** @fn     Port CheckPorts()
         *  @brief  check if a ship is on a town port
         *  @return reference to the port the ship is on, or null if the ship is not on any of the town ports  
         */
        public Port CheckPorts(Ship ship)
        {
            for (int i = 0; i < m_Ports.Length; i++)
            {
                if( m_Ports[i].checkPort(ship) )
                    return m_Ports[i];
            }

            return null;
        }

        /** @fn     void Draw( GraphicsDevice device, Matrix matView, Matrix matProjection )
         *  @brief  draw the town
         */
        public override void Draw(GraphicsDevice device, Matrix matView, Matrix matProjection, Vector3 CameraPosition)
        {
            //Draw the town
            base.Draw(device, matView, matProjection, CameraPosition);

            //Draw the ports
            for( int i = 0; i < m_Ports.Length; ++i )
            {
                //m_Ports[i].Draw( device, matView, matProjection, CameraPosition );

                if( m_Ports[i].Coins > 0 )
                {
                    //Draw the available coin
                    m_vPortCoins[i].Draw( device, matView, matProjection, CameraPosition );
                }
            }

            foreach( Tower tower in m_vTowers )
                tower.Draw(device, matView, matProjection, CameraPosition);
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the town and ports
         */
        public override void Update(float fElapsedTime)
        {
            foreach( Port port in m_Ports )
                port.Update( fElapsedTime );

            foreach( Tower tower in m_vTowers )
                tower.Update( fElapsedTime );

            foreach( Coin coin in m_vPortCoins )
            {
                coin.Yaw += fElapsedTime * 4.0f;

                if( coin.Yaw > MathHelper.TwoPi )
                    coin.Yaw -= MathHelper.TwoPi;
            }
            

            base.Update(fElapsedTime);
        }
    }
}