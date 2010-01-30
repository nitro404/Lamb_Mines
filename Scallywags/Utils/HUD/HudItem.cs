using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    /** @class  HudItem
     *  @brief  base class for items belonging to the HUD
     */
    public abstract class HudItem
    {
        #region DATA_MEMBERS

        private int     m_nWidth;       ///< the width of the hud item
        private int     m_nHeight;      ///< the height of the hud item

        private Vector2 m_vScreenPos    = Vector2.Zero;   ///< The position of the hud item on the screen.  Typically its top left corner, but might be center as well

        #endregion

        #region PROPERTIES
        
        public Vector2 ScreenPosition
        {
            get
            {
                return m_vScreenPos;
            }
            set
            {
                m_vScreenPos = value;
            }
        }

        /** @prop   float Width
         *  @brief  Get and Set for the hud item's width
         */
        public int Width
        {
            get
            {
                return m_nWidth;
            }
        }

        /** @prop   float Height
         *  @brief  Get and Set for the hud item's height
         */
        public int Height
        {
            get
            {
                return m_nHeight;
            }
        }

        /** @prop   float X
         *  @brief  the X screen coordinate of the hud item
         */
        public float X
        {
            get
            {
                return m_vScreenPos.X;
            }
            set
            {
                m_vScreenPos.X = value;
            }
        }

        /** @prop   float Y
         *  @brief  the Y screen position of the hud item
         */
        public float Y
        {
            get
            {
                return m_vScreenPos.Y;
            }
            set
            {
                m_vScreenPos.Y = value;
            }
        }

        #endregion

        #region METHODS

        /** @fn     void Init( float fX, float fY, float fWidth, float fHeight )
         *  @brief  set up the hud item properties
         *  @param  fX [in] the X screen pos
         *  @param  fY [in] the Y screen pos
         *  @param  fWidth [in] the width of the hud item, in screen units
         *  @param  fHeight [in] the height of the hud item in screen units
         */
        public void Init( float fWidth, float fHeight )
        {
            m_nWidth = (int)fWidth;
            m_nHeight = (int)fHeight;
        }

        /** @fn     void Draw( float fX, float fY )
         *  @brief  draw the hud item
         *  @param  spriteBatch [in] a handle to the drawing device
         *  @param  font [in] a sprite font to draw debug output with
         */
        public abstract void Draw( SpriteBatch spriteBatch, SpriteFont font );

        /** @fn     void Update( float fElapsedTime )
         *  @brief  update the hud item
         *  @param  fElapsedTime [in] time elapsed since the last frame in seconds
         */
        public abstract void Update( float fElapsedTime );

        #endregion

    }
}
