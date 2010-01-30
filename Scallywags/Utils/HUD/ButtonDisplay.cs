using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scallywags
{
    public class ButtonDisplay : HudItem
    {
        const float DISPLAY_TIME = 40.0f;

        #region DATA_MEMBERS

        private Dictionary< string, Texture2D >   m_dcXboxButtons;              ///< stores the list of button textures
        private string                            m_strQueueButton;             ///< The name of the button that we are queueing to draw next
        private float                             m_fDisplaytime;               ///< The time we will be displaying the button

        private Color                             m_colFade;                    ///< to apply a fade to the object
        private bool                              m_bEnabled;

        #endregion

        #region Properties

        /** @prop     DelayTime
         *  @brief  Tom: I added this to keep the Buttons displayed during the Pause Screen
         */
        public float DisplayTime
        {
            set
            {
                m_fDisplaytime = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return m_bEnabled;
            }
            set
            {
                m_bEnabled = value;
            }
        }

        #endregion

        #region CLASS_CONSTRUCTOR

        /** @fn     ButtonDisplay()
         *  @brief  Class constructor
         */
        public ButtonDisplay()
        {
            m_dcXboxButtons     = new Dictionary< string, Texture2D >();
            m_strQueueButton    = string.Empty;
            m_fDisplaytime      = 0.0f;
        }

        #endregion

        #region METHODS

        /** @fn     void Init( Dictionary< string, Texture2D > dcXboxButtons )
         *  @brief  Initializes the DuttonDisplay class
         *  @param  dcXboxButtons [in] the dictionary list of all the 2d sprites that are loaded in
         */
        public void Init( Dictionary< string, Texture2D > dcXboxButtons )
        {
            m_dcXboxButtons = dcXboxButtons;
            m_colFade   = Color.White;
            m_colFade.A = 0;
            m_bEnabled = true;
        }

        /** @fn     void QueueButtonToDraw( string strButtonName )
         *  @brief  Queues up the next button we will be drawing, resets the display time and the alpha values back to their defults for the new button
         *  @param  strButtonName [in] the name of the button will will be drawing
         */
        public void QueueButtonToDraw( string strButtonName )
        {
            //m_fDisplaytime      = DISPLAY_TIME;
            m_strQueueButton    = strButtonName;

            m_colFade.A = 0;
        }

        /** @fn     void Kill()
         *  @brief  Gets the button off screen right away
         */
        public void Kill()
        {
            //m_strQueueButton    = string.Empty;
            //m_fDisplaytime      = 0.0f;
            m_colFade.A         = 0;
        }

        /** @fn     void Draw( SpriteBatch spriteBatch, SpriteFont font )
         *  @brief  Draws the button onto the screen that we specifie
         *  @param  spriteBatch [in] the handle to the spritebatch for drawing purposes
         *  @param  font [in] the handle to the sprite font
         */
         public override void Draw( SpriteBatch spriteBatch, SpriteFont font )
        { 
            //if( m_fDisplaytime > 0.0f )
            //{               
            if (m_bEnabled == true)
            {
                spriteBatch.Draw(m_dcXboxButtons[m_strQueueButton],
                                  new Vector2(X, Y),
                                  null,
                                  m_colFade,
                                  0,
                                  new Vector2(m_dcXboxButtons[m_strQueueButton].Width * 0.5f, m_dcXboxButtons[m_strQueueButton].Height * 0.5f),
                                  1.0f,
                                  SpriteEffects.None,
                                  0.1f);
            }
            //}
        }

         public override void Update(float fElapsedTime) { }
        /** @fn     void Update( float fElapsedTime )
         *  @brief  Updates the buttondisplay class handles the fading in and out for us in here
         *  @param  fElapsedTime [in] the elapsed time that has passes since the last frame was called
         */
        public void Update( float fElapsedTime, float IdleTime )
        {

            if (IdleTime < 3.0)
            {
                if (m_colFade.A > 0)
                {
                    m_colFade.A -= 3;
                }
            }
            else
            {
                if (m_colFade.A < 200)
                    m_colFade.A += 3;
            }
           


            //m_fDisplaytime -= 5.0f * fElapsedTime;

            //float fFadeOutCutoff = DISPLAY_TIME - 10.0f;

            //if( m_fDisplaytime < 1.0f )
            //{
            //    m_colFade.A = (byte)( 255 * m_fDisplaytime );
            //}
            //else if (m_fDisplaytime <= DISPLAY_TIME && m_fDisplaytime > fFadeOutCutoff)
            //{
            //    float fDiff = DISPLAY_TIME - m_fDisplaytime;

            //    float fPercentElapsed = fDiff / (DISPLAY_TIME - fFadeOutCutoff);

            //    m_colFade.A = (byte)(255 * (fPercentElapsed));
            //}
            //else //To create a non faded in Button Display I had to create the assumed alternate of the other two -Tom
            //{
            //    m_colFade.A = 255;
            //}
        }

        #endregion
    }
}
