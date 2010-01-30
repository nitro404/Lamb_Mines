using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  CannonFuse
     *  @brief  the hud item representing the amount of time the player has to fire
     */
    public class CannonFuse : HudItem
    {
        private float       m_fElapsedTime;     ///< How long has the fuse been active?
        private float       m_fTotalFireTime;   ///< The time it takes to fire a shot                

        private Texture2D   m_texCannon;        ///< The cannon texture
        private Texture2D   m_texFuse;          ///< The fuse texture
        private Texture2D   m_texSpark;         ///< Texture for sparks
                                                ///
        private bool        m_bActive;          ///< Is the fuse active?
                                                
        private List< Spark >   m_lstSparks;    ///< The sparks
        private Random          m_random;       ///< Random number generator
                                                
        const int FUSE_SPARKS = 1000;

        /** @prop   Active
         *  @brief  is the fuse active?
         */
        public bool Active
        {
            get
            {
                return m_bActive;
            }
        }

        public CannonFuse()
        {
            m_lstSparks = new List<Spark>();
            m_random    = new Random();
        }

        /** @fn     void LightFuse( bool bLightFuse )
         *  @brief  light, or extinguish, the fuse
         *  @param  bLightFuse [in] true to light the fuse, false otherwise
         */
        public void LightFuse( bool bLightFuse )
        {
            m_bActive       = bLightFuse;
            m_fElapsedTime  = 0;
        }

        /** @fn     void Init( Texture2D texCannon, Texture2D texFuse )
         *  @brief  initalize the cannon fuse
         *  @param  texCannon [in] the texture representing the cannon
         *  @param  texFuse [in] the texture representing the fuse
         */
        public void Init( Texture2D texCannon, Texture2D texFuse )
        {
            m_texCannon         = texCannon;
            m_texFuse           = texFuse;
            m_fTotalFireTime    = Settings.SHIP_FIRE_TIMER;
            m_fElapsedTime      = 0;

            m_bActive = false;

            base.Init(
                texCannon.Width + texFuse.Width,
                texCannon.Height);

            ///////////////////////////////////////
            //Create the sparks
            m_texSpark = new Texture2D( texCannon.GraphicsDevice, 1, 1 );
            Color[] vTexColor = { Color.White };
            m_texSpark.SetData< Color >( vTexColor );

            m_lstSparks = new List<Spark>();
            for( int i = 0; i < FUSE_SPARKS; ++i )
                m_lstSparks.Add( new Spark() );
        }

        /** @fn     void Draw( float fX, float fY )
         *  @brief  draw the hud item
         *  @param  spriteBatch [in] a handle to the drawing device
         *  @param  fX [in] the x coordinate at which to draw the hud item
         *  @param  fY [in] the y coordinate at which to draw the hud item
         *  @param  font [in] a sprite font to draw debug output with
         */
        public override void Draw( SpriteBatch spriteBatch, SpriteFont font )
        {
            //Draw the fuse
            float fPercentVisible = 1.0f - m_fElapsedTime / m_fTotalFireTime;

            int nDrawRegionRight    = (int)(m_texFuse.Width * fPercentVisible);
            int nSparkPixelOffset   = 5;
            int nFuseStartX         = (int)X + m_texCannon.Width - 10;
            int nFuseY              = (int)Y + m_texFuse.Height / 2;

            //Draw the left side of the fuse

            Rectangle rectDest = new Rectangle(
                nFuseStartX,
                nFuseY,
                nDrawRegionRight - nSparkPixelOffset,
                m_texFuse.Height );

            Rectangle rectSource = new Rectangle(
                0, 0,
                nDrawRegionRight - nSparkPixelOffset, 
                m_texFuse.Height );

            spriteBatch.Draw( m_texFuse, rectDest, rectSource, Color.White );

            //Draw the right side of the fuse
            rectDest = new Rectangle(
                nFuseStartX + nDrawRegionRight - nSparkPixelOffset,
                nFuseY,
                nSparkPixelOffset,
                m_texFuse.Height);

            rectSource = new Rectangle(
                nDrawRegionRight, 0,
                nSparkPixelOffset,
                m_texFuse.Height);

            Color clrRed = Color.Red;
            spriteBatch.Draw(m_texFuse, rectDest, rectSource, clrRed);

            rectDest = new Rectangle(
                nFuseStartX + nDrawRegionRight - nSparkPixelOffset / 2,
                nFuseY,
                nSparkPixelOffset / 2,
                m_texFuse.Height );

            rectSource = new Rectangle( 
                nDrawRegionRight + nSparkPixelOffset / 2,
                0, 
                nSparkPixelOffset / 2,
                m_texFuse.Height );

            Color clrOrange = Color.Orange;
            spriteBatch.Draw(m_texFuse, rectDest, rectSource, clrOrange);

            //Draw the cannon slightly overlapping the fuse
            spriteBatch.Draw(m_texCannon, new Vector2(X, Y), Color.White);

            //Draw the sparks
            foreach( Spark spark in m_lstSparks )
            {
                if( spark.fLife > 0 )
                {
                    Vector2 vPos = new Vector2( X + m_texCannon.Width + nDrawRegionRight + spark.vPosition.X - 10, 
                        nFuseY + spark.vPosition.Y );

                    spriteBatch.Draw(m_texSpark, vPos, spark.color);
                }
            }
        }

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the hud item
         *  @param  fElapsedTime [in] time elapsed since the last frame in seconds
         */
        public override void Update(float fElapsedTime)
        {
            if( m_bActive )
            {
                if( m_fElapsedTime < m_fTotalFireTime )
                    m_fElapsedTime += fElapsedTime;

                //Create some sparks...
                foreach( Spark spark in m_lstSparks )
                {
                    if( spark.fLife <= 0 )
                    {
                        spark.fLife     = (float)( m_random.NextDouble() );
                        spark.vVelocity = new Vector2( (float)( m_random.NextDouble() ) * 50.0f, (float)m_random.NextDouble() * -50.0f );
                        spark.vPosition = new Vector2( (float)m_random.NextDouble() * -2.0f, (float)(m_random.NextDouble() * m_texFuse.Height));
                    }
                    else
                    {
                        spark.fLife         -= fElapsedTime;
                        spark.vVelocity.Y   += 2.0f;

                        spark.vPosition += spark.vVelocity * fElapsedTime;

                        spark.color.A = (byte)(spark.fLife * 255);
                        spark.color.G = (byte)( spark.fLife * 255 );
                    }
                }
            }
        }
    }
}
