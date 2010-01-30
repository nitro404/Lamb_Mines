using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class XNASlideBar : MenuItem
    {
        #region DATA_MEMBERS

        const int CURSOR_SIZE_STEPS  = 100;

        int         m_nNumberOfSteps;   ///< The number of possible positions for the slide bar
        int         m_nCurrentStep;     ///< The current position of the slide bar
        Texture2D   m_texBG;            ///< The background texture
        Texture2D   m_texCursor;        ///< The cursor texture

        int         m_nCursorSize;      ///< The scale of the cursor
        float       m_fCursorScale;     ///< The percentage scale of the cursor
                                        
        #endregion

        #region PROPERTIES

        /** @prop   StepPosition
         *  @brief  the current position of the slide bar
         */
        public int StepPosition
        {
            get
            {
                return m_nCurrentStep;
            }
        }

        /** @prop   PositionPercent
         *  @brief  the percent of distance the cursor has travelled, ( 0 = left side, 1.0f = right side )
         */
        public float PositionPercent
        {
            get
            {
                return (float)m_nCurrentStep / (float)m_nNumberOfSteps;
            }
            set
            {
                m_nCurrentStep = (int)( value * (float)m_nNumberOfSteps );
            }
        }

        #endregion

        #region INTERFACE

        /** @fn     SlideBar( int nSteps )
         *  @brief  constructor
            @param  nSteps [in] the number of steps the slide bar will contain
         */
        public XNASlideBar(int nSteps)
        {
            m_nNumberOfSteps    = nSteps;
            m_nCurrentStep      = 0;
            m_texBG             = null;
            m_texCursor         = null;

            m_fCursorScale      = 1.0f;
            m_nCursorSize       = CURSOR_SIZE_STEPS / 2;
        }

        /** @fn     void Init( Texture2D texBG, Texture2D texCursor )
         *  @brief  initialize the slidebar
         *  @param  texBG [in] the texture to use for the background
         *  @param  texCursor [in] the texture to use for the cursor
         */
        public void Init( Texture2D texBG, Texture2D texCursor )
        {
            m_fCursorScale  = 1.0f;
            m_nCursorSize   = CURSOR_SIZE_STEPS / 2;

            m_texBG     = texBG;
            m_texCursor = texCursor;

            Width   = m_texBG.Width;
            Height  = m_texBG.Height;
        }
        
        /** @fn     void Draw( SpriteBatch sb )
         *  @brief  draw the UIobject
         *  @param  sb [in] the active sprite batch
         */
        public override void Draw(SpriteBatch sb)
        {
            //Draw the background
            sb.Draw( m_texBG, new Vector2( X, Y ), Color.White );

            //Draw the position cursor
            float fHorizontalBuffer = 5.0f;                                                         //The left and right margin
            float fSlideWidth       = Width - ( 2.0f * fHorizontalBuffer ) - m_texCursor.Width;     //The allowable horizontal slide area width

            float fX = ( X + fHorizontalBuffer + m_texCursor.Width / 2.0f ) + PositionPercent * fSlideWidth;
            float fY = Y + m_texCursor.Height / 2.0f - ( m_texCursor.Height - Height ) / 2.0f;

            sb.Draw( 
                m_texCursor, 
                new Vector2( fX, fY ), 
                null, 
                Color.White, 
                0, 
                new Vector2( m_texCursor.Width / 2.0f, m_texCursor.Height / 2.0f  ), 
                m_fCursorScale, 
                SpriteEffects.None, 
                0.1f );
        }

        /** @fn     void Update( float fElapsedTime, InputManager inputs  )
         *  @brief  update the UIObject
         *  @param  fElapsedTime [in] the time since the last frame, in seconds.
         *  @param  inputs [in] the state of the input
         */
        public override void Update(float fElapsedTime, InputManager inputs)
        {
            if( Selected )
            {
                m_nCursorSize = ( m_nCursorSize + 1 ) % CURSOR_SIZE_STEPS;  //0-100
                int nTemp = m_nCursorSize - CURSOR_SIZE_STEPS / 2;    //-50 - 50

                int nScaleFactor = Math.Abs(nTemp); //0 -> 50 -> 0 -> 50...
            
                float fAddedPercent = (float)nScaleFactor / (float)CURSOR_SIZE_STEPS / 4.0f;

                m_fCursorScale = 1.0f + fAddedPercent;
            }
            else
                m_fCursorScale = 1.0f;
        }

        /** @fn     void SlideRight
         *  @brief  slide the cursor right
         */
        public void SlideRight()
        {
            if( m_nCurrentStep < m_nNumberOfSteps )
                m_nCurrentStep++;
        }

        /** @fn     void SlideLeft
         *  @brief  slide the cursor left
         */
        public void SlideLeft()
        {
            if( m_nCurrentStep > 0 )
                m_nCurrentStep--;
        }

        #endregion
    }
}
