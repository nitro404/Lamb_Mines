using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  PlayerStatus
     *  @brief  the player status on the HUD
     */
    public class PlayerStatus : HudItem
    {
        private const float DISPLAY_TIME = 40.0f;

        #region DATA_MEMBERS

        private Player          m_player;                       ///< The player the hud item is monitorin                           
        private Texture2D       m_texPlayerPortrait;            ///< The player's portrait
        private Texture2D       m_texMiniRing;                  ///< The mini ring around the border that displays the ships status 
        private Texture2D       m_texMiniRingHalf;                                                                              
        private Animation       m_animGlow;                     ///< The glow animation
        
        private Texture2D       m_texCannonBall;                                                                  

        //private Texture2D       m_texGoldDisplay;               ///< The display section for the gold                                                            
        //private Vector2         m_vGoldDisplayPosition;         ///< The position of the gold displayer    
        private GoldDisplay     m_goldDisplay;                  ///< The player's gold display


        private Texture2D       m_texUp;                        ///< The texture for the ship being Up
        private Texture2D       m_texDown;                      ///< The texture for the ship being down
        private Texture2D       m_texCoin;                      ///< The texture for the ship having coin
        private Texture2D       m_texBuy;                       ///< The texture for being able to buy a ship
        private Texture2D       m_texEmpty;                     ///< The texture for not haveing anything                                            

        private Vector2[]       m_vMiniRingPositions;           ///< The position of the center of the minirings    
        private Vector2[]       m_vCannonBallPositions;                                                                
                                                                
        private float           m_fDisplaytime;                 ///< The time we will be displaying the button
        private Color           m_colFade;                      ///< to apply a fade to the object                                                           

        private bool            m_bPreviouslyActive;            ///< Flag to check when a player becomes active

        private float           m_fAlpha;                       //How visible is the hud item?

        #endregion

        #region CLASS_CONSTRUCTOR

        /** @fn     PlayerStatus()  
         *  @brief  constructor
         */
        public PlayerStatus()
        {
            m_player                    = null;
            m_bPreviouslyActive         = false;
            m_texPlayerPortrait         = null;
            m_texMiniRing               = null;
            m_animGlow                  = null;
            m_goldDisplay               = null;

            m_vMiniRingPositions        = null;
            m_vCannonBallPositions      = null;
            
            m_fAlpha                    = 1.0f;
        }

        #endregion

        #region METHODS

        /** @fn     Vector2 GetMiniRingCenter( Vector2 vCenter, float fRadius, float fDrawAngle )
         *  @brief  get the draw rect for a mini ring
         */
        private Vector2 GetMiniRingCenter( Vector2 vCenter, float fRadius, float fDrawAngle )
        {
            Vector2 vRingCenter = Vector2.Zero;

            float fMiniRingRadius = 15.0f;

            float fCos = (float)Math.Cos( fDrawAngle );
            float fSin = (float)Math.Sin( fDrawAngle );

            vRingCenter.X = vCenter.X + ( fRadius + fMiniRingRadius ) * ( fCos );
            vRingCenter.Y = vCenter.Y + ( fRadius + fMiniRingRadius ) * ( fSin );

            return vRingCenter;
        }

        /** @fn     void Init( Player plr )
         *  @brief  initialize the player status HUD item
         *  @param  plr [in] the player this item will monitor
         */
        public void Init( Player plr, Texture2D playerPortrait, Dictionary< string, Texture2D > dtTextures )
        {
            m_vMiniRingPositions = new Vector2[ 3 ];
            for( int i = 0; i < m_vMiniRingPositions.Length; ++i )
            {
                m_vMiniRingPositions[ i ] = Vector2.Zero;;
            }
           
            m_vCannonBallPositions = new Vector2[ 3 ];
            for( int i = 0; i < m_vCannonBallPositions.Length; ++i )
            {
                m_vCannonBallPositions[ i ] = Vector2.Zero;
            }

            m_player                = plr;  
            m_texPlayerPortrait     = playerPortrait;
            //m_texBorder             = dtTextures[ "HUD__0003_rings" ];
            m_texMiniRing           = dtTextures[ "HUD_miniRing" ];
            m_texMiniRingHalf       = dtTextures[ "HUD_miniRingHalf" ];

            AnimatedTexture glowTex = new AnimatedTexture( dtTextures[ "HUD_glow2" ], 180, 180 );
            m_animGlow              = new Animation( glowTex , 0, 39, false, 0.04f );
            m_animGlow.CurrentFrame = 0;    //Idle frame
            //m_texGoldDisplay        = dtTextures[ "HUD_GoldDisplay" ];

            m_goldDisplay = new GoldDisplay();


            ////////////////////////////
            //The boat status icons
            m_texUp     = dtTextures[ "HUD_psBoatUp" ];
            m_texDown   = dtTextures[ "HUD_psBoatDown" ];
            m_texCoin   = dtTextures[ "HUD_psGold" ];
            m_texBuy    = dtTextures[ "HUD_psBuy" ];
            m_texEmpty  = dtTextures[ "HUD_psEmpty" ];

            float fDrawAngle    = 0.0f;
            Vector2 vTopLeft    = new Vector2( 0.0f, 0.0f );
            Vector2 vCenter     = new Vector2( glowTex.CellWidth * 0.5f, glowTex.CellHeight * 0.5f );
         
            float fRadius       = 65.0f;    //As measured in photoshop
            float fRotationStep = MathHelper.Pi / 5.75f;

            float fGoldRotationStep  = fRotationStep * 1.5f;
            float fGoldDisplayRadius = fRadius + 22.50f;

            switch ( m_player.playerNumber )
            {
                case 0:
                    {
                        fDrawAngle = MathHelper.PiOver2;       
                        m_vMiniRingPositions[ 0 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 0 ] = new Vector2( m_vMiniRingPositions[ 0 ].X + m_texUp.Width * 0.4f, m_vMiniRingPositions[ 0 ].Y + m_texUp.Height * 0.4f );
                        
                        fDrawAngle -= fRotationStep;
                        m_vMiniRingPositions[ 1 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 1 ] = new Vector2( m_vMiniRingPositions[ 1 ].X + m_texUp.Width * 0.4f, m_vMiniRingPositions[ 1 ].Y + m_texUp.Height * 0.4f );

                        fDrawAngle -= fRotationStep;
                        m_vMiniRingPositions[ 2 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 2 ] = new Vector2( m_vMiniRingPositions[ 2 ].X + m_texUp.Width * 0.4f, m_vMiniRingPositions[ 2 ].Y + m_texUp.Height * 0.4f );

                        fDrawAngle -= fGoldRotationStep;
                        m_goldDisplay.Init( dtTextures[ "HUD_GoldDisplay" ], GetMiniRingCenter(vCenter, fGoldDisplayRadius, fDrawAngle ) );
                        break;
                    }
                case 1:
                    {
                        //m_vGoldDisplayPositions = GetMiniRingCenter( vCenter, fRadius - 3.0f, ( MathHelper.Pi + MathHelper.PiOver2 ) );
                        fDrawAngle = MathHelper.PiOver2;       
                        m_vMiniRingPositions[ 2 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 2 ] = new Vector2( m_vMiniRingPositions[ 2 ].X - m_texUp.Width * 0.4f, m_vMiniRingPositions[ 2 ].Y + m_texUp.Height * 0.4f );
                        
                        fDrawAngle += fRotationStep;
                        m_vMiniRingPositions[ 1 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 1 ] = new Vector2( m_vMiniRingPositions[ 1 ].X - m_texUp.Width * 0.4f, m_vMiniRingPositions[ 1 ].Y + m_texUp.Height * 0.4f );
                        
                        fDrawAngle += fRotationStep;
                        m_vMiniRingPositions[ 0 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 0 ] = new Vector2( m_vMiniRingPositions[ 0 ].X - m_texUp.Width * 0.4f, m_vMiniRingPositions[ 0 ].Y + m_texUp.Height * 0.4f );

                        fDrawAngle += fGoldRotationStep;
                        m_goldDisplay.Init(dtTextures["HUD_GoldDisplay"], GetMiniRingCenter(vCenter, fGoldDisplayRadius, fDrawAngle));
                        break;
                    }
                case 2:
                    {
                        //m_vGoldDisplayPositions = GetMiniRingCenter( vCenter, fRadius - 3.0f, ( MathHelper.PiOver2 ) );

                        fDrawAngle = MathHelper.Pi + MathHelper.PiOver2;      
                        m_vMiniRingPositions[ 2 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 2 ] = new Vector2( m_vMiniRingPositions[ 2 ].X - m_texUp.Width * 0.4f, m_vMiniRingPositions[ 2 ].Y - m_texUp.Height * 0.4f );;
                        
                        fDrawAngle -= fRotationStep;
                        m_vMiniRingPositions[ 1 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 1 ] = new Vector2( m_vMiniRingPositions[ 1 ].X - m_texUp.Width * 0.4f, m_vMiniRingPositions[ 1 ].Y - m_texUp.Height * 0.4f );
                        
                        fDrawAngle -= fRotationStep;
                        m_vMiniRingPositions[ 0 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 0 ] = new Vector2( m_vMiniRingPositions[ 0 ].X - m_texUp.Width * 0.4f, m_vMiniRingPositions[ 0 ].Y - m_texUp.Height * 0.4f );

                        fDrawAngle -= fGoldRotationStep;
                        m_goldDisplay.Init(dtTextures["HUD_GoldDisplay"], GetMiniRingCenter(vCenter, fGoldDisplayRadius, fDrawAngle));

                        break;
                    }
                case 3:
                    {
                        //m_vGoldDisplayPositions = GetMiniRingCenter( vCenter, fRadius - 3.0f, ( MathHelper.PiOver2 ) );

                        fDrawAngle = MathHelper.Pi + MathHelper.PiOver2;
                        m_vMiniRingPositions[ 0 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 0 ] = new Vector2( m_vMiniRingPositions[ 0 ].X + m_texUp.Width * 0.4f, m_vMiniRingPositions[ 0 ].Y - m_texUp.Height * 0.4f );
                        
                        fDrawAngle += fRotationStep;
                        m_vMiniRingPositions[ 1 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 1 ] = new Vector2( m_vMiniRingPositions[ 1 ].X + m_texUp.Width * 0.4f, m_vMiniRingPositions[ 1 ].Y - m_texUp.Height * 0.4f );
                        
                        fDrawAngle += fRotationStep;
                        m_vMiniRingPositions[ 2 ] = GetMiniRingCenter( vCenter, fRadius, fDrawAngle );
                        m_vCannonBallPositions[ 2 ] = new Vector2( m_vMiniRingPositions[ 2 ].X + m_texUp.Width * 0.4f, m_vMiniRingPositions[ 2 ].Y - m_texUp.Height * 0.4f );

                        fDrawAngle += fGoldRotationStep;
                        m_goldDisplay.Init(dtTextures["HUD_GoldDisplay"], GetMiniRingCenter(vCenter, fGoldDisplayRadius, fDrawAngle));

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            
            ////////////////////////////
            //Init of the cannon ball stuff
            m_texCannonBall = dtTextures[ "cannonBall" ];

            m_colFade       = Color.White;
            m_colFade.A     = 0;
            m_fDisplaytime  = DISPLAY_TIME;
                        
            //Adjust width and height
            base.Init( glowTex.CellWidth, glowTex.CellHeight );
        }
        
        /** @fn     void Draw( SpriteBatch spriteBatch,  SpriteFont font )
         *  @brief  draw the hud item
         *  @param  spriteBatch [in] a handle to the drawing device
         *  @param  font [in] the debug font
         */
        public override void Draw( SpriteBatch spriteBatch,  SpriteFont font )
        {
            //Draw the portrait
            spriteBatch.Draw( m_texPlayerPortrait, new Vector2( X, Y ), m_colFade );

            //Draw the border
            Color colTint = m_colFade;
            
            byte nAlpha = (byte)(255 * m_fAlpha);
            if( m_colFade.A > nAlpha )
                m_colFade.A = nAlpha;

            if( m_player.Active )
            {
                colTint    = Color.Gold;
                colTint.A  = m_colFade.A;
            }

            m_animGlow.Tint = colTint;
            m_animGlow.Draw( new Vector2( X + (Width) * 0.5f, Y + (Height) * 0.5f ), 0, spriteBatch );               

            //Draw the rings
            Vector2[] vCenters = new Vector2[m_vMiniRingPositions.Length];
            for (int i = 0; i < m_vMiniRingPositions.Length; ++i)
            {
                vCenters[i] = m_vMiniRingPositions[i];
                vCenters[i].X += (int)X;
                vCenters[i].Y += (int)Y;
            }
            Vector2[] vCannonCenters = new Vector2[ m_vCannonBallPositions.Length ];
            for( int i = 0; i < m_vCannonBallPositions.Length; ++i )
            {
                vCannonCenters[ i ] = m_vCannonBallPositions[ i ];
                vCannonCenters[ i ].X += (int)X;
                vCannonCenters[ i ].Y += (int)Y;
            }
            //Vector2 vGoldDisplayCenter = new Vector2( X + m_vGoldDisplayPosition.X, Y + m_vGoldDisplayPosition.Y );

            Ship[] ships    = m_player.Ships;
            Vector2 vOrigin = new Vector2( m_texMiniRing.Width * 0.5f, m_texMiniRing.Height * 0.5f );
            Vector2 vCannonOrigin = new Vector2( m_texCannonBall.Width * 0.5f, m_texCannonBall.Height * 0.5f );

            bool bDrawnY = false;

            for( int i = 0; i < ships.Length; ++i )
            {
                //Get the ship status
                if( ships[ i ]== null )
                {
                    spriteBatch.Draw( m_texMiniRingHalf, vCenters[ i ], null, m_colFade, 0, vOrigin, 1.0f, SpriteEffects.None, 0.1f );

                    if( m_player.Active )
                    {
                        //Can purchase a boat
                        if( m_player.Port.Coins > 0 && bDrawnY == false )
                        {
                            spriteBatch.Draw(m_texBuy, vCenters[i], null, m_colFade, 0, vOrigin, 1.0f, SpriteEffects.None, 0.1f);
                            spriteBatch.Draw( m_texMiniRing, vCenters[ i ], null, m_colFade, 0, vOrigin, 1.0f,  SpriteEffects.None, 0.1f );
                            bDrawnY = true;
                        }
                        else
                        {
                            spriteBatch.Draw( m_texMiniRingHalf, vCenters[ i ], null, m_colFade, 0, vOrigin, 1.0f, SpriteEffects.None, 0.1f );
                            //Hasn't/can't be purchased
                           // spriteBatch.Draw( m_texMiniRingHalf, vCenters[i], null, m_colFade, 0, vOrigin, 1.0f, SpriteEffects.None, 0.1f);
                        }
                    }
                    else
                    {
                        //Hasn't/can't be purchased
                       // spriteBatch.Draw( m_texMiniRingHalf, vCenters[ i ], null, m_colFade, 0, vOrigin, 1.0f,  SpriteEffects.None, 0.1f );
                    }
                    
                }
                else if( ships[ i ].isDisabled )
                {
                    //Down
                    spriteBatch.Draw( m_texDown, vCenters[ i ], null, m_colFade, 0, vOrigin, 1.0f,  SpriteEffects.None, 0.1f );
                }
                else
                {
                    if (ships[ i ].hasBooty)
                    {
                        //Coin
                        spriteBatch.Draw( m_texCoin, vCenters[ i ], null, m_colFade, 0, vOrigin, 1.0f,  SpriteEffects.None, 0.1f );
                    }
                    else
                    {
                        //Up
                        spriteBatch.Draw( m_texUp, vCenters[ i ], null, m_colFade, 0, vOrigin, 1.0f,  SpriteEffects.None, 0.1f );
                    }
                }

                
                
                //Draw the minirings
                Color colRing = m_colFade;

                if( m_player.Active && m_player.CurrentShip == ships[ i ] )
                {
                    colRing = Color.Gold;
                    colRing.A = m_colFade.A;
                }

                if( ships[ i ] != null )
                {
                    spriteBatch.Draw( m_texMiniRing, vCenters[ i ], null, colRing, 0, vOrigin, 1.0f,  SpriteEffects.None, 0.1f );
                    //draw cannon ball if the player is active
                    if( ships[ i ].HasCannon == true && m_player.Active && !ships[ i ].isDisabled )
                    {
                        //draw cannon ball
                        spriteBatch.Draw( m_texCannonBall, vCannonCenters[ i ], null, colRing, 0, vCannonOrigin, 1.0f, SpriteEffects.None, 0.1f );
                    }
                }
                //else
                  //  spriteBatch.Draw( m_texMiniRingHalf, vCenters[ i ], null, colRing, 0, vOrigin, 1.0f, SpriteEffects.None, 0.1f );
            }

            m_goldDisplay.ParentPos = ScreenPosition;
            m_goldDisplay.Draw( spriteBatch, font );
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the hud item
         *  @param  fElapsedTime [in] time elapsed since the last frame in seconds
         */
        public override void Update( float fElapsedTime )
        {
            m_goldDisplay.CoinAmount = m_player.Port.Coins;

            m_animGlow.Update( fElapsedTime );
            m_goldDisplay.Update( fElapsedTime );

            m_fDisplaytime -= 5.0f * fElapsedTime;
            float fFadeOutCutoff = DISPLAY_TIME - 10.0f;
          
            if( m_fDisplaytime <= DISPLAY_TIME && m_fDisplaytime > fFadeOutCutoff )
            {
                float fDiff = DISPLAY_TIME - m_fDisplaytime;

                float fPercentElapsed =  fDiff / ( DISPLAY_TIME - fFadeOutCutoff );

                m_colFade.A         = (byte)( 255 * ( fPercentElapsed ) );
                m_goldDisplay.Alpha = fPercentElapsed;
            }

            //Start the animation when the player is activated
            if( m_bPreviouslyActive == false && m_player.Active == true )
            {
                m_animGlow.Start();
            }

            //Store the player's previous activation state.
            m_bPreviouslyActive = m_player.Active;
        }

        #endregion
    }
}
