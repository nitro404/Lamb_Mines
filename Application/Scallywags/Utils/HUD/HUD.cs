using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Scallywags
{
    public class HUD
    {
        #region DATA_MEMBERS

        private SpriteBatch       m_spriteBatch;                ///< The spriteBatch that we will use to draw with
        private Rectangle         m_viewportRect;               ///< The rectangle size of the screen
                                                       
        private Player[]          m_vPlayers;                   ///< List of players
                                                       
        private Dictionary< string, SpriteFont > m_dtFont;      ///< A list of fonts we can use to write text
        private Dictionary< string, Texture2D > m_dtTexture;    ///< A list of textures
        private bool                m_bIsFiring;  
        private float[]             m_vThumbstickXPositions;  
        private float[]             m_vThumbstickYPositions;
                                                      
        ///////////////////////////
        //Components
        private PlayerStatus[]    m_vPlayerStatus;              ///< The player status HUD conponent
        private MoveBar           m_moveBar;                    ///< The move bar hud item
        private Popup             m_popup;                      ///< The popup message
        private CannonFuse        m_fuse;                       ///< The cannon fuse
        private ButtonDisplay     m_button;                     ///< The displayer for the xbox buttons on screen   
        private ThumbstickAnimation[] m_aniThumbstick;                                                                         
        
        private TurnDisplay       m_turnDisplay;                ///< The hud item to show how many turns have elapsed/are left         
        private Rectangle         m_rectSafeArea;               ///< the region of the screen it's safe to draw to  
                                                                ///
                                                        
        #endregion

        #region PROPERTIES

        /** @fn     PlayerStatus PlayerStatusHud
         *  @brief  Get the handle to the playerStatus
         */
        public PlayerStatus[] PlayerStatusHUD
        {
            get
            {
                return m_vPlayerStatus;
            }
        }

        public ButtonDisplay ButtonDisplay
        {
            get
            {
                return m_button;
            }
        }
        
        public TurnDisplay TheTurnDisplay
        {
            get
            {
                return m_turnDisplay;
            }
        }

        public bool IsFiring
        {
            set
            {
                m_bIsFiring = value;
            }
        }

        #endregion

        #region CLASS_CONSTRUCTOR

        /** @fn     private HUD()
         *  @brief  Class Constructor
         */
        public HUD()
        {
            m_spriteBatch       = null;            
            m_viewportRect      = new Rectangle( 0, 0, 0, 0 );

            m_vPlayerStatus     = null;
            m_rectSafeArea      = new Rectangle();
            m_moveBar           = new MoveBar();
            m_popup             = new Popup();
            m_fuse              = new CannonFuse();
            m_button            = new ButtonDisplay();
            m_dtFont            = new Dictionary<string,SpriteFont>();
            m_aniThumbstick     = null;
            m_vThumbstickXPositions = null;
            m_vThumbstickYPositions = null;

            m_turnDisplay       = new TurnDisplay();
        }

        #endregion

        #region METHODS

        /** @fn     void Init( int nViewportWidth, int nViewportHeight, SpriteBatch spriteBatch )
         *  @brief  Initializes the HUD
         *  @param  nViewportWidth [in] the width of the screen size
         *  @param  nViewportHeight [in] the height of the screen size
         *  @param  spriteBatch [in] the handle to the spritebatch
         *  @param  vPlayer [in] the array of players
         *  @param  world [in] the game world instance
         */
        public void Init( int nViewportWidth, int nViewportHeight, SpriteBatch spriteBatch, Player[] vPlayer, World world, TurnManager tm, Dictionary< string, Texture2D > dtTextures, Dictionary< string, SpriteFont > dtFonts )
        {
            m_spriteBatch  = spriteBatch;
            m_viewportRect = new Rectangle( 0, 0, nViewportWidth, nViewportHeight );
            m_dtTexture = dtTextures;
            m_dtFont = dtFonts;

            m_vPlayers = vPlayer;

            //Calculate the safe area
            m_rectSafeArea = new Rectangle( 
                (int)( nViewportWidth * Settings.SAFE_REGION_PERCENTAGE ),
                (int)(nViewportHeight * Settings.SAFE_REGION_PERCENTAGE),
                nViewportWidth - (int)(nViewportWidth * Settings.SAFE_REGION_PERCENTAGE * 2),
                nViewportHeight - (int)(nViewportHeight * Settings.SAFE_REGION_PERCENTAGE * 2)); 
  
                        
            ////////////////////////////////////////
            //Init hud items
            m_vPlayerStatus = new PlayerStatus[m_vPlayers.Length];

            //Create the correct amount of player status hud items
            for (int i = 0; i < m_vPlayerStatus.Length; ++i)
            {
                m_vPlayerStatus[i] = new PlayerStatus();

                CharacterInfo charInfo = new CharacterInfo( m_vPlayers[ i ].CharacterID, false );

                m_vPlayerStatus[i].Init( 
                    m_vPlayers[i], 
                    dtTextures[ charInfo.strPortrait ], 
                    dtTextures );
            }

            m_moveBar.Init( spriteBatch.GraphicsDevice, 
                            dtTextures[ "MovementBarBorder" ], 
                            dtTextures[ "MovementBarBacking" ], 
                            dtTextures[ "MovementBarWater" ] );
            m_fuse.Init(dtTextures["HUD_cannon"], dtTextures["HUD_fuse"]);

            m_turnDisplay.Init( dtTextures[ "TurnBG" ], dtTextures[ "TurnInfinity" ], dtFonts[ "PirateFontLarge" ] );
            m_turnDisplay.ScreenPosition = new Vector2( m_rectSafeArea.X + ( m_rectSafeArea.Width * 0.5f - m_turnDisplay.Width * 0.5f ), m_rectSafeArea.Y );

            ////////////////////////////////////////////////
            //Position all the hud items
            //////////////////////////////////

            //The possible X  and Y positions of the player status hud items
            float[] vPlayerStatusXPositions = new float[4];
            vPlayerStatusXPositions[0] = m_rectSafeArea.X;
            vPlayerStatusXPositions[1] = m_rectSafeArea.X + m_rectSafeArea.Width - m_vPlayerStatus[0].Width;
            vPlayerStatusXPositions[2] = m_rectSafeArea.X + m_rectSafeArea.Width - m_vPlayerStatus[0].Width;
            vPlayerStatusXPositions[3] = m_rectSafeArea.X;

            int nBottomY = m_rectSafeArea.Y + m_rectSafeArea.Height - m_vPlayerStatus[0].Height;

            float[] vPlayerStatusYPositions = new float[4];
            vPlayerStatusYPositions[0] = m_rectSafeArea.Y;
            vPlayerStatusYPositions[1] = m_rectSafeArea.Y;
            vPlayerStatusYPositions[2] = nBottomY;
            vPlayerStatusYPositions[3] = nBottomY;

            //The X and Y positions of the 4 thumbsitcks
            m_vThumbstickXPositions = new float[4];
            m_vThumbstickXPositions[0] = m_rectSafeArea.X + m_vPlayerStatus[0].Width + 20.0f;
            m_vThumbstickXPositions[1] = m_rectSafeArea.X + m_rectSafeArea.Width - m_vPlayerStatus[0].Width - 20.0f;
            m_vThumbstickXPositions[2] = m_rectSafeArea.X + m_rectSafeArea.Width - m_vPlayerStatus[0].Width - 20.0f;
            m_vThumbstickXPositions[3] = m_rectSafeArea.X + m_vPlayerStatus[0].Width + 20.0f;

            m_vThumbstickYPositions = new float[4];
            m_vThumbstickYPositions[0] = m_rectSafeArea.Y + m_vPlayerStatus[0].Height;
            m_vThumbstickYPositions[1] = m_rectSafeArea.Y + m_vPlayerStatus[0].Height;
            m_vThumbstickYPositions[2] = m_rectSafeArea.Y + m_rectSafeArea.Height - m_vPlayerStatus[0].Height;
            m_vThumbstickYPositions[3] = m_rectSafeArea.Y + m_rectSafeArea.Height - m_vPlayerStatus[0].Height;

            m_aniThumbstick = new ThumbstickAnimation[ 4 ];
            for( int i = 0; i < m_aniThumbstick.Length; ++i )
            {
                m_aniThumbstick[ i ] = new ThumbstickAnimation();
                m_aniThumbstick[ i ].Init( m_vThumbstickXPositions[ i ], m_vThumbstickYPositions[ i ], dtTextures );
            }

            //Player status icons
            for (int i = 0; i < m_vPlayerStatus.Length; ++i)
            {
                m_vPlayerStatus[i].X = vPlayerStatusXPositions[i];
                m_vPlayerStatus[i].Y = vPlayerStatusYPositions[i];
            }

            //Fuse
            int nFuseLeft = (int)(m_rectSafeArea.X + m_rectSafeArea.Width / 2 - m_fuse.Width / 2);
            int nFuseTop = (int)(m_rectSafeArea.Y + m_rectSafeArea.Height - m_fuse.Height);

            m_fuse.X = nFuseLeft;
            m_fuse.Y = nFuseTop;

            //Popup
            int nCenterX = m_rectSafeArea.X + m_rectSafeArea.Width / 2;
            int nCenterY = m_rectSafeArea.Y + m_rectSafeArea.Height / 8;

            m_popup.X = nCenterX;
            m_popup.Y = nCenterY;
            
            //Move bar
            int nMoveBarLeft = (int)( m_rectSafeArea.X ) + m_rectSafeArea.Width / 2 - m_moveBar.Width / 2;
            int nMoveBarTop = (int)( m_rectSafeArea.Y )  + m_rectSafeArea.Height - m_moveBar.Height ;
           
            m_moveBar.X = nMoveBarLeft;
            m_moveBar.Y = nMoveBarTop;

            //button display init
            m_button.X = nViewportWidth / 2;
            m_button.Y = nViewportHeight - (int)( nViewportHeight / 3.5 );
            
            m_button.Init( dtTextures );
        } 
        
        /** @fn     void ShowPopupMessage( string strMessage, Color clr )
         *  @param  strMessage [in] the message to show
         *  @param  clr [in] the colour of the messge text
         */
        public void ShowPopupMessage( string strMessage, Color clr )
        {
            if( m_popup != null )
            {
                m_popup.ShowMessage( strMessage, clr );
            }
        }

        /** @fn     void LightFuse( bool bLightFuse )
         *  @brief  light the fuse hud item
         *  @param  bLightFuse [in] true to turn the fuse on, false to turn it off
         */
        public void LightFuse( bool bLightFuse )
        {
            m_fuse.LightFuse( bLightFuse );
        }

        /** @fn     void Update()
         *  @brief  Updates our HUD
         */
        public void Update( float fElapsedTime, bool bIsAITurn, float IdleTime  ) 
        {
            int Turn = 0;
            for( int i = 0; i < m_vPlayerStatus.Length; ++i )
            {
                m_vPlayerStatus[i].Update( fElapsedTime );

                if (m_vPlayers[i].Active)
                {
                    Turn = i;
                    m_moveBar.CurrentPlayer = m_vPlayers[i];
                }
            }

            for( int i = 0; i < m_aniThumbstick.Length; ++i )
            {
                m_aniThumbstick[ i ].Update( fElapsedTime );
            }

            if( bIsAITurn == false )
            {
                m_moveBar.Update( fElapsedTime );
                m_fuse.Update(fElapsedTime);
            }

            if (!bIsAITurn && !m_vPlayers[Turn].IsAI)
                m_button.Update( fElapsedTime, IdleTime );

            m_popup.Update( fElapsedTime );

            m_turnDisplay.Update( fElapsedTime );
        }

        /** @fn     void Draw( GraphicsDevice device ) 
         *  @brief  Draws our hud on screen
         *  @param  device [in] the graphics device
         *  @param  font [in] the debug font 
         */
        public void Draw( GraphicsDevice device, bool bIsAITurn )
        {
            m_spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            if( Settings.DRAW_SAFE_REGION )
            {
                //Draw the safe area
                Texture2D texSafeArea = new Texture2D(device, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
                texSafeArea.SetData<Color>(new Color[] { new Color(255, 255, 255, 20) });
                m_spriteBatch.Draw(texSafeArea, m_rectSafeArea, Color.White);
            }
            
            if( m_bIsFiring == true )
            {
                for( int i = 0; i < m_aniThumbstick.Length; ++i )
                {
                    
                    string strMessage = "Influence the Shot";
                    Vector2 vStringDimention = Vector2.Zero;
                    vStringDimention = ( m_dtFont[ "PirateFont" ].MeasureString( strMessage ) );
                    if( i == 0 || i == 3 )
                    {
                        m_spriteBatch.Draw( m_dtTexture[ "HUD_woodBacking" ], 
                                            new Vector2( m_vThumbstickXPositions[ i ] - 20, m_vThumbstickYPositions[i] - ( m_dtTexture[ "HUD_woodBacking" ].Height / 2 ) ),
                                            Color.White );
                        m_spriteBatch.DrawString( m_dtFont[ "PirateFont" ], 
                                                  strMessage, 
                                                  new Vector2( m_vThumbstickXPositions[i] + vStringDimention.X / 5, m_vThumbstickYPositions[i] - 15),
                                                  Color.Yellow );
                    }
                    else
                    {
                        m_spriteBatch.Draw( m_dtTexture[ "HUD_woodBacking" ], 
                                            new Vector2( m_vThumbstickXPositions[i] - ( vStringDimention.X + vStringDimention.X / 5 ) - 15, m_vThumbstickYPositions[i] - ( m_dtTexture[ "HUD_woodBacking" ].Height / 2 ) ),
                                            Color.White );
                        m_spriteBatch.DrawString( m_dtFont[ "PirateFont" ], 
                                                  strMessage, 
                                                  new Vector2( m_vThumbstickXPositions[i] - ( vStringDimention.X + vStringDimention.X / 5 ), m_vThumbstickYPositions[i] - 15 ),
                                                  Color.Yellow );
                    }

                    m_aniThumbstick[ i ].Draw( m_spriteBatch );
                }
            }
            //m_button.Draw( m_spriteBatch, "Thumbstick.Right" );
            //////////////////////////////////
            //Draw the player status icons - 4 corners
            foreach( PlayerStatus ps in m_vPlayerStatus )
                ps.Draw( m_spriteBatch, m_dtFont[ "PirateFontMedium" ] );

            if( bIsAITurn == false )
            {

                if( m_fuse.Active )
                {
                    m_fuse.Draw(m_spriteBatch, m_dtFont[ "PirateFont" ] );
                }
                else
                {
                    m_moveBar.Draw( m_spriteBatch, m_dtFont[ "PirateFont" ]  );
                }
                

                m_button.Draw(m_spriteBatch, m_dtFont["PirateFont"]);
            }

            m_popup.Draw( m_spriteBatch, m_dtFont[ "PirateFontLarge" ] );

            m_turnDisplay.Draw( m_spriteBatch, m_dtFont[ "PirateFontMedium" ] );

            m_spriteBatch.End();
        }

        /** @fn     void Clear()
         *  @brief  clear the array(s)
         */
        public void Clear()
        {
            m_popup.Clear();
        }

        #endregion
    }
}
