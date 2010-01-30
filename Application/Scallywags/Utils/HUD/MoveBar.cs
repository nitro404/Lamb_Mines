using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  MoveBar
     *  @brief  the hud item that shows how much further the player can move
     */
    public class MoveBar : HudItem
    {
        private const float DISPLAY_TIME        = 40.0f;
        private const int MAX_WIDTH             = 300;      ///< The max width of the movement bar, in pixels
        private const int HEIGHT                = 20;
                                            
        private Player          m_playerCurrent;        ///< The player who is moving
        private Texture2D       m_texBar;               ///< The bar texture
        private Texture2D       m_texBorder;            ///< The boarder of the movementbar
        private Texture2D       m_texBacking;           ///< The backing for the moevementbar   
        private Texture2D       m_texWater;             ///< The movement 50% that will display in the bar
        private float           m_fPercentage;          ///< The percent of move points the user has left
        private Color           m_colCurrent;           ///< The current colour
        
        private float           m_fDisplaytime;                 ///< The time we will be displaying the button                                                  
        private Color           m_colFade;              ///< The fade in color

        /** @prop   CurrentPlayer
         *  @brief  the player the move bar will represent
         */
        public Player CurrentPlayer
        {
            set
            {
                m_playerCurrent = value;
            }
        }

        /** @fn     MoveBar()
         *  @brief  constructor
         */
        public MoveBar()
        {
            //Width           = MAX_WIDTH;
            //Height          = HEIGHT;
            m_playerCurrent = null;
            m_fPercentage   = 0;
        }

        /** @fn     void Init( GraphicsDevice device )
         *  @brief  initialize the move bar
         *  @param  device [in] the graphics device 
         */
         public void Init( GraphicsDevice device, Texture2D texBorder, Texture2D texBacking, Texture2D texWater )
         {
             //1 x 20 texture
             m_texBar       = new Texture2D(device, 1, HEIGHT);
             m_texBorder    = texBorder;
             m_texBacking   = texBacking;
             m_texWater     = texWater;
             m_colCurrent   = Color.DarkGreen;

             m_fDisplaytime = DISPLAY_TIME;
             m_colFade.A    = 0;
             m_colFade      = Color.White;
             m_colCurrent.A = 0;
             m_colCurrent   = Color.DarkGreen;

             InitTex();

             base.Init(texBorder.Width, texBorder.Height);
         }

        /** @fn     void InitTex
         *  @brief  set the color of the move bar
         *  @param  clr [in] the current colour
         */
        private void InitTex()
        {
            int nGradientOffset = 20;

            Color[] colPixels = new Color[HEIGHT];
            m_texBar.GetData<Color>(colPixels);

            //Set the pixels, apply a gradient
            for (int i = 0; i < colPixels.Length; ++i)
            {
                //colPixels[i] = clr;

                float fBlackPercentage = (colPixels.Length - i + nGradientOffset) / (float)colPixels.Length;

                colPixels[i].R = (byte)(255 * fBlackPercentage);
                colPixels[i].G = (byte)(255 * fBlackPercentage);
                colPixels[i].B = (byte)(255 * fBlackPercentage);
                colPixels[i].A = 255;
            }

            colPixels[0].A = 100;
            colPixels[HEIGHT-1].A = 100;

            m_texBar.SetData<Color>(colPixels);
        }

        /** @fn     void Draw( float fX, float fY )
         *  @brief  draw the hud item
         *  @param  spriteBatch [in] a handle to the drawing device
         *  @param  fX [in] the x coordinate between 0 and 1.0 to draw the hud item
         *  @param  fY [in] the y coordinate between 0 and 1.0 to draw the hud item
         *  @param  font [in] a sprite font to draw debug output with
         */
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if( m_playerCurrent == null )
                return;

            int nCurrentWidth = (int)(m_fPercentage * MAX_WIDTH);
            float fWidth = (float)spriteBatch.GraphicsDevice.Viewport.Width;
            float fX = (fWidth - m_texBacking.Width ) / 2;
            float fY = Y + ( Height - m_texBacking.Height ) / 2;

            spriteBatch.Draw( m_texBacking, 
                              new Vector2( fX, fY ),
                              m_colFade );

            fX = (fWidth - MAX_WIDTH) / 2.0f;
            fY = Y + ( Height - HEIGHT ) / 2.0f - 1;
            spriteBatch.Draw(m_texBar, new Rectangle((int)fX, (int)fY, nCurrentWidth, HEIGHT ), m_colCurrent);

            fX = (fWidth - m_texBorder.Width) / 2;
            fY = Y + ( Height - m_texBorder.Height ) / 2;//Y + ( m_texBacking.Height - m_texBorder.Height ) / 2;

            spriteBatch.Draw( m_texBorder, 
                              new Vector2( fX, fY ),
                              m_colFade );           
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the hud item
         *  @param  fElapsedTime [in] time elapsed since the last frame in seconds
         */
        public override void Update( float fElapsedTime )
        {
            m_fPercentage = m_playerCurrent.MovePoints / m_playerCurrent.InitialMovePoints;
            m_fDisplaytime -= 5.0f * fElapsedTime;
            float fFadeOutCutoff = DISPLAY_TIME - 10.0f;
          
            if( m_fDisplaytime <= DISPLAY_TIME && m_fDisplaytime > fFadeOutCutoff )
            {
                float fDiff = DISPLAY_TIME - m_fDisplaytime;

                float fPercentElapsed =  fDiff / ( DISPLAY_TIME - fFadeOutCutoff );

                m_colFade.A     = (byte)( 255 * ( fPercentElapsed ) );
                m_colCurrent.A  = (byte)( 255 * ( fPercentElapsed ) );
            }
            else if( m_fPercentage > 0.5f )
                m_colCurrent = Color.DarkGreen;
            else if( m_fPercentage > 0.25f )
                m_colCurrent = Color.Yellow;
            else
                m_colCurrent = Color.Red;
        }
    }
}
